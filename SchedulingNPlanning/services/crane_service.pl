get_all_crane_facts(CraneFacts):-
    findall(crane(Name, Speed), crane(Name, Speed), CraneFacts).

get_crane_sum([], 0).
get_crane_sum([crane(_, Speed) | RestCranes], Sum):-
    get_crane_sum(RestCranes, Sum1),
    Sum is (Sum1 + Speed).

calculate_load_unload_time(LoadCount, UnloadCount, CraneList, LoadTime, UnloadTime):-
    get_crane_sum(CraneList, Sum),

    ( LoadCount > 0 -> LoadTime = (Sum / LoadCount) ; LoadTime = 0 ),
    ( UnloadCount > 0 -> UnloadTime = (Sum / UnloadCount) ; UnloadTime = 0 ).