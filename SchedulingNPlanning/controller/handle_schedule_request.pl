:- use_module(library(http/json_convert)).
:- use_module(library(http/json)).
:- use_module(library(http/http_open)).
:- use_module(library(http/http_json)).

% Main entry point for the scheduling algorithm
% Format of Request:
%   host:port/schedule?target_date=YYYY-MM-DD
handle_schedule_request(Request) :-
    http_read_json_dict(Request, Data),
    TargetDate = Data.get(target_date),
    schedule_daily_operations(TargetDate, ScheduleResult),
    
    reply_json(ScheduleResult).