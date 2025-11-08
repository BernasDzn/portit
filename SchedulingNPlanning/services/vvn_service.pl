:- use_module(library(http/json_convert)).
:- use_module(library(http/json)).
:- use_module(library(http/http_open)).
:- use_module(library(http/http_json)).

get_all_vvn(_Request) :-
    http_open("http://localhost:5195/VesselVisitNotification", Stream, []),
    json_read_dict(Stream, JsonData),
    close(Stream),
    reply_json(JsonData).