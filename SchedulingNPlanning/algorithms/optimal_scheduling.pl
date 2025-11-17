% First support for IARTI project 2025/2026
% Scheduling Vessels Unload/Load

:-dynamic shortest_delay/2.
:- dynamic vessel/5.
:- dynamic interval/3.

interval(0, 0, 24).
interval(1, 0, 24).
interval(2, 0, 24).
interval(3, 0, 24).
interval(4, 0, 24).
interval(5, 0, 24).
interval(6, 0, 24).

vessel(zeus, 6, 63, 10, 16).
vessel(poseidon, 23, 50, 9, 7).
vessel(marenostrum, 8, 40, 5, 12).
vessel(nautilus, 10, 30, 0, 8).
vessel(floating, 36, 70, 12, 0).

% Sequence temporization
sequence_temporization(LV,SeqTriplets):-
	sequence_temporization1(0,LV,SeqTriplets).

sequence_temporization1(EndPrevSeq,[V|LV],[(V,TInUnload,TEndLoad)|SeqTriplets]):-
    vessel(V,TIn,_,TUnload,TLoad),
    ( (TIn> EndPrevSeq,!, TInUnload is TIn); TInUnload is EndPrevSeq+1),
    
    TEndLoad is TInUnload + TUnload+TLoad -1,
    sequence_temporization1(TEndLoad,LV,SeqTriplets).

sequence_temporization1(_,[],[]).

% Find the sum of delays
sum_delays([],0).

sum_delays([(V,_,TEndLoad)|LV],S):-
    vessel(V,_,TDep,_,_),TPossibleDep is TEndLoad+1,
    ( (TPossibleDep>TDep,!,SV is TPossibleDep-TDep);SV is 0),
    sum_delays(LV,SLV),
    S is SV+SLV.

% Obtain the sequence with the shortest delay
obtain_seq_shortest_delay(SeqBetterTriplets, SShortestDelay):-
    (obtain_seq_shortest_delay1;true),retract(shortest_delay(SeqBetterTriplets, SShortestDelay)),!.

obtain_seq_shortest_delay1:-
    asserta(shortest_delay(_,100000)),
    findall(V,vessel(V,_,_,_,_),LV),
    permutation(LV,SeqV),
    sequence_temporization(SeqV,SeqTriplets),
    sum_delays(SeqTriplets,S),
    compare_shortest_delay(SeqTriplets,S),
    fail.

compare_shortest_delay(SeqTriplets,S):-
 shortest_delay(_,SLower),
    ((S<SLower,!,retract(shortest_delay(_,_)),asserta(shortest_delay(SeqTriplets,S)));true).

allowed_interval(Day, Start, End) :-
    interval(Day, IStart, IEnd),
    Start >= IStart,
    End   =< IEnd.