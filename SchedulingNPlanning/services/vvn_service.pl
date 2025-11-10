:- use_module(library(http/json_convert)).
:- use_module(library(http/json)).
:- use_module(library(http/http_open)).
:- use_module(library(http/http_json)).

:- use_module('../config.pl').

% get_all_vvn(Request) :-
%     http_open("http://localhost:5195/VesselVisitNotification", Stream, []),
%     json_read_dict(Stream, JsonData),
%     close(Stream),
%     reply_json(JsonData).

get_vvns_on_day(Date, DaysAhead, DockCode, JsonData) :-
    api_url(BaseUrl),
    format(atom(URL), "~w/VesselVisitNotification/collectScheduleData?Value=~w&daysAhead=~w&day=~w",
           [BaseUrl, DockCode, DaysAhead, Date]),
    http_open(URL, Stream, []),
    json_read_dict(Stream, JsonData),
    close(Stream).