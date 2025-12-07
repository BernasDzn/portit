:- consult('../services/rebalancing_service.pl').
:- consult('../config.pl').
:- use_module(library(http/http_cors)).

% API endpoint to handle dock rebalancing requests
handle_rebalance_request(Request) :-
    
    frontend_url(FrontendURL),

    % Enable CORS for frontend communication
    cors_enable(Request, [
        methods([get, post, options]),
        origin(FrontendURL)
    ]),

    % Parse URL parameters: ?date=2025-11-09&days_ahead=0&sort_by=arrival
    http_parameters(Request, [
        date(Date, [string]),
        daysAhead(DaysAhead, [integer, default(0)]),
        sortBy(SortBy, [string, default('arrival')])
    ]),

    format(user_error, '~n=== REBALANCE REQUEST ===~n', []),
    format(user_error, 'Parameters: Date=~w, DaysAhead=~w, SortBy=~w~n', [Date, DaysAhead, SortBy]),

    % Call rebalancing service
    rebalance_docks_on_day(Date, DaysAhead, SortBy, RebalancedData, Metrics),

    % Debug: Print the response structure
    format(user_error, '~nPreparing JSON response...~n', []),
    format(user_error, 'RebalancedData: ~w~n', [RebalancedData]),
    format(user_error, 'Metrics: ~w~n', [Metrics]),

    % Return JSON response
    Response = #{
        success: true,
        date: Date,
        sortBy: SortBy,
        data: RebalancedData,
        metrics: Metrics
    },
    format(user_error, 'Sending response: ~w~n', [Response]),
    reply_json(Response).
