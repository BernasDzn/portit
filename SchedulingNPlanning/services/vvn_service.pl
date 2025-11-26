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

get_vvns_on_day(Date, DaysAhead, JsonData) :-
    api_url(BaseUrl),
    format(atom(URL), "~w/VesselVisitNotification/collectScheduleData?daysAhead=~w&day=~w",
           [BaseUrl, DaysAhead, Date]),

    format(user_error, 'Fetching VVN data from URL: ~w~n', [URL]),  % Debug
    
    % Disable SSL certificate verification for self-signed certificates
    http_open(URL, Stream, [ cert_verify_hook(ssl_verify), status_code(Code)]),
    handle_response(Code, Stream, JsonData).

handle_response(200, Stream, JsonData) :-
    json_read_dict(Stream, JsonData),
    !,
    close(Stream).

handle_response(Code, Stream, JsonData) :-
    format(user_error, 'Error fetching VVN data: HTTP ~w - ~w~n', [Code, Txt]),
    close(Stream),
    JsonData = _{ error: Txt }.

% Hook to accept any SSL certificate (for development with self-signed certs)
:- public ssl_verify/5.
ssl_verify(_SSL, _ProblemCert, _AllCerts, _FirstCert, _Error).