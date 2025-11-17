:- consult('./vvn_service.pl').
:- consult('../dml/vvn_mapper.pl').
:- consult('../dml/operational_window_mapper.pl').
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

    % ok this is gonna caus eproblems with the operational window
    % so lets just set days ahead to 1 for now
    DaysAhead = 1,

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
    JsonList = JsonData.craneWorkloads,
    extract_scheduling_data(JsonList, VesselFacts, IntervalFact),
    
    format(user_error, 'Extracted Vessel Facts: ~w~n', [VesselFacts]),
    format(user_error, 'Extracted Interval Facts: ~w~n', [IntervalFact]),

    % Cleanup any previous facts
    retractall(vessel(_,_,_,_,_)),
    retractall(interval(_,_,_)),
    
    flatten(VesselFacts, FlatVesselFacts),
    flatten(IntervalFact, FlatIntervalFacts),

    % Assert new facts dynamically
    assert_vessel_facts(FlatVesselFacts),
    assert_interval_facts(FlatIntervalFacts),

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

% get only the names of the vessels from the vessel facts
get_vessel_names([], []).
get_vessel_names([vessel(Name,_,_,_,_,_)|Rest], [Name|RestNames]) :-
    get_vessel_names(Rest, RestNames).

% Dynamically assert vessel facts into the knowledge base
assert_vessel_facts([]).
assert_vessel_facts([vessel(Name, ArrivalTime, DepartureTime, UnloadingTime, LoadingTime)|Rest]) :-
    assertz(vessel(Name, ArrivalTime, DepartureTime, UnloadingTime, LoadingTime)),
    assert_vessel_facts(Rest).

assert_interval_facts([]).
assert_interval_facts([interval(Day, StartTime, EndTime)|Rest]) :-
    assertz(interval(Day, StartTime, EndTime)),
    assert_interval_facts(Rest).

% Extract scheduling data from JSON list into a list of vessel facts and scheduling facts
extract_scheduling_data([], [], []).
extract_scheduling_data([WorkloadJson|RestJson], [VesselFact | RestVesselFacts], [IntervalFact, RestIntervalFacts]) :-
    extract_vessel_data(WorkloadJson.vesselTaskFacts, VesselFact),
    extract_interval_data(WorkloadJson.operatingWindow.shifts, IntervalFact),
    extract_scheduling_data(RestJson, RestVesselFacts, RestIntervalFacts).

extract_vessel_data([], []).
extract_vessel_data([JsonData | RestJson], [VesselFact | RestVesselFacts]) :-
    json_to_vvn_fact(JsonData, VesselFact),
    % format(user_error, 'Extracted Vessel Fact: ~w~n', [VesselFact]),
    extract_vessel_data(RestJson, RestVesselFacts).

extract_interval_data([], []).
extract_interval_data([JsonData | RestJson], [IntervalFact | RestIntervalFacts]) :-
    json_to_interval_fact(JsonData, IntervalFact),
    % format(user_error, 'Extracted Interval Fact: ~w~n', [IntervalFact]),
    extract_interval_data(RestJson, RestIntervalFacts).