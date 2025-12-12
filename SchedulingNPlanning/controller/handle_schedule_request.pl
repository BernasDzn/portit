:- consult('../services/scheduling_service.pl').
:- consult('../config.pl').
:- use_module(library(http/http_cors)).

% Api entrypoint to handle scheduling requests
handle_schedule_request(Request) :-
    
    frontend_url(FrontendURL),

    % Enable CORS for frontend communication
    cors_enable(Request, [
        methods([get, post, options]),
        origin(FrontendURL)
    ]),

    % Parse URL parameters: ?day=2025-11-09&dock=DCK002&days_ahead=2&alg=greedy
    http_parameters(Request, [
        day(Day, [string]),
        daysAhead(DaysAhead, [integer, default(0)]),
        alg(Algorithm, [string, default('auto')]),
        compare(Compare, [boolean, default(false)])
    ]),

    % Handle comparison mode or single algorithm mode
    ( Compare = true ->
        schedule_with_comparison(Day, DaysAhead, FormattedResult, Metrics)
    ;
        schedule_daily_operations(Day, DaysAhead, Algorithm, FormattedResult, Metrics)
    ),

    % Clean up facts after formatting is complete
    retractall(dock(_)),
    retractall(vessel(_,_,_,_,_,_)),
    retractall(crane(_,_)),

    format(user_error, 'Scheduling result: ~w~n', [FormattedResult]),

    reply_json(#{data: FormattedResult, metrics: Metrics, date: Day}).

format_timetable_docks([], []).
format_timetable_docks([DockResult|Rest], [Dict|FormattedRest]) :-
    DockCode = DockResult.dock,
    ScheduleList = DockResult.schedule,
    format_timetable(ScheduleList, FormattedSchedule),
    Dict = #{dock: DockCode, schedule: FormattedSchedule},
    !,
    format_timetable_docks(Rest, FormattedRest).

% Helper to calculate unload and load times for a vessel
calculate_vessel_times(Name, CraneNames, UnloadTime, LoadTime) :-
    vessel(Name, _, _, UnloadCount, LoadCount, _),
    % Look up the actual crane facts from the crane names
    findall(Speed, (member(CraneName, CraneNames), crane(CraneName, Speed)), Speeds),
    sum_list(Speeds, Sum),
    % Calculate times
    ( (UnloadCount > 0, Sum > 0) -> UnloadTime is (UnloadCount / Sum) ; UnloadTime = 0 ),
    ( (LoadCount > 0, Sum > 0) -> LoadTime is (LoadCount / Sum) ; LoadTime = 0 ).

% Format the list of tuples into a more readable structure with crane info
format_timetable([], []).
% Handle 4-tuple format (Name, StartTime, EndTime, Cranes) - new format with cranes included
% StartTime = when unloading enters, EndTime = when loading exits
format_timetable([(Name, UnloadingEnterTime, LoadingExitTime, Cranes)|Rest], [Dict|FormattedRest]) :-
    is_list(Cranes), % Ensure it's actually a 4-tuple
    !,
    % Calculate the intermediate times using the Cranes from the tuple
    ( Cranes \= [] ->
        calculate_vessel_times(Name, Cranes, UnloadTime, LoadTime),
        % Unloading happens first
        UnloadingExitTime is UnloadingEnterTime + UnloadTime,
        % Loading starts after unloading ends
        LoadingEnterTime is UnloadingExitTime
    ;
        % If no cranes, assume zero times
        UnloadingExitTime = UnloadingEnterTime,
        LoadingEnterTime = UnloadingEnterTime
    ),
    Dict = #{
        name: Name,
        cranes: Cranes,
        unloading_enter_time: UnloadingEnterTime,
        unloading_exit_time: UnloadingExitTime,
        loading_enter_time: LoadingEnterTime,
        loading_exit_time: LoadingExitTime
    },
    format_timetable(Rest, FormattedRest).
% Handle 3-tuple format (Name, StartTime, EndTime) - legacy format, lookup cranes from facts
format_timetable([(Name, UnloadingEnterTime, LoadingExitTime)|Rest], [Dict|FormattedRest]) :-
    number(LoadingExitTime),  % Ensure it's actually a 3-tuple, not a 4-tuple mismatched
    ( vessel(Name, _, _, _, _, CraneList), CraneList \= [] ->
        findall(CraneName, member(crane(CraneName, _), CraneList), Cranes),
        calculate_vessel_times(Name, Cranes, UnloadTime, LoadTime),
        % Unloading happens first
        UnloadingExitTime is UnloadingEnterTime + UnloadTime,
        % Loading starts after unloading ends
        LoadingEnterTime is UnloadingExitTime,
        Dict = #{
            name: Name,
            cranes: Cranes,
            unloading_enter_time: UnloadingEnterTime,
            unloading_exit_time: UnloadingExitTime,
            loading_enter_time: LoadingEnterTime,
            loading_exit_time: LoadingExitTime
        }
    ;
        % Fallback for vessels without cranes
        Dict = #{
            name: Name,
            cranes: [],
            unloading_enter_time: UnloadingEnterTime,
            unloading_exit_time: UnloadingEnterTime,
            loading_enter_time: UnloadingEnterTime,
            loading_exit_time: LoadingExitTime
        }
    ),
    !,
    format_timetable(Rest, FormattedRest).

% Fall back, should only be called for non list inputs
format_timetable(Error, Error).