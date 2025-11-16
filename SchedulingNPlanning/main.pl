:- initialization(run).

:- consult('http-server.pl').

run :-
    URL = 'https://vs-gate.dei.isep.ipp.pt:10228/Staff',
	PORT = 2228,
	open_server(PORT),
	writeln('Server started. Press Ctrl+C to stop.'),

    current_prolog_flag(argv, Argv),
    dispatch(Argv).

dispatch([]) :-
    % Run in thread get message loop
    % Aka keep the thread alive
    writeln('Running in production mode...'),
	thread_get_message(_).

dispatch(['test'|_]) :-
    % Let the server receive commands
    writeln('Running in test mode...').