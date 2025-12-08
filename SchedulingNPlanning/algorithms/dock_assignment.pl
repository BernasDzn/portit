:- use_module(library(lists)).

% Dock rebalancing algorithm - balance vessel load across docks by arrival time
rebalance_docks(JsonData, SortBy, Rebalanced, Metrics) :-
    ( get_dict(vesselTaskFacts, JsonData, VesselTaskFacts) -> true ; VesselTaskFacts = [] ),
    extract_vessels_for_rebalancing(VesselTaskFacts, Vessels),
    extract_docks_with_cranes(JsonData, Docks),
    
    length(Vessels, VesselCount),
    length(Docks, DockCount),
    
    % Validate data
    ( VesselCount = 0 ->
        throw(error(no_vessels, 'No vessels available for rebalancing on the selected date'))
    ; DockCount = 0 ->
        throw(error(no_docks, 'No docks with cranes available for rebalancing'))
    ;
        % Sort by arrival and assign
        get_time(StartTime),
        predsort(compare_by_arrival, Vessels, SortedVessels),
        initialize_dock_loads(Docks, InitialLoads),
        assign_vessels_to_docks(SortedVessels, InitialLoads, FinalLoads, Assignments),
        get_time(EndTime),
        ComputationTime is EndTime - StartTime,
        
        % Calculate metrics and format result
        calculate_metrics(Assignments, FinalLoads, SortBy, ComputationTime, Metrics),
        format_result(Assignments, FinalLoads, Rebalanced)
    ).

% Extract vessel data
extract_vessels_for_rebalancing([], []).
extract_vessels_for_rebalancing([JsonVessel|Rest], [Vessel|RestVessels]) :-
    get_dict(vessel, JsonVessel, VesselData), get_dict(name, VesselData, Name),
    get_dict(dock, JsonVessel, Dock), get_dict(eta, JsonVessel, Arrival),
    get_dict(unloadingCount, JsonVessel, Unload), get_dict(loadingCount, JsonVessel, Load),
    Vessel = vessel{name: Name, dock: Dock, arrival: Arrival, unload: Unload, load: Load}, !,
    extract_vessels_for_rebalancing(Rest, RestVessels).
extract_vessels_for_rebalancing([_|Rest], RestVessels) :-
    extract_vessels_for_rebalancing(Rest, RestVessels).

% Extract docks with cranes
extract_docks_with_cranes(JsonData, Docks) :-
    ( get_dict(docks, JsonData, DockList) -> true ; DockList = [] ),
    ( get_dict(craneWorkloads, JsonData, CraneList) -> true ; CraneList = [] ),
    extract_docks_with_crane_counts(DockList, CraneList, Docks).

extract_docks_with_crane_counts([], _, []).
extract_docks_with_crane_counts([JsonDock|Rest], CraneList, Docks) :-
    get_dict(code, JsonDock, Code),
    findall(_, (member(Crane, CraneList), get_dict(dock, Crane, Code)), Cranes),
    length(Cranes, Count),
    ( Count > 0 ->
        Docks = [dock{code: Code, cranes: Count}|RestDocks]
    ;
        Docks = RestDocks
    ),
    extract_docks_with_crane_counts(Rest, CraneList, RestDocks).

% Compare vessels by arrival time
compare_by_arrival(Order, V1, V2) :- compare(Order, V1.arrival, V2.arrival).

% Initialize dock loads to zero
initialize_dock_loads([], []).
initialize_dock_loads([Dock|Rest], [load{dock: Dock.code, cranes: Dock.cranes, total_load: 0, vessels: []}|RestLoads]) :-
    initialize_dock_loads(Rest, RestLoads).

% Assign vessels to least-loaded dock
assign_vessels_to_docks([], Final, Final, []).
assign_vessels_to_docks([Vessel|RestVessels], Current, Final, [Assignment|RestAssignments]) :-
    find_min_load_dock(Current, MinDock),
    VesselLoad is (Vessel.unload + Vessel.load) / MinDock.cranes,
    NewLoad is MinDock.total_load + VesselLoad,
    Assignment = assignment{vessel: Vessel.name, current_dock: Vessel.dock, proposed_dock: MinDock.dock, load_contribution: VesselLoad},
    update_dock_load(Current, MinDock.dock, Vessel.name, NewLoad, Updated),
    assign_vessels_to_docks(RestVessels, Updated, Final, RestAssignments).

find_min_load_dock([Load], Load) :- !.
find_min_load_dock([Load|Rest], Min) :-
    find_min_load_dock(Rest, RestMin),
    ( Load.total_load < RestMin.total_load -> Min = Load ; Min = RestMin ).

update_dock_load([], _, _, _, []).
update_dock_load([Load|Rest], Code, VesselName, NewLoad, [Updated|RestUpdated]) :-
    ( Load.dock = Code ->
        append(Load.vessels, [VesselName], NewVessels),
        Updated = load{dock: Load.dock, cranes: Load.cranes, total_load: NewLoad, vessels: NewVessels}
    ;
        Updated = Load
    ),
    update_dock_load(Rest, Code, VesselName, NewLoad, RestUpdated).

% Calculate metrics
calculate_metrics(Assignments, FinalLoads, SortBy, Time, Metrics) :-
    count_reassignments(Assignments, Reassignments),
    extract_loads(FinalLoads, Loads),
    length(Loads, NumDocks),
    length(Assignments, VesselCount),
    sum_list(Loads, Total),
    AvgLoad is Total / NumDocks,
    max_list(Loads, MaxLoad),
    min_list(Loads, MinLoad),
    LoadRange is MaxLoad - MinLoad,
    calculate_std_dev(Loads, AvgLoad, StdDev),
    Metrics = #{sortBy: SortBy, vesselCount: VesselCount, dockCount: NumDocks,
                reassignments: Reassignments, avgLoad: AvgLoad, maxLoad: MaxLoad,
                minLoad: MinLoad, loadRange: LoadRange, stdDev: StdDev, computationTime: Time}.

count_reassignments([], 0).
count_reassignments([Assignment|Rest], Count) :-
    count_reassignments(Rest, RestCount),
    ( Assignment.current_dock \= Assignment.proposed_dock -> Count is RestCount + 1 ; Count = RestCount ).

extract_loads([], []).
extract_loads([Load|Rest], [Load.total_load|RestLoads]) :- extract_loads(Rest, RestLoads).

calculate_std_dev(Values, Mean, StdDev) :-
    maplist(squared_diff(Mean), Values, Diffs),
    sum_list(Diffs, Sum), length(Values, N),
    Variance is Sum / N, StdDev is sqrt(Variance).

squared_diff(Mean, Value, Diff) :- Diff is (Value - Mean) ** 2.

% Format result
format_result(Assignments, FinalLoads, Result) :-
    format_assignments(Assignments, FormattedAssignments),
    format_dock_loads(FinalLoads, FormattedLoads),
    Result = #{assignments: FormattedAssignments, dockLoads: FormattedLoads}.

format_assignments([], []).
format_assignments([Assignment|Rest], [Dict|RestFormatted]) :-
    ( Assignment.current_dock \= Assignment.proposed_dock -> RequiresReassignment = true ; RequiresReassignment = false ),
    Dict = #{vessel: Assignment.vessel, currentDock: Assignment.current_dock, proposedDock: Assignment.proposed_dock,
          loadContribution: Assignment.load_contribution, requiresReassignment: RequiresReassignment},
    format_assignments(Rest, RestFormatted).

format_dock_loads([], []).
format_dock_loads([Load|Rest], [Dict|RestFormatted]) :-
    length(Load.vessels, VesselCount),
    Dict = #{dock: Load.dock, cranes: Load.cranes, totalLoad: Load.total_load, vesselCount: VesselCount, vessels: Load.vessels},
    format_dock_loads(Rest, RestFormatted). 