% Bibliotecas 
:- use_module(library(http/thread_httpd)).
:- use_module(library(http/http_dispatch)).
:- use_module(library(http/http_parameters)).

% Importa outros ficheiros Prolog
:- consult('services/vvn_service.pl').
:- consult('controller/handle_schedule_request.pl').
:- consult('controller/handle_rebalance_request.pl').

:- use_module(library(http/http_cors)).
:- set_setting(http:cors, [*]).  % allow requests from any origin

% Relação entre pedidos HTTP e predicados que os processam
:- http_handler('/lapr5', responde_ola, []).
:- http_handler('/register_user', register_user, []).
:- http_handler('/send_file_post', send_file_post, []).
:- http_handler('/get_all_vvn', get_all_vvn, []).
:- http_handler('/schedule', handle_schedule_request, []).
:- http_handler('/rebalance', handle_rebalance_request, []).

open_server(Port) :-
        write("Starting on port "), write(Port), nl,
        http_server(http_dispatch, [port(Port)]).

close_server(Port) :-
        http_stop_server(Port, []).
		
responde_ola(_Request) :-					
        format('Content-type: text/plain~n~n'),
        format('Olá LAPR5!~n').

register_user(Request) :-
    http_parameters(Request,
                    [ name(Name, []),
                      sex(Sex, [oneof([male,female])]),
                      birth_year(BY, [between(1850,10000)])
                    ]),
    format('Content-type: text/plain~n~n'),
    format('User registered!~n'),
	format('Name: ~w~nSex: ~w~nBirth Year: ~w~n',[Name,Sex,BY]).

% Método POST enviando um ficheiro de texto
% http_client:http_post('http://localhost:5000/send_file_post', form_data([file=file('./teste.txt')]), Reply, []).
send_file_post(Request) :-
	http_parameters(Request,[ file(X,[])]),
    format('Content-type: text/plain~n~n'),
	format('Received: ~w~n',[X]).