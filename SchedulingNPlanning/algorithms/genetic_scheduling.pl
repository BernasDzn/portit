
% - Initialization variables
:- dynamic generations/1. % stores max number of generations
:- dynamic population_size/1. % stores population size
:- dynamic prob_crossover/1. % stores probability of crossover
:- dynamic prob_mutation/1. % stores probability of mutation
:- dynamic time_limit/1. % stores time limit in seconds
:- dynamic stability_limit/1. % stores stability limit (generations without improvement)

% - Tracking variables
:- dynamic start_time/1. % stores start time for timing
:- dynamic best_solution_tracker/2. % stores the best found solution (BestDelay, NumberOfGenerationsWithoutImprovement)

% - Dynamic declaration for vessel_visit/4
:- dynamic vessel_visit/4.
:- dynamic vessel/6.
:- dynamic num_vessels/1.

% --- Mapper: Transform vessel/6 to vessel_visit/4 ---
% vessel/6 format: vessel(Name, ArrivalTime, DepartureTime, UnloadContainers, LoadContainers, CraneList)
% vessel_visit/4 format: vessel_visit(VesselName, ArrivalT, DepartureT, ProcessingT)
% ProcessingT = (UnloadContainers + LoadContainers) / sum of crane speeds

% To make it easier for the service to enter we'll consider:
% 100 generations, 10 population size, 70% crossover, 10% mutation, 300 seconds time limit, 10 stability limit
obtain_seq_genetic(SeqTriplets, Delay) :-
    generate(100, 10, 70, 10, 300, 10, SeqTriplets, Delay).

% -- map_vessels_to_visits -- populate vessel_visit/4 from vessel/6 facts
    map_vessels_to_visits :-
        retractall(vessel_visit(_, _, _, _)),
        forall(
            vessel(Name, ArrivalTime, DepartureTime, UnloadContainers, LoadContainers, CraneList),
            (
                get_crane_sum(CraneList, CraneSpeed),
                TotalContainers is UnloadContainers + LoadContainers,
                (CraneSpeed > 0 -> ProcessingT is TotalContainers / CraneSpeed ; ProcessingT = 0),
                assertz(vessel_visit(Name, ArrivalTime, DepartureTime, ProcessingT))
            )
        ),
        findall(_, vessel_visit(_, _, _, _), Visits),
        length(Visits, Count),
        (retract(num_vessels(_)); true),
        asserta(num_vessels(Count)).

    get_crane_sum([], 0).
    get_crane_sum([crane(_, Speed) | RestCranes], Sum):-
        get_crane_sum(RestCranes, Sum1),
        Sum is (Sum1 + Speed).
%

% -- initialize -- initialize with or without data a priori
    initialize :-
        map_vessels_to_visits,
        write('Number of generations (Max): '), read(NG),
        (retract(generations(_)); true), asserta(generations(NG)),
        write('Population size: '), read(PS),
        (retract(population_size(_)); true), asserta(population_size(PS)),
        write('Probability of crossover (%): '), read(P1),
        PC is P1/100,
        (retract(prob_crossover(_)); true), asserta(prob_crossover(PC)),
        write('Probability of mutation (%): '), read(P2),
        PM is P2/100,
        (retract(prob_mutation(_)); true), asserta(prob_mutation(PM)),
        % Add new termination conditions:
        % - We can set a time limit (in seconds)
        % - We can set a stability limit (generations without improvement) 
        write('Time limit (seconds): '), read(TL),
        (retract(time_limit(_)); true), asserta(time_limit(TL)),
        write('Stability limit (generations without improvement): '), read(SL),
        (retract(stability_limit(_)); true), asserta(stability_limit(SL)).

    initialize(MaxGen, PopSize, ProbCross, ProbMut, TimeLim, StabilityLim) :-
        map_vessels_to_visits,
        (retract(generations(_)); true), asserta(generations(MaxGen)),
        (retract(population_size(_)); true), asserta(population_size(PopSize)),
        PC is ProbCross / 100,
        (retract(prob_crossover(_)); true), asserta(prob_crossover(PC)),
        PM is ProbMut / 100,
        (retract(prob_mutation(_)); true), asserta(prob_mutation(PM)),
        (retract(time_limit(_)); true), asserta(time_limit(TimeLim)),
        (retract(stability_limit(_)); true), asserta(stability_limit(StabilityLim)).
%

% -- generate -- Entry point for genetic algorithm

    generate(MaxGen, PopSize, ProbCross, ProbMut, TimeLim, StabilityLim, SeqTriplets, Delay) :-
        initialize(MaxGen, PopSize, ProbCross, ProbMut, TimeLim, StabilityLim),
        generate1(SeqTriplets, Delay).

    generate(SeqTriplets, Delay) :-
        initialize,
        generate1(SeqTriplets, Delay).

    generate :-
        initialize,
        generate1(SeqTriplets, Delay),
        write('Best Solution: '), write(SeqTriplets), nl,
        write('Best Cost (Delays): '), write(Delay), nl.

    generate1(SeqTriplets, Delay) :-
        %
        % start timing for time limit
        get_time(Now),
        (retract(start_time(_)); true), asserta(start_time(Now)),
        %
        % Initialize stability tracker: (Infinity, 0)
        (retract(best_solution_tracker(_,_)); true), asserta(best_solution_tracker(100000, 0)),
        %
        generate_population(Pop),
        % write('Pop='),write(Pop),nl,
        %   
        evaluate_population(Pop, PopValue),
        % write('PopValue='),write(PopValue),nl,
        %
        order_population(PopValue, PopOrd),
        %
        generations(NumGenerations),
        generate_generation(0, NumGenerations, PopOrd, BestInd, BestVal),
        %
        % Convert the best individual to triplet format to comply with other algorithms
        convert_to_triplets(BestInd, SeqTriplets),
        Delay = BestVal.
%

% -- generate_population ---
    generate_population(Pop) :-
        population_size(PopSize),
        num_vessels(NumV),
        findall(VesselN, vessel_visit(VesselN,_,_,_), VesselsList),
        generate_population(PopSize, VesselsList, NumV, Pop).

    generate_population(0, _, _, []) :- !.

    generate_population(PopSize, VesselsList, NumV, [Ind|RestPop]) :-
        PopSize1 is PopSize - 1,
        generate_population(PopSize1, VesselsList, NumV, RestPop),
        % to generate a random permutation for the individuals in the population
        % we can use the built-in random_permutation/2 predicate
        generate_unique_individual(VesselsList, RestPop, Ind).

    generate_unique_individual(VesselsList, RestPop, Ind) :-
        random_permutation(VesselsList, Ind),
        \+ member(Ind, RestPop), !.

    % Fallback: if we can't generate unique individuals (e.g., only 1 vessel),
    % just generate a random permutation (allows duplicates)
    generate_unique_individual(VesselsList, _, Ind) :-
        random_permutation(VesselsList, Ind).
%

% -- evaluate_population ---
    evaluate_population([], []).

    evaluate_population([Ind|Rest], [Ind*V|Rest1]) :-
        evaluate(Ind, V),
        evaluate_population(Rest, Rest1).

    evaluate(Seq, V) :- evaluate(Seq, 0, V).

    evaluate([], _, 0).

    evaluate([VesselName|Rest], CurrentTime, TotalCost) :-
        vessel_visit(VesselName, ArrivalTime, DepartureTime, ProcessingT),
        % Ensure the start time takes into account arrival time 
        StartTime is max(CurrentTime, ArrivalTime),
        FinishTime is StartTime + ProcessingT,
        
        evaluate(Rest, FinishTime, RestCost),

        % Calculate Delay (Cost)
        ((FinishTime =< DepartureTime,!, Delay is 0) ; 
         (Delay is FinishTime - DepartureTime) ),
        
        TotalCost is Delay + RestCost.
%

% -- order_population -- Bubble Sort on value
    order_population(PopValue,PopValueOrd):-
        bsort(PopValue,PopValueOrd).

    bsort([X],[X]):-!.
    bsort([X|Xs],Ys):-
        bsort(Xs,Zs),
        bchange([X|Zs],Ys).


    bchange([X],[X]):-!.

    bchange([X*VX,Y*VY|L1],[Y*VY|L2]):-
        VX>VY,!,
        bchange([X*VX|L1],L2).

    bchange([X|L1],[X|L2]):-bchange(L1,L2).
%

% -- generate_generation
    generate_generation(G, MaxG, [BestInd*BestVal|_], BestInd, BestVal) :-
        G >= MaxG, !,
        write('--- Max number of generations reached ---'), nl,
        write('Final Generation: '), write(G), nl,
        write('Best Solution: '), write(BestInd), nl,
        write('Best Cost (Delays): '), write(BestVal), nl.

    generate_generation(N, MaxG, Pop, BestInd, BestVal) :-
        % Ensure new termination conditions are checked first
        check_termination(N, Pop, PassedChecks),
        PassedChecks = true,
        
        write('Generation '), write(N), write('...'), nl, write(Pop), nl,
        
        % Randomize population order before crossover to avoid fixed pairings
        % Suggested improvement from slide 28 of Support TP
        random_permutation(Pop, ShuffledPop),
        
        crossover(ShuffledPop, ShuffledPopCO),
        mutation(ShuffledPopCO, ShuffledPopCOMUT),
        evaluate_population(ShuffledPopCOMUT, ShuffledPopCOMUTVal),
        order_population(ShuffledPopCOMUTVal, ShuffledPopCOMUTValOrd),
        
        ShuffledPopCOMUTValOrd = [BestChildInd*BestChildVal|_],
        update_stability(BestChildVal),
        
        N1 is N+1,
        generate_generation(N1, MaxG, ShuffledPopCOMUTValOrd, BestInd, BestVal).

    % Helper to handle early termination printing
    generate_generation(N, _, [BestInd*BestVal|_], BestInd, BestVal) :-
        write('--- TERMINATION CONDITION MET ---'), nl,
        write('Generation: '), write(N), nl,
        write('Best Solution: '), write(BestInd), nl,
        write('Best Cost (Delays): '), write(BestVal), nl.
%

% -- check_termination -- (Time limit, Stability limit, and Optimal solution)
    check_termination(_, _, false) :-
        get_time(Now), start_time(Start), time_limit(Limit),
        Elapsed is Now - Start,
        Elapsed > Limit, !,
        write('--- Max time limit reached ---'), nl.

    check_termination(_, _, false) :-
        stability_limit(Limit),
        best_solution_tracker(_, Count),
        Count >= Limit, !,
        write('--- Population stabilized (No improvement for '), 
        write(Limit), write(' generations) ---'), nl.

    check_termination(_, [_*BestVal|_], false) :-
        BestVal =:= 0, !,
        write('--- Optimal solution found (Cost = 0) ---'), nl.

    check_termination(_, _, true).
%

% -- crossover --
    crossover([], []).
    crossover([Ind*_], [Ind]).
    crossover([Ind1*_, Ind2*_|Rest], [NInd1, NInd2|Rest1]) :-
        prob_crossover(Prob_Crossover), random(0.0, 1.0, Roll),
        ((Roll =< Prob_Crossover, !,
            generate_crossover_points(P1, P2),
            cross(Ind1, Ind2, P1, P2, NInd1),
            cross(Ind2, Ind1, P1, P2, NInd2))
        ;
            (NInd1 = Ind1, NInd2 = Ind2)
        ),
        crossover(Rest, Rest1).
%

% -- generate_crossover_points --
    generate_crossover_points(P1, P2) :-
        num_vessels(N),
        NTemp is N + 1,
        random(1, NTemp, R1),
        random(1, NTemp, R2),
        (R1 == R2 ->
            generate_crossover_points(P1, P2)
        ;
            (R1 < R2 -> P1 = R1, P2 = R2 ; P1 = R2, P2 = R1)
        ).
%

% -- cross --
    cross(Ind1, Ind2, P1, P2, Child) :-
        sublist(Ind1, P1, P2, Sub1),
        num_vessels(NumV),
        RotateLen is NumV - P2,
        rotate_right(Ind2, RotateLen, Ind2Rotated),
        remove(Ind2Rotated, Sub1, CleanedInd2),
        P3 is P2 + 1,
        insert(CleanedInd2, Sub1, P3, ChildRotated),
        removeh(ChildRotated, Child).
%

% -- sublist --
    sublist(L1,I1,I2,L):-I1 < I2,!,
        sublist1(L1,I1,I2,L).

    sublist(L1,I1,I2,L):-sublist1(L1,I2,I1,L).

    sublist1([X|R1],1,1,[X|H]):-!, fillh(R1,H).

    sublist1([X|R1],1,N2,[X|R2]):-!,N3 is N2 - 1,
        sublist1(R1,1,N3,R2).

    sublist1([_|R1],N1,N2,[h|R2]):-N3 is N1 - 1,
            N4 is N2 - 1,
            sublist1(R1,N3,N4,R2).

    fillh([ ],[ ]).

    fillh([_|R1],[h|R2]):-
        fillh(R1,R2).
%

% -- rotate_right --
   rotate_right(L,K,L1):- num_vessels(N),
	T is N - K,
	rr(T,L,L1).

    rr(0,L,L):-!.

    rr(N,[X|R],R2):- N1 is N - 1,
        append(R,[X],R1),
        rr(N1,R1,R2).
%

% -- remove --
    remove([],_,[]):-!.
    remove([X|R1],L,[X|R2]):- not(member(X,L)),!,
        remove(R1,L,R2).
    remove([_|R1],L,R2):-
        remove(R1,L,R2).
%

% -- insert --
    insert([],L,_,L):-!.
    insert([X|R],L,N,L2):-
        num_vessels(T),
        ((N>T,!,N1 is N mod T);N1 = N),
        insert1(X,N1,L,L1),
        N2 is N + 1,
        insert(R,L1,N2,L2).


    insert1(X,1,L,[X|L]):-!.
    insert1(X,N,[Y|L],[Y|L1]):-
        N1 is N-1,
        insert1(X,N1,L,L1).
%

% -- removeh
    removeh([],[]).

    removeh([h|R1],R2):-!,
        removeh(R1,R2).

    removeh([X|R1],[X|R2]):-
        removeh(R1,R2).
%

% -- mutation --
    mutation([], []).
    mutation([Ind|Rest], [NInd|Rest1]) :-
        prob_mutation(Prob_Mutation),
        random(0.0, 1.0, Roll),
        ((Roll < Prob_Mutation, !, mutation1(Ind, NInd))
        ; NInd = Ind ),
        mutation(Rest, Rest1).

    mutation1(Ind,NInd):-
        generate_crossover_points(P1,P2),
        mutation2(Ind,P1,P2,NInd).

    mutation2([G1|Ind],1,P2,[G2|NInd]):-
        !, P21 is P2-1,
        mutation3(G1,P21,Ind,G2,NInd).
    mutation2([G|Ind],P1,P2,[G|NInd]):-
        P11 is P1-1, P21 is P2-1,
        mutation2(Ind,P11,P21,NInd).

    mutation3(G1,1,[G2|Ind],G2,[G1|Ind]):-!.
    mutation3(G1,P,[G|Ind],G2,[G|NInd]):-
        P1 is P-1,
        mutation3(G1,P1,Ind,G2,NInd).
%

% -- update_stability --
    update_stability(CurrentBestVal) :-
        best_solution_tracker(LastBestVal, Count),
        ((CurrentBestVal < LastBestVal, !,
            % Improvement found: Reset count, update value
            retract(best_solution_tracker(LastBestVal, Count)),
            asserta(best_solution_tracker(CurrentBestVal, 0)))
        ;
        (CurrentBestVal =:= LastBestVal, !,
            % No change: Increment count
            NewCount is Count + 1,
            retract(best_solution_tracker(LastBestVal, Count)),
            asserta(best_solution_tracker(LastBestVal, NewCount)))
        ;
            true
        ).
%

% -- convert_to_triplets -- Convert sequence to (VesselName, StartTime, EndTime) format
    convert_to_triplets(Sequence, Triplets) :-
        convert_to_triplets(Sequence, 0, Triplets).

    convert_to_triplets([], _, []).
    
    convert_to_triplets([VesselName|Rest], EndPrevSeq, [(VesselName, TInUnload, TEndLoad)|RestTriplets]) :-
        % Get vessel data to calculate proper times (matching greedy algorithm logic)
        vessel(VesselName, TIn, _, TUnloadContainers, TLoadContainers, Cranes),
        get_crane_sum(Cranes, CraneSpeed),
        
        % Calculate unload and load times separately
        (TUnloadContainers > 0, CraneSpeed > 0 -> TUnload is TUnloadContainers / CraneSpeed ; TUnload = 0),
        (TLoadContainers > 0, CraneSpeed > 0 -> TLoad is TLoadContainers / CraneSpeed ; TLoad = 0),
        
        % Start time logic (same as greedy algorithm)
        (TIn > EndPrevSeq -> TInUnload is TIn ; TInUnload is EndPrevSeq + 1),
        
        % End time formula (corrected - removed the -1 bug)
        TEndLoad is TInUnload + TUnload + TLoad,
        
        % Next vessel can start after this one finishes
        convert_to_triplets(Rest, TEndLoad, RestTriplets).
%