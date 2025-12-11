% First support for IARTI project 2025/2026
% Scheduling Vessels Unload/Load

:-dynamic shortest_delay/2.
:- dynamic vessel/6.
:- dynamic interval/3.
:- dynamic crane/2.

% interval(0, 0, 24).
% interval(1, 0, 24).
% interval(2, 0, 24).
% interval(3, 0, 24).
% interval(4, 0, 24).
% interval(5, 0, 24).
% interval(6, 0, 24).

% vessel(zeus, 6, 63, 10, 16, [crane('STS001', 24)]).
% vessel(poseidon, 23, 50, 9, 7, [crane('STS001', 24)]).
% vessel(marenostrum, 8, 40, 5, 12, [crane('STS001', 24)]).
% vessel(nautilus, 10, 30, 0, 8, [crane('STS001', 24)]).
% vessel(floating, 36, 70, 12, 0, [crane('STS001', 24)]).

% crane('STS001', 24).
% crane('STS002', 2).
% crane('STS003', 30).

get_crane_sum_optimal([], 0).
get_crane_sum_optimal([crane(_, Speed) | RestCranes], Sum):-
    get_crane_sum_optimal(RestCranes, Sum1),
    Sum is (Sum1 + Speed).

calculate_load_unload_time_optimal(LoadCount, UnloadCount, CraneList, LoadTime, UnloadTime):-
    get_crane_sum_optimal(CraneList, Sum),
    % Time = Containers / Speed
    ( (LoadCount > 0, Sum > 0) -> LoadTime is (LoadCount / Sum) ; LoadTime = 0 ),
    ( (UnloadCount > 0, Sum > 0) -> UnloadTime is (UnloadCount / Sum) ; UnloadTime = 0 ).

% Sequence temporization
sequence_temporization(LV,SeqTriplets):-
	sequence_temporization1(0,LV,SeqTriplets).

sequence_temporization1(EndPrevSeq,[V|LV],[(V,TInUnload,TEndLoad)|SeqTriplets]):-
    vessel(V,TIn,_,TUnloadC,TLoadC, Cranes),
    calculate_load_unload_time_optimal(TUnloadC,TLoadC, Cranes,TUnload, TLoad),
    
    ( (TIn> EndPrevSeq,!, TInUnload is TIn); TInUnload is EndPrevSeq + 1),
    (format(user_error,'~nVessel: ~w TInUnload: ~w TUnload: ~w TLoad: ~w~n',[V,TInUnload,TUnload,TLoad]), true),
    
    TEndLoad is TInUnload + TUnload + TLoad,
    sequence_temporization1(TEndLoad,LV,SeqTriplets).

sequence_temporization1(_,[],[]).

% Find the sum of delays
sum_delays_optimal([],0).

sum_delays_optimal([(V,_,TEndLoad)|LV],S):-
    vessel(V,_,TDep,_,_,_),TPossibleDep is TEndLoad+1,
    ( (TPossibleDep>TDep,!,SV is TPossibleDep-TDep);SV is 0),
    sum_delays_optimal(LV,SLV),
    S is SV+SLV.

% Obtain the sequence with the shortest delay
obtain_seq_shortest_delay(SeqBetterTriplets, SShortestDelay):-
    (obtain_seq_shortest_delay1;true),retract(shortest_delay(SeqBetterTriplets, SShortestDelay)),!.

obtain_seq_shortest_delay1:-
    asserta(shortest_delay(_,100000)),
    findall(V,vessel(V,_,_,_,_,_),LV),
    permutation(LV,SeqV),
    sequence_temporization(SeqV,SeqTriplets),
    sum_delays_optimal(SeqTriplets,S),
    compare_shortest_delay(SeqTriplets,S),
    fail.

compare_shortest_delay(SeqTriplets,S):-
 shortest_delay(_,SLower),
    ((S<SLower,!,retract(shortest_delay(_,_)),asserta(shortest_delay(SeqTriplets,S)));true).