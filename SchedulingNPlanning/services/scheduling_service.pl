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
    assert_vessel_facts(FlatVesselFacts),

    % Run the appropriate algorithm based on selection
    run_scheduling_algorithm(Algorithm, ScheduleResult, TotalDelay, ComputationTime),

    % Prepare metrics for comparison
    length(FlatVesselFacts, VesselCount),
    Metrics = #{
        algorithm: Algorithm,
        totalDelay: TotalDelay,
        computationTime: ComputationTime,
        vesselCount: VesselCount
    },
    
    format(user_error, 'Schedule Result: ~w~n', [ScheduleResult]),
    format(user_error, 'Metrics: ~w~n', [Metrics]).

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
run_scheduling_algorithm(_, ScheduleResult, TotalDelay, ComputationTime) :-
    format(user_error, 'Unknown algorithm, defaulting to optimal~n', []),
    run_scheduling_algorithm('optimal', ScheduleResult, TotalDelay, ComputationTime).

% Dynamically assert vessel facts into the knowledge base
assert_vessel_facts([]).
assert_vessel_facts([vessel(Name, ArrivalTime, DepartureTime, UnloadingTime, LoadingTime, Crane)|Rest]) :-

    % Espeta todas as cranes logo associadas a cada vessel, depois podemos mudar dinamicamente esta lista
    % Francisco muda isto !
    findall(crane(CraneName, Speed), crane(CraneName, Speed), CraneFacts),

    assertz(vessel(Name, ArrivalTime, DepartureTime, UnloadingTime, LoadingTime, CraneFacts)),
    assert_vessel_facts(Rest).

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