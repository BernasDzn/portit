:- initialization(run).

:- consult('http-server.pl').

run :-
	URL = 'https://localhost:2228/Staff',
	PORT = 8080,
	open_server(PORT).