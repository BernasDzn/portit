:- consult('../services/scheduling_service.pl').

% Api entrypoint to handle scheduling requests
handle_schedule_request(Request) :-
    schedule_daily_operations('2025-11-23', Result),

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