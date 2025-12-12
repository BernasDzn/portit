:- initialization(run).

:- use_module('config.pl').
:- consult('http-server.pl').

% :- dynamic api_url/1.
% :- dynamic frontend_url/1.

run :-
	PORT = 2228,
	open_server(PORT),
	writeln('Server started. Press Ctrl+C to stop.'),

    current_prolog_flag(argv, Argv),
    dispatch(Argv),
    (frontend_url(FEURL) -> writeln(FEURL) ; writeln('No frontend_url set')),
    (api_url(APIURL) -> writeln(APIURL) ; writeln('No api_url set')).

dispatch([]) :-
    % Run in thread get message loop
    % Aka keep the thread alive
    writeln('Running in production mode...'),
	thread_get_message(_).

dispatch(['test'|_]) :-

    % Set the testing endpoints
    retractall(api_url(_)),
    retractall(frontend_url(_)),

    assertz(api_url('http://localhost:5195')),
    assertz(frontend_url('http://localhost:5173')),

    % Let the server receive commands
    writeln('Running in test mode...'),
    thread_get_message(_).