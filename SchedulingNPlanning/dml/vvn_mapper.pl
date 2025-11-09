% Extract scheduling data from a JSON output into a vessel tuple
json_to_vvn_fact(JsonDict, vessel(VesselName, ArrivalTime, DepartureTime, UnloadingTime, LoadingTime)) :-
    VesselName = JsonDict.vessel.name,

    % Convert datetime to hours
    parse_time(JsonDict.expectedArrival, ArrivalTimestamp),
    parse_time(JsonDict.expectedDeparture, DepartureTimestamp),
    ArrivalTime is round(ArrivalTimestamp / 3600),
    DepartureTime is round(DepartureTimestamp / 3600),

    % TODO we don't have this data yet
    % UnloadingTime is 0,
    % LoadingTime is 0.
    random_between(5, 15, UnloadingTime),
    random_between(5, 15, LoadingTime).
