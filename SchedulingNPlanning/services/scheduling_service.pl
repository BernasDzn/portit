:- consult('./vvn_service.pl').
:- consult('./crane_service.pl').
:- consult('../dml/vvn_mapper.pl').
:- consult('../dml/crane_mapper.pl').
:- consult('../algorithms/optimal_scheduling.pl').
:- consult('../algorithms/greedy_scheduling.pl').

:- use_module(library(lists)).

:- dynamic current_schedule_day/1.

date_weekday(DateString, Weekday) :-
    split_string(DateString, "-", "", [YearStr, MonthStr, DayStr]),
    number_string(Year, YearStr),
    number_string(Month, MonthStr),
    number_string(Day, DayStr),
    day_of_the_week(date(Year, Month, Day), TempWeekday),
    % make sunday = 0 instead of 7
    Weekday is TempWeekday mod 7.

% Schedule daily operations given a date with algorithm selection
% This predicate will see what operations need to be scheduled for loading or unloading on a given date 
% following the specified scheduling algorithm.
schedule_daily_operations(TargetDate, DaysAhead, DockCode, Algorithm, ScheduleResult, Metrics) :-

    % Fetch data from database
    get_vvns_on_day(TargetDate, DaysAhead, DockCode, JsonData),
    
    % Set the current schedule day for use in other predicates
    date_weekday(TargetDate, Weekday),
    assertz(current_schedule_day(Weekday)),
    
    % use axuiliary predicate to check for errors
    schedule_daily_operations1(JsonData, Algorithm, ScheduleResult, Metrics),
    retractall(current_schedule_day(_)).

schedule_daily_operations1(JsonData, _, 'Missing resource (qualified staff or STS cranes on dock)', Metrics) :-
    _{ error: ErrorMsg } :< JsonData,
    format(user_error, 'Error in JSON Data: ~w~n', [JsonData]),
    !,
    Metrics = #{
        algorithm: none,
        totalDelay: 0,
        computationTime: 0,
        vesselCount: 0
    }.

schedule_daily_operations1(JsonData, Algorithm, ScheduleResult, Metrics) :-
    % Parse JSON data to extract vessel facts
    extract_scheduling_data(JsonData, VesselFacts, CraneFacts),
    
    format(user_error, 'Extracted Vessel Facts: ~w~n', [VesselFacts]),
    format(user_error, 'Extracted Crane Facts: ~w~n', [CraneFacts]),

    % Cleanup any previous facts
    retractall(vessel(_,_,_,_,_,_)),
    retractall(crane(_,_)),

    flatten(VesselFacts, FlatVesselFacts),
    flatten(CraneFacts, FlatCraneFacts),

    % Assert new facts dynamically
    assert_crane_facts(FlatCraneFacts),

    % STEP 1: Try single-crane (fastest crane only)
    format(user_error, '~n=== PHASE 1: Trying single-crane scheduling ===~n', []),
    assert_vessel_facts_single_crane(FlatVesselFacts),
    run_scheduling_algorithm(Algorithm, SingleResultRaw, SingleDelay, SingleTime),
    format(user_error, 'Single-crane delay: ~w~n', [SingleDelay]),
    % Capture vessel assignments at this point
    capture_vessel_assignments(SingleResultRaw, SingleResult),
    
    % STEP 2: If there are delays, try multi-crane with permutations
    ( SingleDelay > 0 ->
        format(user_error, '~n=== PHASE 2: Delays detected, trying multi-crane permutations ===~n', []),
        retractall(vessel(_,_,_,_,_,_)),
        get_time(MultiStartTime),
        find_best_crane_assignment(FlatVesselFacts, Algorithm, MultiResultRaw, MultiDelay),
        get_time(MultiEndTime),
        MultiTime is MultiEndTime - MultiStartTime,
        format(user_error, 'Best multi-crane delay: ~w~n', [MultiDelay]),
        % Capture vessel assignments at this point
        capture_vessel_assignments(MultiResultRaw, MultiResult),
        
        % STEP 3: Compare and pick the best
        ( MultiDelay < SingleDelay ->
            format(user_error, 'Multi-crane is BETTER! Improvement: ~w~n', [SingleDelay - MultiDelay]),
            ScheduleResult = MultiResult,
            TotalDelay = MultiDelay,
            ComputationTime = MultiTime,
            Strategy = 'multi-crane'
        ;
            format(user_error, 'Single-crane is BETTER or EQUAL! Keeping single-crane~n', []),
            ScheduleResult = SingleResult,
            TotalDelay = SingleDelay,
            ComputationTime = SingleTime,
            Strategy = 'single-crane'
        )
    ;
        format(user_error, 'Single-crane has NO delays! Using single-crane~n', []),
        ScheduleResult = SingleResult,
        TotalDelay = SingleDelay,
        ComputationTime = SingleTime,
        Strategy = 'single-crane'
    ),

    % Prepare metrics for comparison
    length(FlatVesselFacts, VesselCount),
    Metrics = #{
        algorithm: Algorithm,
        totalDelay: TotalDelay,
        computationTime: ComputationTime,
        vesselCount: VesselCount,
        strategy: Strategy
    },
    
    format(user_error, 'Final Schedule Result: ~w~n', [ScheduleResult]),
    format(user_error, 'Final Metrics: ~w~n', [Metrics]).

% Capture crane assignments from vessel facts into the schedule result
capture_vessel_assignments([], []).
capture_vessel_assignments([(VesselName, StartTime, EndTime)|Rest], [(VesselName, StartTime, EndTime, Cranes)|RestWithCranes]) :-
    ( vessel(VesselName, _, _, _, _, CraneList) ->
        findall(CraneName, member(crane(CraneName, _), CraneList), Cranes)
    ;
        Cranes = []
    ),
    capture_vessel_assignments(Rest, RestWithCranes).

% Route to the appropriate scheduling algorithm
run_scheduling_algorithm('optimal', ScheduleResult, TotalDelay, ComputationTime) :-
    get_time(StartTime),
    obtain_seq_shortest_delay(ScheduleResult, TotalDelay),
    get_time(EndTime),
    ComputationTime is EndTime - StartTime.

run_scheduling_algorithm('greedy', ScheduleResult, TotalDelay, ComputationTime) :-
    get_time(StartTime),
    obtain_seq_greedy(ScheduleResult, TotalDelay),
    get_time(EndTime),
    ComputationTime is EndTime - StartTime.

% Default to optimal if algorithm not recognized
run_scheduling_algorithm(Algorithm, ScheduleResult, TotalDelay, ComputationTime) :-
    format(user_error, 'Unknown algorithm: "~w", defaulting to optimal~n', [Algorithm]),
    run_scheduling_algorithm('optimal', ScheduleResult, TotalDelay, ComputationTime).

% Assert vessels with SINGLE crane (fastest only)
assert_vessel_facts_single_crane([]).
assert_vessel_facts_single_crane([vessel(Name, ArrivalTime, DepartureTime, UnloadingTime, LoadingTime, _)|Rest]) :-
    findall(crane(CraneName, Speed), crane(CraneName, Speed), AllCranes),
    sort_cranes_by_speed(AllCranes, [FastestCrane|_]),
    assertz(vessel(Name, ArrivalTime, DepartureTime, UnloadingTime, LoadingTime, [FastestCrane])),
    assert_vessel_facts_single_crane(Rest).

% Assert vessels with MULTI crane - try different assignments
assert_vessel_facts_multi_crane([]).
assert_vessel_facts_multi_crane([vessel(Name, ArrivalTime, DepartureTime, UnloadingTime, LoadingTime, _)|Rest]) :-
    % For now, assign one crane per vessel (will be optimized in find_best_crane_assignment)
    assertz(vessel(Name, ArrivalTime, DepartureTime, UnloadingTime, LoadingTime, [])),
    assert_vessel_facts_multi_crane(Rest).

% Try different crane COUNT assignments and find the best one
find_best_crane_assignment(VesselFacts, Algorithm, BestResult, BestDelay) :-
    findall(crane(CraneName, Speed), crane(CraneName, Speed), AllCranes),
    length(AllCranes, NumCranes),
    findall(VName, member(vessel(VName, _, _, _, _, _), VesselFacts), VesselNames),
    length(VesselNames, NumVessels),
    
    format(user_error, 'Trying crane count permutations: ~w cranes for ~w vessels~n', [NumCranes, NumVessels]),
    
    % Generate all possible crane count assignments (each vessel can have 1 to NumCranes cranes)
    generate_crane_count_assignments(NumVessels, NumCranes, Assignments),
    length(Assignments, TotalAssignments),
    format(user_error, 'Total possible assignments: ~w~n', [TotalAssignments]),
    
    % Try each assignment and find the best
    find_best_assignment(Assignments, VesselFacts, AllCranes, Algorithm, BestResult, BestDelay).

% Generate all possible crane count assignments
% Each vessel gets a number from 1 to MaxCranes
generate_crane_count_assignments(0, _, [[]]) :- !.
generate_crane_count_assignments(NumVessels, MaxCranes, Assignments) :-
    NumVessels > 0,
    NumVessels1 is NumVessels - 1,
    generate_crane_count_assignments(NumVessels1, MaxCranes, RestAssignments),
    findall([CraneCount|RestAssignment],
            (between(1, MaxCranes, CraneCount), member(RestAssignment, RestAssignments)),
            Assignments).

% Find the best assignment by trying each one
find_best_assignment([FirstAssignment|RestAssignments], VesselFacts, AllCranes, Algorithm, BestResult, BestDelay) :-
    % Try first assignment
    retractall(vessel(_,_,_,_,_,_)),
    apply_crane_count_assignment(VesselFacts, FirstAssignment, AllCranes),
    run_scheduling_algorithm(Algorithm, FirstResult, FirstDelay, _),
    format(user_error, 'Crane counts ~w -> Delay: ~w~n', [FirstAssignment, FirstDelay]),
    
    % Try remaining assignments
    find_best_assignment_helper(RestAssignments, VesselFacts, AllCranes, Algorithm, FirstResult, FirstDelay, BestResult, BestDelay).

find_best_assignment_helper([], _, _, _, CurrentBest, CurrentDelay, CurrentBest, CurrentDelay) :- !.
find_best_assignment_helper([Assignment|Rest], VesselFacts, AllCranes, Algorithm, CurrentBest, CurrentDelay, BestResult, BestDelay) :-
    retractall(vessel(_,_,_,_,_,_)),
    apply_crane_count_assignment(VesselFacts, Assignment, AllCranes),
    run_scheduling_algorithm(Algorithm, Result, Delay, _),
    format(user_error, 'Crane counts ~w -> Delay: ~w~n', [Assignment, Delay]),
    
    ( Delay < CurrentDelay ->
        NewBest = Result,
        NewDelay = Delay
    ;
        NewBest = CurrentBest,
        NewDelay = CurrentDelay
    ),
    find_best_assignment_helper(Rest, VesselFacts, AllCranes, Algorithm, NewBest, NewDelay, BestResult, BestDelay).

% Apply crane count assignment: assign N cranes to each vessel
apply_crane_count_assignment([], [], _).
apply_crane_count_assignment([vessel(Name, Arrival, Departure, Unload, Load, _)|RestVessels], [CraneCount|RestCounts], AllCranes) :-
    % Take first CraneCount cranes from the list
    take_n_cranes(CraneCount, AllCranes, AssignedCranes),
    assertz(vessel(Name, Arrival, Departure, Unload, Load, AssignedCranes)),
    apply_crane_count_assignment(RestVessels, RestCounts, AllCranes).

% Take N cranes from the list
take_n_cranes(0, _, []) :- !.
take_n_cranes(N, [Crane|Rest], [Crane|TakenRest]) :-
    N > 0,
    N1 is N - 1,
    take_n_cranes(N1, Rest, TakenRest).
take_n_cranes(N, [], []) :- N > 0. % If not enough cranes, return what we have

% Sort cranes by speed (descending - fastest first)
sort_cranes_by_speed(Cranes, Sorted) :-
    predsort(compare_crane_speed, Cranes, SortedAsc),
    reverse(SortedAsc, Sorted).

compare_crane_speed(Order, crane(_, Speed1), crane(_, Speed2)) :-
    compare(Order, Speed1, Speed2).

assert_crane_facts([]).
assert_crane_facts([crane(Name, Speed)|Rest]) :-
    assertz(crane(Name, Speed)),
    assert_crane_facts(Rest).

% Extract scheduling data from JSON list into a list of vessel facts and scheduling facts
extract_scheduling_data([], [], []).
extract_scheduling_data(WorkloadJson, VesselFact, CraneFact) :-
    format(user_error, 'Processing Workload JSON: ~w~n', [WorkloadJson]),
    extract_vessel_data(WorkloadJson.vesselTaskFacts, VesselFact),
    extract_crane_data(WorkloadJson.craneWorkloads, CraneFact).

extract_vessel_data([], []).
extract_vessel_data([JsonData | RestJson], [VesselFact | RestVesselFacts]) :-
    json_to_vvn_fact(JsonData, VesselFact),
    format(user_error, 'Extracted Vessel Fact: ~w~n', [VesselFact]),
    extract_vessel_data(RestJson, RestVesselFacts).

extract_crane_data([], []).
extract_crane_data([JsonData | RestJson], [CraneFact | RestCraneFacts]) :-
    json_to_crane_fact(JsonData, CraneFact),
    format(user_error, 'Extracted Crane Fact: ~w~n', [CraneFact]),
    extract_crane_data(RestJson, RestCraneFacts).