:- dynamic vessel_counter/1.

% Initialize the counter if not already
init_vessel_counter :-
    (   vessel_counter(_)
    ->  true
    ;   assertz(vessel_counter(1))
    ).

% Increment the counter and return the new value
next_vessel_id(Id) :-
    retract(vessel_counter(Current)),
    Id = Current,
    Next is Current + 1,
    assertz(vessel_counter(Next)).

% Convert JSON dict to vessel tuple with unique name
json_to_vvn_fact(JsonDict, vessel(VesselName, ArrivalTime, DepartureTime, UnloadingTime, LoadingTime, Crane)) :-
    init_vessel_counter,
    next_vessel_id(Counter),
    % format(string(VesselName), "~w_~d", [JsonDict.vessel.name, Counter]),
    VesselName = JsonDict.VvnId,

    ArrivalTime = JsonDict.eta,
    DepartureTime = JsonDict.etd,

    Crane = [],

    UnloadingTime = JsonDict.unloadingCount,
    LoadingTime = JsonDict.loadingCount.
