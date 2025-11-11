:- consult('./vvn_service.pl').
:- consult('../dml/vvn_mapper.pl').
:- consult('../algorithms/resource_allocation_task_sequencing.pl').

% Schedule daily operations given a date
% This predicate will see what operations need to be scheduled for loading or unloading on a given date 
% following the scheduling algorithm.
schedule_daily_operations(TargetDate, DaysAhead, DockCode, ScheduleResult) :-
    % Fetch data from database
    get_vvns_on_day(TargetDate, DaysAhead, DockCode, JsonData),

    % Parse JSON data to extract vessel facts
    JsonList = JsonData.vesselTaskFacts,
    extract_scheduling_data(JsonList, VesselFacts),

    % format(user_error, 'Vessel Facts: ~w~n', [VesselFacts]),

    % Cleanup any previous facts
    retractall(vessel(_,_,_,_,_)),
    
    % Assert new facts dynamically
    assert_vessel_facts(VesselFacts),

    % Pass the list of vessel names to sequence_temporization
    obtain_seq_shortest_delay(ScheduleResult, _),
    format(user_error, 'Schedule Result: ~w~n', [ScheduleResult]).

% get only the names of the vessels from the vessel facts
get_vessel_names([], []).
get_vessel_names([vessel(Name,_,_,_,_)|Rest], [Name|RestNames]) :-
    get_vessel_names(Rest, RestNames).

% Dynamically assert vessel facts into the knowledge base
assert_vessel_facts([]).
assert_vessel_facts([vessel(Name, ArrivalTime, DepartureTime, UnloadingTime, LoadingTime)|Rest]) :-
    assertz(vessel(Name, ArrivalTime, DepartureTime, UnloadingTime, LoadingTime)),
    assert_vessel_facts(Rest).

% Extract scheduling data from JSON list into a list of vessel facts
extract_scheduling_data([], []).
extract_scheduling_data([VesselJson|RestJson], [VesselFact|RestFacts]) :-
    json_to_vvn_fact(VesselJson, VesselFact),
    extract_scheduling_data(RestJson, RestFacts).