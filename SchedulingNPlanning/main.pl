:- initialization(run).

:- consult('http-server.pl').

run :-
	URL = 'https://localhost:5001/Staff',
	PORT = 8080,
	open_server(PORT).