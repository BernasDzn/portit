:- consult('./vvn_service.pl').

% Schedule daily operations given a date
% This predicate will see what operations need to be scheduled for loading or unloading on a given date 
% following the scheduling optimization algorithm.
schedule_daily_operations(TargetDate, ScheduleResult) :-
    % Fetch data from database
    get_vvns_on_day(TargetDate, ScheduleResult), 