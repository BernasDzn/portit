:- consult('./vvn_service.pl').
:- consult('../dml/vvn_mapper.pl').
:- consult('../algorithms/resource_allocation_task_sequencing.pl').
:- consult('../algorithms/greedy_scheduling.pl').

:- use_module(library(lists)).

% Schedule daily operations given a date with algorithm selection
% This predicate will see what operations need to be scheduled for loading or unloading on a given date 
% following the specified scheduling algorithm.
schedule_daily_operations(TargetDate, DaysAhead, DockCode, Algorithm, ScheduleResult, Metrics) :-
    % Fetch data from database
    get_vvns_on_day(TargetDate, DaysAhead, DockCode, JsonData),

    % Parse JSON data to extract vessel facts
    JsonList = JsonData.craneWorkloads,
    extract_scheduling_data(JsonList, VesselFacts),

    % Cleanup any previous facts
    retractall(vessel(_,_,_,_,_,_)),
    
    flatten(VesselFacts, FlatVesselFacts),
    format(user_error, 'Vessel Facts: ~w~n', [FlatVesselFacts]),

    % Assert new facts dynamically
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
    obtain_seq_greedy(ScheduleResult, TotalDelay).
    get_time(EndTime),
    ComputationTime is EndTime - StartTime.

% Default to optimal if algorithm not recognized
run_scheduling_algorithm(_, ScheduleResult, TotalDelay, ComputationTime) :-
    format(user_error, 'Unknown algorithm, defaulting to optimal~n', []),
    run_scheduling_algorithm('optimal', ScheduleResult, TotalDelay, ComputationTime).

% get only the names of the vessels from the vessel facts
get_vessel_names([], []).
get_vessel_names([vessel(Name,_,_,_,_,_)|Rest], [Name|RestNames]) :-
    get_vessel_names(Rest, RestNames).

% Dynamically assert vessel facts into the knowledge base
assert_vessel_facts([]).
assert_vessel_facts([vessel(Name, ArrivalTime, DepartureTime, UnloadingTime, LoadingTime, Crane)|Rest]) :-
    assertz(vessel(Name, ArrivalTime, DepartureTime, UnloadingTime, LoadingTime, Crane)),
    assert_vessel_facts(Rest).

% Extract scheduling data from JSON list into a list of vessel facts and scheduling facts
extract_scheduling_data([], []).
extract_scheduling_data([WorkloadJson|RestJson], [VesselFact | RestVesselFacts ]) :-
    extract_vessel_data(WorkloadJson.vesselTaskFacts, VesselFact, WorkloadJson.crane),
    extract_scheduling_data(RestJson, RestVesselFacts).

extract_vessel_data([], [], _).
extract_vessel_data([JsonData | RestJson], [VesselFact | RestVesselFacts], Crane) :-
    json_to_vvn_fact(JsonData, Crane, VesselFact),
    format(user_error, 'Extracted Vessel Fact: ~w~n', [VesselFact]),
    extract_vessel_data(RestJson, RestVesselFacts, Crane).