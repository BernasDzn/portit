% Extract scheduling data from a JSON output into a vessel tuple
json_to_vvn_fact(JsonDict, ActualCrane, vessel(VesselName, ArrivalTime, DepartureTime, UnloadingTime, LoadingTime, Crane)) :-
    VesselName = JsonDict.vessel.name,

    ArrivalTime = JsonDict.eta,
    DepartureTime = JsonDict.etd,

    UnloadingTime = JsonDict.unloadingTime,
    LoadingTime = JsonDict.loadingTime,
    Crane = ActualCrane.
