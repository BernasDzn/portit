:- dynamic vessel/6.
:- dynamic crane/2.

% ============================================================================
% ALGORITHM SELECTION POLICY
% ============================================================================
% Rules:
% If computation time limit is less than 5 seconds, prefer greedy
% (1-3 vessels): Use optimal algorithm
% (4-6 vessels): Use greedy algorithm
% (7+ vessels): Use genetic algorithm 
% If there are many operations per vessel, consider complexity factor

% Main entry point: Select the best algorithm based on problem characteristics
% Input: ComputationTimeLimit (in seconds) - maximum allowed computation time
% Output: SelectedAlgorithm - one of 'optimal', 'greedy', or 'genetic'
%         SelectionReason - explanation of why this algorithm was chosen
select_algorithm(ComputationTimeLimit, SelectedAlgorithm, SelectionReason) :-
    % Evaluate problem scale
    evaluate_problem_scale(VesselCount, CraneCount, AvgOperations, Complexity),
    
    % Apply selection policy
    apply_selection_policy(VesselCount, CraneCount, AvgOperations, Complexity, 
                          ComputationTimeLimit, SelectedAlgorithm, SelectionReason),
    
    % Log the decision
    format(user_error, '~n=== AUTOMATIC ALGORITHM SELECTION ===~n', []),
    format(user_error, 'Problem Scale:~n', []),
    format(user_error, '  - Vessels: ~w~n', [VesselCount]),
    format(user_error, '  - Cranes: ~w~n', [CraneCount]),
    format(user_error, '  - Avg Operations/Vessel: ~w~n', [AvgOperations]),
    format(user_error, '  - Complexity Score: ~w~n', [Complexity]),
    format(user_error, '  - Time Limit: ~w seconds~n', [ComputationTimeLimit]),
    format(user_error, 'Selected Algorithm: ~w~n', [SelectedAlgorithm]),
    format(user_error, 'Reason: ~w~n', [SelectionReason]),
    format(user_error, '====================================~n~n', []).

% ============================================================================
% PROBLEM SCALE EVALUATION
% ============================================================================
% Analyzes the current problem to determine its scale and complexity
% Output: VesselCount - number of vessels to schedule
%         CraneCount - number of available cranes
%         AvgOperations - average operations per vessel
%         Complexity - overall complexity score (0-100)
evaluate_problem_scale(VesselCount, CraneCount, AvgOperations, Complexity) :-
    % Count vessels
    findall(V, vessel(V, _, _, _, _, _), Vessels),
    length(Vessels, VesselCount),
    
    % Count cranes
    findall(C, crane(C, _), Cranes),
    length(Cranes, CraneCount),
    
    % Calculate average operations per vessel
    calculate_avg_operations(Vessels, AvgOperations),
    
    % Calculate complexity score
    calculate_complexity(VesselCount, CraneCount, AvgOperations, Complexity).

% Calculate average number of operations (load + unload) per vessel
calculate_avg_operations([], 0) :- !.
calculate_avg_operations(Vessels, AvgOperations) :-
    findall(Ops, 
            (member(V, Vessels), 
             vessel(V, _, _, UnloadCount, LoadCount, _),
             Ops is UnloadCount + LoadCount),
            OperationsList),
    sum_list(OperationsList, TotalOps),
    length(Vessels, VesselCount),
    (VesselCount > 0 -> AvgOperations is TotalOps / VesselCount ; AvgOperations is 0).

% Calculate overall complexity score (0-100)
% Formula: (VesselCount * 15) + (CraneCount * 5) + (AvgOperations / 10)
calculate_complexity(VesselCount, CraneCount, AvgOperations, Complexity) :-
    VesselFactor is VesselCount * 15,
    CraneFactor is CraneCount * 5,
    OperationFactor is AvgOperations / 10,
    ComplexityRaw is VesselFactor + CraneFactor + OperationFactor,
    Complexity is min(100, ComplexityRaw).

% ============================================================================
% SELECTION POLICY APPLICATION
% ============================================================================
% Applies the algorithm selection policy based on problem characteristics

% Rule 1: Very strict time limit - always use greedy
apply_selection_policy(_, _, _, _, TimeLimit, "greedy", Reason) :-
    TimeLimit < 5,
    !,
    format(atom(Reason), 'Very strict time limit (~w seconds) - using fast greedy algorithm', [TimeLimit]).

% Rule 2: Small problem (1-3 vessels) - use optimal
apply_selection_policy(VesselCount, _, _, _, _, "optimal", Reason) :-
    VesselCount =< 3,
    !,
    format(atom(Reason), 'Small problem size (~w vessels) - using optimal algorithm for best solution', [VesselCount]).

% Rule 3: Medium problem (4-6 vessels) - use greedy
apply_selection_policy(VesselCount, _, _, _, _, "greedy", Reason) :-
    VesselCount >= 4,
    VesselCount =< 6,
    !,
    format(atom(Reason), 'Medium problem size (~w vessels) - using greedy algorithm for balanced performance', [VesselCount]).

% Rule 4: Large problem (7+ vessels) - use genetic
apply_selection_policy(VesselCount, _, _, _, _, "genetic", Reason) :-
    VesselCount >= 7,
    !,
    format(atom(Reason), 'Large problem size (~w vessels) - using genetic algorithm for tractability', [VesselCount]).

% Rule 5: High complexity score - use genetic
apply_selection_policy(_, _, _, Complexity, _, "genetic", Reason) :-
    Complexity > 60,
    !,
    format(atom(Reason), 'High complexity score (~w/100) - using genetic algorithm', [Complexity]).

% Rule 6: Default fallback - use greedy
apply_selection_policy(VesselCount, CraneCount, AvgOps, Complexity, _, "greedy", Reason) :-
    format(atom(Reason), 'Default selection (V:~w, C:~w, Ops:~w, Cmplx:~w) - using greedy algorithm', 
           [VesselCount, CraneCount, AvgOps, Complexity]).
