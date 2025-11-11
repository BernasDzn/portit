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

    % Parse URL parameters: ?day=2025-11-09&dock=DCK002&days_ahead=2
    http_parameters(Request, [
        day(Day, [string]),
        dock(Dock, [string]),
        daysAhead(DaysAhead, [integer, default(0)])
        % add alg(Alg, [string]) one day 
    ]),

    schedule_daily_operations(Day, DaysAhead, Dock ,Result),

    format_timetable(Result, FormattedResult),
    reply_json(#{status: success, data: FormattedResult}).

% Format the list of tuples into a more readable structure
format_timetable([], []).
format_timetable([(Name, LoadingEnterTime, LoadingExitTime)|Rest], [Dict|FormattedRest]) :-
    Dict = #{
        name: Name,
        loading_enter_time: LoadingEnterTime,
        loading_exit_time: LoadingExitTime
    },
    format_timetable(Rest, FormattedRest).