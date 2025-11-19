time_string_minutes(String, Hours) :-
    split_string(String, ":", "", [HStr, MStr, _]),
    number_string(H, HStr),
    number_string(M, MStr),
    Hours is H + M / 60.

% Extract scheduling data from a JSON output into a vessel tuple
json_to_interval_fact(JsonDict, interval(Day, StartTime, EndTime)) :-
    Day = JsonDict.day,
    time_string_minutes(JsonDict.startTime, StartTime),
    time_string_minutes(JsonDict.endTime, EndTime).

json_to_crane_fact(JsonDict, crane(Name, Speed)) :-
    Name = JsonDict.crane,
    Speed = JsonDict.speed.