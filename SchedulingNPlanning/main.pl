:- initialization(run).

:- consult('http-server.pl').

run :-
	URL = 'https://vs-gate.dei.isep.ipp.pt:10228/Staff',
	PORT = 2228,
	open_server(PORT).