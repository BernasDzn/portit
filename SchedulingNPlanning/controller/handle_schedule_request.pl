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
        dock(Dock, [string]),
        daysAhead(DaysAhead, [integer, default(0)]),
        alg(Algorithm, [string, default('optimal')])
    ]),

    schedule_daily_operations(Day, DaysAhead, Dock, Algorithm, Result, Metrics),

    format_timetable(Result, FormattedResult),
    reply_json(#{status: success, data: FormattedResult, metrics: Metrics}).

% Format the list of tuples into a more readable structure
format_timetable([], []).
format_timetable([(Name, LoadingEnterTime, LoadingExitTime)|Rest], [Dict|FormattedRest]) :-
    Dict = #{
        name: Name,
        loading_enter_time: LoadingEnterTime,
        loading_exit_time: LoadingExitTime
    },
    format_timetable(Rest, FormattedRest).