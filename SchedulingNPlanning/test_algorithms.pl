% Test file for comparing scheduling algorithms
:- consult('./algorithms/optimal_scheduling.pl').
:- consult('./algorithms/greedy_scheduling.pl').
:- consult('./algorithms/genetic_scheduling.pl').

% Test data - same as in resource_allocation_task_sequencing.pl
test_setup :-
    retractall(vessel(_,_,_,_,_,_)),
    assertz(vessel(zeus, 6, 63, 10, 16, [crane('STS001',24)])),
    assertz(vessel(poseidon, 23, 50, 9, 7, [crane('STS001',24)])),
    assertz(vessel(marenostrum, 8, 40, 5, 12, [crane('STS001',24)])),
    assertz(vessel(nautilus, 10, 30, 0, 8, [crane('STS001',24)])),
    assertz(vessel(floating, 36, 70, 12, 0, [crane('STS001',24)])),
    assertz(vessel(apollo, 15, 55, 8, 10, [crane('STS001',24)])),
    assertz(vessel(hermes, 5, 45, 6, 9, [crane('STS001',24)])),
    assertz(vessel(athena, 12, 60, 7, 11, [crane('STS001',24)])).
    %assertz(vessel(hera, 20, 80, 15, 5, [crane('STS001',24)])),
    %assertz(vessel(artemis, 30, 90, 10, 10, [crane('STS001',24)])).
    % assertz(vessel(demeter, 25, 75, 9, 8, [crane('STS001',24)])). this one lasted 6 minutes.

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
    get_time(Start),
    obtain_seq_greedy(SeqTriplets, Delay),
    get_time(End),
    ComputationTime is End - Start,
    format('Schedule: ~w~n', [SeqTriplets]),
    format('Total Delay: ~w hours~n', [Delay]),
    format('Computation Time: ~w seconds~n', [ComputationTime]).

test_genetic :-
    test_setup,
    format('~n=== GENETIC ALGORITHM ===~n'),
    get_time(Start),
    generate(100, 10, 70, 10, 60, 10, SeqTriplets, Delay),
    get_time(End),
    ComputationTime is End - Start,
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
    test_genetic,
    format('~n========================================~n').

% Compare both algorithms side by side
compare_algorithms :-
    test_setup,
    
    % Optimal - measure time
    get_time(StartOptimal),
    obtain_seq_shortest_delay(SeqOptimal, DelayOptimal),
    get_time(EndOptimal),
    TimeOptimal is EndOptimal - StartOptimal,
    
    % Greedy
    get_time(StartGreedy),
    obtain_seq_greedy(SeqGreedy, DelayGreedy),
    get_time(EndGreedy),
    TimeGreedy is EndGreedy - StartGreedy,

    % Genetic
    get_time(StartGenetic),
    generate(100, 10, 70, 10, 60, 10, SeqGenetic, DelayGenetic),
    get_time(EndGenetic),
    TimeGenetic is EndGenetic - StartGenetic,
    
    format('~n========================================~n'),
    format('  ALGORITHM COMPARISON SUMMARY  ~n'),
    format('========================================~n'),
    format('Algorithm       | Total Delay | Comp. Time~n'),
    format('----------------|-------------|---------------~n'),
    TimeOptimalMs is TimeOptimal * 1000,
    format('Optimal         | ~10w | ~3f ms~n', [DelayOptimal, TimeOptimalMs]),
    TimeGreedyMs is TimeGreedy * 1000,
    format('Greedy (EDD)    | ~10w | ~3f ms~n', [DelayGreedy, TimeGreedyMs]),
    TimeGeneticMs is TimeGenetic * 1000,
    format('Genetic         | ~10w | ~3f ms~n', [DelayGenetic, TimeGeneticMs]),
    format('========================================~n').
