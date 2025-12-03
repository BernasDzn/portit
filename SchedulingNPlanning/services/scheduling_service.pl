:- consult('./vvn_service.pl').
:- consult('./crane_service.pl').
:- consult('../dml/vvn_mapper.pl').
:- consult('../dml/crane_mapper.pl').
:- consult('../algorithms/optimal_scheduling.pl').
:- consult('../algorithms/greedy_scheduling.pl').
:- consult('../algorithms/genetic_scheduling.pl').
:- use_module(library(lists)).

:- dynamic current_schedule_day/1.
% The global dock being scheduled
:- dynamic dock/1.

date_weekday(DateString, Weekday) :-
    split_string(DateString, "-", "", [YearStr, MonthStr, DayStr]),
    number_string(Year, YearStr), number_string(Month, MonthStr), number_string(Day, DayStr),
    day_of_the_week(date(Year, Month, Day), TempWeekday),
    Weekday is TempWeekday mod 7.

schedule_daily_operations(TargetDate, DaysAhead, Algorithm, ScheduleResult, Metrics) :-
    get_vvns_on_day(TargetDate, DaysAhead, JsonData),
    date_weekday(TargetDate, Weekday),
    assertz(current_schedule_day(Weekday)),
    schedule_daily_operations1(JsonData, Algorithm, ScheduleResult, Metrics),
    retractall(current_schedule_day(_)).

schedule_daily_operations1(JsonData, _, 'Missing resource (qualified staff or STS cranes on dock)', Metrics) :-
    _{ error: _ } :< JsonData, !,
    Metrics = #{algorithm: none, totalDelay: 0, computationTime: 0, vesselCount: 0}.

% This is now a wrapper to the actual dispatch algorithm
% We want to seperate algorithm execution from dock selection since it will be sequential
schedule_daily_operations1(JsonData, Algorithm, ScheduleResult, Metrics) :-

    % Get dock list
    extract_dock_list(JsonData.docks, DockList),
    format(user_error, 'Available docks: ~w~n', [DockList]),

    dispatch_to_docks(DockList, JsonData, Algorithm, ScheduleResult, Metrics).
    
% Dispatch to docks sequentially
% Dispatch to docks sequentially
dispatch_to_docks([], _, _, [], []).
dispatch_to_docks([DockHead|DockTail], JsonData, Algorithm, [ScheduleHead|ScheduleRest], [MetricsHead|MetricsRest]) :-
    
    format(user_error, '~n***** Scheduling for dock: ~w *****~n', [DockHead]),
    assertz(dock(DockHead)),
    dispatch_algorithm(JsonData, Algorithm, DockSchedule, DockMetrics),

    ScheduleHead = #{dock: DockHead, schedule: DockSchedule},
    MetricsHead = DockMetrics,

    retractall(dock(_)),
    dispatch_to_docks(DockTail, JsonData, Algorithm, ScheduleRest, MetricsRest).

dispatch_algorithm(JsonData, Algorithm, ScheduleResult, Metrics) :-
    
    retractall(vessel(_,_,_,_,_,_)),
    retractall(crane(_,_)),

    extract_scheduling_data(JsonData, VesselFacts, CraneFacts, DockList),
    
    flatten(VesselFacts, FlatVesselFacts),
    flatten(CraneFacts, FlatCraneFacts),

    format(user_error, 'Vessel Facts: ~w~n', [FlatVesselFacts]),
    format(user_error, 'Crane Facts: ~w~n', [FlatCraneFacts]),
    
    maplist(assertz, FlatCraneFacts),
    
    % Try single-crane first
    format(user_error, '~n=== PHASE 1: Single-crane scheduling ===~n', []),
    assert_single_crane(FlatVesselFacts),
    run_algorithm(Algorithm, SingleRaw, SingleDelay, SingleTime),
    format(user_error, 'Single-crane delay: ~w~n', [SingleDelay]),
    capture_assignments(SingleRaw, SingleResult),
    
    % Try multi-crane if delays exist
    ( SingleDelay > 0 ->
        format(user_error, '~n=== PHASE 2: Multi-crane permutations ===~n', []),
        retractall(vessel(_,_,_,_,_,_)),
        get_time(T1),
        find_best_assignment(FlatVesselFacts, Algorithm, MultiRaw, MultiDelay),
        get_time(T2), MultiTime is T2 - T1,
        capture_assignments(MultiRaw, MultiResult),
        ( MultiDelay < SingleDelay ->
            format(user_error, 'Multi-crane BETTER! Improvement: ~w~n', [SingleDelay - MultiDelay]),
            ScheduleResult = MultiResult, TotalDelay = MultiDelay, 
            ComputationTime = MultiTime, Strategy = 'multi-crane'
        ;   ScheduleResult = SingleResult, TotalDelay = SingleDelay,
            ComputationTime = SingleTime, Strategy = 'single-crane'
        )
    ;   format(user_error, 'Single-crane has NO delays! Using single-crane~n', []),
        ScheduleResult = SingleResult, TotalDelay = SingleDelay,
        ComputationTime = SingleTime, Strategy = 'single-crane'
    ),
    
    length(FlatVesselFacts, VesselCount),
    Metrics = #{algorithm: Algorithm, totalDelay: TotalDelay, computationTime: ComputationTime, 
                vesselCount: VesselCount, strategy: Strategy},
    format(user_error, 'Final Schedule: ~w~n', [ScheduleResult]),
    format(user_error, 'Final Metrics: ~w~n', [Metrics]).

% Capture assignments
capture_assignments([], []).
capture_assignments([(VesselName, Start, End)|Rest], [(VesselName, Start, End, Cranes)|RestOut]) :-
    ( vessel(VesselName, _, _, _, _, CraneList) -> 
        findall(CraneName, member(crane(CraneName, _), CraneList), Cranes) 
    ; Cranes = [] ),
    capture_assignments(Rest, RestOut).

% Run algorithm with timing
run_algorithm(Algorithm, Result, Delay, Time) :-
    get_time(StartTime),
    ( Algorithm == "optimal" -> obtain_seq_shortest_delay(Result, Delay)
    ; Algorithm == "greedy" -> obtain_seq_greedy(Result, Delay)
    ; Algorithm == "genetic" -> obtain_seq_genetic(Result, Delay)
    ; format(user_error, 'Unknown algorithm ~w, using optimal~n', [Algorithm]), obtain_seq_shortest_delay(Result, Delay) ),
    get_time(EndTime), 
    Time is EndTime - StartTime.

% Assert single crane (fastest)
assert_single_crane([]).
assert_single_crane([vessel(Name, Arrival, Departure, Unload, Load, _)|Rest]) :-
    findall(crane(CraneName, Speed), crane(CraneName, Speed), AllCranes),
    % C# already filters and sends available cranes, so just take first (fastest)
    AllCranes = [crane(FastestName, FastestSpeed)|_],
    format(user_error, 'Vessel ~w assigned crane: ~w (speed: ~w)~n', [Name, FastestName, FastestSpeed]),
    assertz(vessel(Name, Arrival, Departure, Unload, Load, [crane(FastestName, FastestSpeed)])),
    assert_single_crane(Rest).

% Find best crane assignment
find_best_assignment(VesselFacts, Algorithm, BestResult, BestDelay) :-
    findall(crane(CraneName, Speed), crane(CraneName, Speed), AllCranes),
    length(AllCranes, NumCranes), 
    length(VesselFacts, NumVessels),
    format(user_error, 'Trying ~w cranes for ~w vessels~n', [NumCranes, NumVessels]),
    gen_assignments(NumVessels, NumCranes, Assignments),
    length(Assignments, TotalAssignments),
    format(user_error, 'Total assignments: ~w~n', [TotalAssignments]),
    test_assignments(Assignments, VesselFacts, AllCranes, Algorithm, BestResult, BestDelay).

% Generate crane count assignments
gen_assignments(0, _, [[]]) :- !.
gen_assignments(NumVessels, MaxCranes, Assignments) :-
    NumVessels > 0, 
    NumVessels1 is NumVessels - 1,
    gen_assignments(NumVessels1, MaxCranes, RestAssignments),
    findall([CraneCount|RestAssignment], 
            (between(1, MaxCranes, CraneCount), member(RestAssignment, RestAssignments)), 
            Assignments).

% Test all assignments
test_assignments([FirstAssignment|RestAssignments], VesselFacts, AllCranes, Algorithm, BestResult, BestDelay) :-
    retractall(vessel(_,_,_,_,_,_)),
    apply_assignment(VesselFacts, FirstAssignment, AllCranes),
    run_algorithm(Algorithm, FirstResult, FirstDelay, _),
    sum_list(FirstAssignment, FirstTotal),
    format(user_error, '~w (total: ~w) -> Delay: ~w~n', [FirstAssignment, FirstTotal, FirstDelay]),
    test_helper(RestAssignments, VesselFacts, AllCranes, Algorithm, FirstResult, FirstDelay, FirstAssignment, 
                BestResult, BestDelay, BestAssignment),
    sum_list(BestAssignment, BestTotal),
    format(user_error, '~n=== SELECTED: ~w (total: ~w) delay: ~w ===~n', [BestAssignment, BestTotal, BestDelay]),
    retractall(vessel(_,_,_,_,_,_)),
    apply_assignment(VesselFacts, BestAssignment, AllCranes),
    run_algorithm(Algorithm, BestResult, BestDelay, _).

test_helper([], _, _, _, CurrentBest, CurrentDelay, CurrentAssignment, CurrentBest, CurrentDelay, CurrentAssignment) :- !.
test_helper([Assignment|RestAssignments], VesselFacts, AllCranes, Algorithm, CurrentBest, CurrentDelay, CurrentAssignment, 
            BestResult, BestDelay, BestAssignment) :-
    retractall(vessel(_,_,_,_,_,_)),
    apply_assignment(VesselFacts, Assignment, AllCranes),
    run_algorithm(Algorithm, Result, Delay, _),
    sum_list(Assignment, TotalCranes), 
    sum_list(CurrentAssignment, CurrentTotal),
    format(user_error, '~w (total: ~w) -> Delay: ~w~n', [Assignment, TotalCranes, Delay]),
    ( Delay < CurrentDelay ->
        format(user_error, '  -> BETTER: Lower delay~n', []),
        NewBest = Result, NewDelay = Delay, NewAssignment = Assignment
    ; Delay =:= CurrentDelay, TotalCranes < CurrentTotal ->
        format(user_error, '  -> BETTER: Fewer cranes (~w < ~w)~n', [TotalCranes, CurrentTotal]),
        NewBest = Result, NewDelay = Delay, NewAssignment = Assignment
    ;   NewBest = CurrentBest, NewDelay = CurrentDelay, NewAssignment = CurrentAssignment
    ),
    test_helper(RestAssignments, VesselFacts, AllCranes, Algorithm, NewBest, NewDelay, NewAssignment, 
                BestResult, BestDelay, BestAssignment).

% Apply assignment
apply_assignment([], [], _).
apply_assignment([vessel(Name, Arrival, Departure, Unload, Load, _)|RestVessels], [CraneCount|RestCounts], AllCranes) :-
    take_n(CraneCount, AllCranes, AssignedCranes),
    assertz(vessel(Name, Arrival, Departure, Unload, Load, AssignedCranes)),
    apply_assignment(RestVessels, RestCounts, AllCranes).

take_n(0, _, []) :- !.
take_n(Count, [Crane|RestCranes], [Crane|TakenRest]) :- 
    Count > 0, 
    Count1 is Count - 1, 
    take_n(Count1, RestCranes, TakenRest).
take_n(Count, [], []) :- Count > 0.

% Extract data
extract_scheduling_data([], [], [], []).
extract_scheduling_data(JsonData, VesselFacts, CraneFacts, DockList) :-
    extract_crane_data(JsonData.craneWorkloads, CraneFacts),
    extract_vessel_data(JsonData.vesselTaskFacts, VesselFacts).

extract_vessel_data([], []).
extract_vessel_data([JsonVessel|RestJson], Result) :-
    dock(DockCode),
    (
        JsonVessel.dock \= DockCode ->
        extract_vessel_data(RestJson, Result)
        ; 
        json_to_vvn_fact(JsonVessel, VesselFact), 
        extract_vessel_data(RestJson, RestVessels),
        Result = [VesselFact|RestVessels] 
    ).

extract_crane_data([], []).
extract_crane_data([JsonCrane|RestJson], Result) :-
    dock(DockCode),
    ( 
        JsonCrane.dock \= DockCode ->
        extract_crane_data(RestJson, Result)
        ;
        json_to_crane_fact(JsonCrane, CraneFact), 
        extract_crane_data(RestJson, RestCranes),
        Result = [CraneFact|RestCranes] 
    ).

extract_dock_list([], []).
extract_dock_list([JsonDock|RestDocks], [DockHead|DockTail]) :-
    DockHead = JsonDock.code,
    extract_dock_list(RestDocks, DockTail).