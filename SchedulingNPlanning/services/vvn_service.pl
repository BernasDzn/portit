:- use_module(library(http/json_convert)).
:- use_module(library(http/json)).
:- use_module(library(http/http_open)).
:- use_module(library(http/http_json)).

% get_all_vvn(Request) :-
%     http_open("http://localhost:5195/VesselVisitNotification", Stream, []),
%     json_read_dict(Stream, JsonData),
%     close(Stream),
%     reply_json(JsonData).

get_vvns_on_day(Date, DaysAhead, DockCode, JsonData) :-
    format(atom(URL), "http://localhost:5195/VesselVisitNotification/collectScheduleData?Value=~w&daysAhead=~w&day=~w", [DockCode, DaysAhead, Date]),
    http_open(URL, Stream, []),
    json_read_dict(Stream, JsonData),
    close(Stream).