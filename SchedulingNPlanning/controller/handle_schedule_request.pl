:- consult('../services/scheduling_service.pl').
:- consult('../config.pl').

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
        alg(Algorithm, [string, default('optimal')]),
        compare(Compare, [boolean, default(false)])
    ]),

    % Handle comparison mode or single algorithm mode
    ( Compare = true ->
        schedule_with_comparison(Day, DaysAhead, FormattedResult, Metrics)
    ;
        schedule_daily_operations(Day, DaysAhead, Algorithm, Result, Metrics),
        format_timetable(Result, FormattedResult)
    ),

    reply_json(#{data: FormattedResult, metrics: Metrics}).

% Format the list of tuples into a more readable structure with crane info
format_timetable([], []).
% Handle 4-tuple format (Name, StartTime, EndTime, Cranes) - new format with cranes included
format_timetable([(Name, LoadingEnterTime, LoadingExitTime, Cranes)|Rest], [Dict|FormattedRest]) :-
    Dict = #{
        name: Name,
        loading_enter_time: LoadingEnterTime,
        loading_exit_time: LoadingExitTime,
        cranes: Cranes
    },
    !,
    format_timetable(Rest, FormattedRest).
% Handle 3-tuple format (Name, StartTime, EndTime) - legacy format, lookup cranes from facts
format_timetable([(Name, LoadingEnterTime, LoadingExitTime)|Rest], [Dict|FormattedRest]) :-
    ( vessel(Name, _, _, _, _, CraneList), CraneList \= [] ->
        findall(CraneName, member(crane(CraneName, _), CraneList), Cranes),
        Dict = #{
            name: Name,
            loading_enter_time: LoadingEnterTime,
            loading_exit_time: LoadingExitTime,
            cranes: Cranes
        }
    ;
        Dict = #{
            name: Name,
            loading_enter_time: LoadingEnterTime,
            loading_exit_time: LoadingExitTime
        }
    ),
    !,
    format_timetable(Rest, FormattedRest).

% Fall back, should only be called for non list inputs
format_timetable(Error, Error).