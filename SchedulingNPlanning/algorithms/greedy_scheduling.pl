% Greedy Scheduling Algorithm
% This algorithm prioritizes computational efficiency over optimality
% Strategy: Schedule vessels in order of earliest departure time (EDD - Earliest Due Date)

:- dynamic vessel/6.

% ============================================================================
% MAIN ENTRY POINT: GREEDY SCHEDULING ALGORITHM (Earliest Due Date)
% ============================================================================
% This predicate schedules vessels using a greedy approach
% Input:  (no input parameters - reads from vessel/6 facts in knowledge base)
% Output: SeqTriplets = list of (VesselName, StartTime, EndTime) tuples
%         STotalDelay = total delay in hours across all vessels
%         ComputationTime = how long the algorithm took to run (in seconds)
obtain_seq_greedy(SeqTriplets, STotalDelay, ComputationTime) :-
    % Record the start time (for performance measurement)
    statistics(cputime, StartTime),
    
    % Step 1: Get all vessel names from the knowledge base
    % findall collects all vessels V where vessel(V,...) is true
    % Result: LV = [zeus, poseidon, marenostrum, nautilus, floating]
    findall(V, vessel(V,_,_,_,_,_), LV),
    
    % Step 2: Sort vessels by their departure deadline (earliest first)
    % This is the GREEDY HEURISTIC: prioritize urgent vessels
    sort_vessels_by_departure(LV, SortedVessels),
    
    % Step 3: Generate the actual schedule by processing vessels in sorted order
    % This respects the dock constraint (only one vessel at a time)
    sequence_temporization_greedy(SortedVessels, SeqTriplets),
    
    % Step 4: Calculate how much delay we accumulated
    % Delay = how many hours late each vessel is compared to its desired departure
    sum_delays(SeqTriplets, STotalDelay),
    
    % Record the end time and calculate duration
    statistics(cputime, EndTime),
    ComputationTime is EndTime - StartTime.

% ============================================================================
% SORTING VESSELS BY DEPARTURE TIME (GREEDY HEURISTIC)
% ============================================================================
% This predicate sorts vessels by their departure deadline
% Vessels that need to leave sooner are scheduled first
% Input:  Vessels = [zeus, poseidon, marenostrum, ...]
% Output: SortedVessels = [nautilus, marenostrum, poseidon, ...] (sorted by deadline)
sort_vessels_by_departure(Vessels, SortedVessels) :-
    % Step 1: Create (DepartureTime, VesselName) pairs
    % Example: zeus with departure=63 becomes (63, zeus)
    map_vessels_with_departure(Vessels, VesselPairs),
    
    % Step 2: Sort pairs by the first element (departure time)
    % Prolog's sort/2 automatically sorts by first element of tuples
    % Example: [(63,zeus), (30,nautilus)] becomes [(30,nautilus), (63,zeus)]
    sort(VesselPairs, SortedPairs),
    
    % Step 3: Extract just the vessel names, discard the times
    % Example: [(30,nautilus), (63,zeus)] becomes [nautilus, zeus]
    extract_vessel_names(SortedPairs, SortedVessels).

% ============================================================================
% HELPER: Map vessel names to (DepartureTime, Name) pairs
% ============================================================================
% Base case: empty list produces empty list
map_vessels_with_departure([], []).

% Recursive case: process one vessel at a time
% Input:  [zeus | RestVessels]
% Output: [(63, zeus) | RestPairs]
map_vessels_with_departure([V|Rest], [(TDep, V)|RestPairs]) :-
    % Look up this vessel's departure time in the knowledge base
    % vessel(Name, Arrival, Departure, Unload, Load, Crane)
    %         V      _      TDep      _      _     _
    vessel(V, _, TDep, _, _, _),
    
    % Recursively process the remaining vessels
    map_vessels_with_departure(Rest, RestPairs).

% ============================================================================
% HELPER: Extract vessel names from (Time, Name) pairs
% ============================================================================
% Base case: empty list
extract_vessel_names([], []).

% Recursive case: take the name, ignore the time
% Input:  [(30, nautilus) | RestPairs]
% Output: [nautilus | RestNames]
extract_vessel_names([(_, V)|Rest], [V|RestNames]) :-
    % The _ discards the time, V captures the name
    % Recursively extract names from remaining pairs
    extract_vessel_names(Rest, RestNames).

% ============================================================================
% SCHEDULE GENERATION: Convert vessel list to timed schedule
% ============================================================================
% This predicate takes a sorted list of vessels and generates actual start/end times
% Input:  LV = [nautilus, marenostrum, poseidon, ...] (ordered list)
% Output: SeqTriplets = [(nautilus,10,17), (marenostrum,18,34), ...]
sequence_temporization_greedy(LV, SeqTriplets) :-
    % Start with EndPrevSeq = 0 (dock is free from time 0)
    sequence_temporization_greedy1(0, LV, SeqTriplets).

% Recursive case: schedule one vessel at a time
% EndPrevSeq = time when the dock becomes free (previous vessel finished)
% [V|LV] = current vessel V and remaining vessels LV
% [(V, TInUnload, TEndLoad)|SeqTriplets] = output schedule with this vessel's times added
sequence_temporization_greedy1(EndPrevSeq, [V|LV], [(V, TInUnload, TEndLoad)|SeqTriplets]) :-
    % Look up this vessel's data from the knowledge base
    % vessel(Name, ArrivalTime, DepartureDeadline, UnloadTime, LoadTime, Crane)
    vessel(V, TIn, _, TUnload, TLoad, _),
    
    % Decide when this vessel can start being serviced:
    % CASE 1: If the vessel hasn't arrived yet (TIn > EndPrevSeq)
    %         → it starts when it arrives (TInUnload = TIn)
    %         Example: Dock free at hour 5, vessel arrives at hour 10 → starts at 10
    % 
    % CASE 2: If the vessel is already waiting (TIn <= EndPrevSeq)
    %         → it starts right after previous vessel finishes (TInUnload = EndPrevSeq + 1)
    %         Example: Dock free at hour 18, vessel arrived at hour 8 → starts at 19 (waited!)
    (TIn > EndPrevSeq -> TInUnload is TIn ; TInUnload is EndPrevSeq + 1),
    
    % Calculate when this vessel finishes all operations:
    % Finish time = Start + Unloading + Loading - 1
    % We subtract 1 because hours are 0-indexed
    % Example: Start at 10, unload 0h, load 8h → End at 10+0+8-1 = 17
    %          (Operations happen during hours 10,11,12,13,14,15,16,17)
    TEndLoad is TInUnload + TUnload + TLoad - 1,
    
    % Recursively schedule the rest of the vessels
    % Pass TEndLoad as the new "dock becomes free" time for the next vessel
    sequence_temporization_greedy1(TEndLoad, LV, SeqTriplets).

% Base case: no more vessels to schedule
% When the vessel list is empty, we're done - output empty schedule
% Input:  EndPrevSeq = doesn't matter, VesselList = []
% Output: SeqTriplets = [] (empty schedule)
sequence_temporization_greedy1(_, [], []).

% ============================================================================
% DELAY CALCULATION: How late is each vessel?
% ============================================================================
% This calculates the total delay across all vessels
% Delay = how many hours past their deadline each vessel departs

% Base case: no vessels means no delay
sum_delays([], 0).

% Recursive case: calculate delay for one vessel, add to rest
% Input:  [(nautilus, 10, 17) | RestVessels]
% Output: TotalDelay across all vessels
sum_delays([(V, _, TEndLoad)|LV], S) :-
    % Look up when this vessel wanted to depart from the knowledge base
    % vessel(Name, _, DesiredDeparture, _, _, _)
    vessel(V, _, TDep, _, _, _),
    
    % Calculate when this vessel can actually depart
    % Operations finish at TEndLoad, so vessel can leave at TEndLoad + 1
    % We do TEndLoad + 1 because departure happens the hour after operations complete
    % Example: Finishes at hour 17 → can depart at hour 18
    TPossibleDep is TEndLoad + 1,
    
    % Calculate delay for THIS vessel:
    % IF actual departure time > desired departure time
    %    THEN Delay = ActualDeparture - DesiredDeparture
    %    ELSE Delay = 0 (vessel left on time or early!)
    % 
    % Example 1: Finishes at 76 (departs 77), wanted to leave at 63
    %            → 77 > 63, so Delay = 77-63 = 14 hours LATE
    % Example 2: Finishes at 17 (departs 18), wanted to leave at 30
    %            → 18 <= 30, so Delay = 0 hours (LEFT EARLY!)
    (TPossibleDep > TDep -> SV is TPossibleDep - TDep ; SV is 0),
    
    % Recursively calculate delays for all remaining vessels
    sum_delays(LV, SLV),
    
    % Total delay = this vessel's delay + sum of all remaining vessels' delays
    % Example: zeus delayed 14h + rest delayed 20h = 34h total
    S is SV + SLV.