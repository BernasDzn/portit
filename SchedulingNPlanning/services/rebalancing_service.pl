:- consult('./vvn_service.pl').
:- consult('./crane_service.pl').
:- consult('../algorithms/dock_assignment.pl').
:- use_module(library(http/json)).

% Service for dock rebalancing operations
% Fetches vessel data and computes optimal dock assignments

rebalance_docks_on_day(Date, DaysAhead, SortBy, RebalancedData, Metrics) :-
    format(user_error, '~n=== DOCK REBALANCING SERVICE ===~n', []),
    format(user_error, 'Date: ~w, Days Ahead: ~w, Sort By: ~w~n', [Date, DaysAhead, SortBy]),
    
    % Get vessel visit notifications for the date
    get_vvns_on_day(Date, DaysAhead, JsonData),
    
    % Check if data was retrieved successfully
    ( _{ error: ErrorMsg } :< JsonData ->
        format(user_error, 'ERROR: ~w~n', [ErrorMsg]),
        RebalancedData = #{error: ErrorMsg},
        Metrics = #{error: true}
    ;
        % Call the rebalancing algorithm with error handling
        format(user_error, 'Calling rebalancing algorithm...~n', []),
        catch(
            (
                rebalance_docks(JsonData, SortBy, RebalancedData, Metrics),
                format(user_error, '~nRebalancing completed successfully~n', [])
            ),
            Error,
            (
                format(user_error, '~nERROR in rebalancing algorithm: ~w~n', [Error]),
                RebalancedData = #{error: "Internal rebalancing error"},
                Metrics = #{error: true}
            )
        )
    ).
