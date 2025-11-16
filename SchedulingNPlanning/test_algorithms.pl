% Test file for comparing scheduling algorithms
:- consult('./algorithms/resource_allocation_task_sequencing.pl').
:- consult('./algorithms/greedy_scheduling.pl').

% Test data - same as in resource_allocation_task_sequencing.pl
test_setup :-
    retractall(vessel(_,_,_,_,_,_)),
    assertz(vessel(zeus, 6, 63, 10, 16, 'STS001')),
    assertz(vessel(poseidon, 23, 50, 9, 7, 'STS001')),
    assertz(vessel(marenostrum, 8, 40, 5, 12, 'STS001')),
    assertz(vessel(nautilus, 10, 30, 0, 8, 'STS001')),
    assertz(vessel(floating, 36, 70, 12, 0, 'STS001')).

% Run optimal algorithm
test_optimal :-
    test_setup,
    format('~n=== OPTIMAL ALGORITHM (Exhaustive Search) ===~n'),
    obtain_seq_shortest_delay(SeqTriplets, Delay),
    format('Schedule: ~w~n', [SeqTriplets]),
    format('Total Delay: ~w hours~n', [Delay]).

% Run greedy algorithm (EDD)
test_greedy :-
    test_setup,
    format('~n=== GREEDY ALGORITHM (Earliest Due Date) ===~n'),
    obtain_seq_greedy(SeqTriplets, Delay, ComputationTime),
    format('Schedule: ~w~n', [SeqTriplets]),
    format('Total Delay: ~w hours~n', [Delay]),
    format('Computation Time: ~w seconds~n', [ComputationTime]).

% Run all tests
test_all :-
    format('~n========================================~n'),
    format('  SCHEDULING ALGORITHM COMPARISON TEST  ~n'),
    format('========================================~n'),
    test_optimal,
    test_greedy,
    format('~n========================================~n').

% Compare both algorithms side by side
compare_algorithms :-
    test_setup,
    
    % Optimal
    obtain_seq_shortest_delay(SeqOptimal, DelayOptimal),
    
    % Greedy
    obtain_seq_greedy(SeqGreedy, DelayGreedy, TimeGreedy),
    
    format('~n========================================~n'),
    format('  ALGORITHM COMPARISON SUMMARY  ~n'),
    format('========================================~n'),
    format('Algorithm       | Total Delay | Comp. Time (s)~n'),
    format('----------------|-------------|---------------~n'),
    format('Optimal         | ~10w | N/A~n', [DelayOptimal]),
    format('Greedy (EDD)    | ~10w | ~14f~n', [DelayGreedy, TimeGreedy]),
    format('========================================~n').
