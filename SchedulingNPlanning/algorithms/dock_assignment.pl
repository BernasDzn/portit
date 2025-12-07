:- use_module(library(lists)).

% Main predicate: rebalance vessels across docks by arrival time
% rebalance_docks(+JsonData, +SortBy, -Rebalanced, -Metrics)
rebalance_docks(JsonData, SortBy, Rebalanced, Metrics) :-
    % Extract vessels and docks with crane counts
    format(user_error, 'Step 1: Extracting vessel task facts...~n', []),
    ( get_dict(vesselTaskFacts, JsonData, VesselTaskFacts) ->
        format(user_error, 'Step 1: Found vessel task facts~n', [])
    ;
        VesselTaskFacts = [],
        format(user_error, 'WARNING: No vesselTaskFacts found in JSON~n', [])
    ),
    format(user_error, 'Step 2: Extracting vessels for rebalancing...~n', []),
    extract_vessels_for_rebalancing(VesselTaskFacts, Vessels),
    format(user_error, 'Step 3: Extracting docks with cranes...~n', []),
    extract_docks_with_cranes(JsonData, Docks),
    format(user_error, 'Extraction complete~n', []),
    
    length(Vessels, VesselCount),
    length(Docks, DockCount),
    format(user_error, '~nRebalancing ~w vessels across ~w docks~n', [VesselCount, DockCount]),
    format(user_error, 'Sort criterion: ~w~n', [SortBy]),
    
    % Handle empty data case
    ( VesselCount = 0 ->
        format(user_error, 'ERROR: No vessels to rebalance~n', []),
        Rebalanced = #{assignments: [], dockLoads: []},
        Metrics = #{
            sortBy: SortBy, vesselCount: 0, dockCount: DockCount,
            reassignments: 0, avgLoad: 0, maxLoad: 0, minLoad: 0,
            loadRange: 0, stdDev: 0, computationTime: 0
        }
    ; DockCount = 0 ->
        format(user_error, 'ERROR: No docks available~n', []),
        Rebalanced = #{assignments: [], dockLoads: []},
        Metrics = #{
            sortBy: SortBy, vesselCount: VesselCount, dockCount: 0,
            reassignments: 0, avgLoad: 0, maxLoad: 0, minLoad: 0,
            loadRange: 0, stdDev: 0, computationTime: 0
        }
    ;
        % Continue with normal processing
    
    % Sort vessels based on criterion
    format(user_error, 'Sorting ~w vessels by ~w~n', [VesselCount, SortBy]),
    sort_vessels(Vessels, SortBy, SortedVessels),
    format(user_error, 'Sorting complete~n', []),
    
    % Initialize dock loads (all start at 0)
    format(user_error, 'Initializing dock loads for ~w docks~n', [DockCount]),
    initialize_dock_loads(Docks, InitialLoads),
    format(user_error, 'Dock loads initialized~n', []),
    
    % Assign vessels to least loaded dock iteratively
    format(user_error, 'Starting vessel assignment~n', []),
    get_time(StartTime),
    assign_vessels_to_docks(SortedVessels, InitialLoads, FinalLoads, Assignments),
    get_time(EndTime),
    ComputationTime is EndTime - StartTime,
    format(user_error, 'Vessel assignment complete~n', []),
    
    % Calculate metrics
    calculate_rebalancing_metrics(InitialLoads, FinalLoads, Assignments, SortBy, ComputationTime, Metrics),
    
        % Format result
        format_rebalancing_result(Assignments, FinalLoads, Rebalanced),
        
        format(user_error, 'Rebalancing complete. Total computation time: ~3f seconds~n', [ComputationTime])
    ).

% Extract vessels with load/unload times
extract_vessels_for_rebalancing([], []).
extract_vessels_for_rebalancing([JsonVessel|Rest], [Vessel|RestVessels]) :-
    get_dict(vessel, JsonVessel, VesselData),
    get_dict(name, VesselData, Name),
    get_dict(dock, JsonVessel, Dock),
    get_dict(eta, JsonVessel, Arrival),
    get_dict(etd, JsonVessel, Departure),
    get_dict(unloadingCount, JsonVessel, Unload),
    get_dict(loadingCount, JsonVessel, Load),
    Vessel = vessel{
        name: Name,
        dock: Dock,
        arrival: Arrival,
        departure: Departure,
        unload: Unload,
        load: Load
    },
    !,  % Prevent backtracking to the fallback clause once extraction succeeds
    extract_vessels_for_rebalancing(Rest, RestVessels).
extract_vessels_for_rebalancing([JsonVessel|Rest], RestVessels) :-
    % Skip vessels that don't match the expected structure
    format(user_error, 'WARNING: Skipping malformed vessel: ~w~n', [JsonVessel]),
    extract_vessels_for_rebalancing(Rest, RestVessels).

% Extract docks with crane counts from JSON data
extract_docks_with_cranes(JsonData, Docks) :-
    format(user_error, '  - Getting docks list...~n', []),
    ( get_dict(docks, JsonData, DockList) ->
        format(user_error, '  - Found ~w docks~n', [DockList])
    ;
        format(user_error, '  - ERROR: No docks found in JSON~n', []),
        DockList = []
    ),
    format(user_error, '  - Getting crane workloads...~n', []),
    ( get_dict(craneWorkloads, JsonData, CraneList) ->
        format(user_error, '  - Found crane workloads~n', [])
    ;
        format(user_error, '  - ERROR: No craneWorkloads found in JSON~n', []),
        CraneList = []
    ),
    format(user_error, '  - Extracting dock crane counts...~n', []),
    extract_docks_with_crane_counts(DockList, CraneList, Docks),
    format(user_error, '  - Dock extraction complete~n', []).

extract_docks_with_crane_counts([], _, []) :- 
    format(user_error, '    - All docks processed~n', []).
extract_docks_with_crane_counts([JsonDock|Rest], CraneList, Docks) :-
    format(user_error, '    - Processing dock: ~w~n', [JsonDock]),
    get_dict(code, JsonDock, DockCode),
    format(user_error, '    - Dock code: ~w~n', [DockCode]),
    count_cranes_for_dock(DockCode, CraneList, CraneCount),
    format(user_error, '    - Crane count: ~w~n', [CraneCount]),
    ( CraneCount > 0 ->
        % Only include docks with cranes
        Dock = dock{
            code: DockCode,
            cranes: CraneCount
        },
        Docks = [Dock|RestDocks],
        format(user_error, '    - Dock ~w included (has cranes)~n', [DockCode])
    ;
        % Skip docks without cranes
        Docks = RestDocks,
        format(user_error, '    - Dock ~w skipped (no cranes)~n', [DockCode])
    ),
    extract_docks_with_crane_counts(Rest, CraneList, RestDocks).

count_cranes_for_dock(DockCode, CraneList, Count) :-
    format(user_error, '      - Counting cranes for dock ~w~n', [DockCode]),
    findall(_, (member(Crane, CraneList), get_dict(dock, Crane, DockCode)), Cranes),
    length(Cranes, Count),
    format(user_error, '      - Found ~w cranes~n', [Count]).

% Sort vessels by arrival time only
sort_vessels(Vessels, _, Sorted) :-
    predsort(compare_by_arrival, Vessels, Sorted).

% Comparison predicate for sorting by arrival
compare_by_arrival(Order, V1, V2) :-
    compare(Order, V1.arrival, V2.arrival).

% Initialize dock loads
initialize_dock_loads([], []).
initialize_dock_loads([Dock|Rest], [Load|RestLoads]) :-
    Load = load{
        dock: Dock.code,
        cranes: Dock.cranes,
        total_load: 0,
        vessels: []
    },
    initialize_dock_loads(Rest, RestLoads).

% Assign vessels to docks iteratively (each to least loaded)
assign_vessels_to_docks([], FinalLoads, FinalLoads, []).
assign_vessels_to_docks([Vessel|RestVessels], CurrentLoads, FinalLoads, [Assignment|RestAssignments]) :-
    % Find dock with minimum load
    find_min_load_dock(CurrentLoads, MinDock, MinLoad),
    
    % Calculate load this vessel adds to the dock
    VesselLoad is (Vessel.unload + Vessel.load) / MinDock.cranes,
    NewLoad is MinLoad.total_load + VesselLoad,
    
    % Create assignment
    Assignment = assignment{
        vessel: Vessel.name,
        current_dock: Vessel.dock,
        proposed_dock: MinDock.dock,
        load_contribution: VesselLoad
    },
    
    format(user_error, 'Assigning ~w: ~w -> ~w (load: ~3f)~n', 
           [Vessel.name, Vessel.dock, MinDock.dock, VesselLoad]),
    
    % Update dock loads
    update_dock_load(CurrentLoads, MinDock.dock, Vessel.name, NewLoad, UpdatedLoads),
    
    % Process remaining vessels
    assign_vessels_to_docks(RestVessels, UpdatedLoads, FinalLoads, RestAssignments).

% Find dock with minimum load
find_min_load_dock([Load], Load, Load) :- !.
find_min_load_dock([Load|Rest], MinDock, MinLoad) :-
    find_min_load_dock(Rest, RestMin, RestMinLoad),
    ( Load.total_load < RestMinLoad.total_load ->
        MinDock = Load, MinLoad = Load
    ;
        MinDock = RestMin, MinLoad = RestMinLoad
    ).

% Update dock load
update_dock_load([], _, _, _, []).
update_dock_load([Load|Rest], DockCode, VesselName, NewLoad, [Updated|RestUpdated]) :-
    ( Load.dock = DockCode ->
        append(Load.vessels, [VesselName], NewVessels),
        Updated = load{
            dock: Load.dock,
            cranes: Load.cranes,
            total_load: NewLoad,
            vessels: NewVessels
        }
    ;
        Updated = Load
    ),
    update_dock_load(Rest, DockCode, VesselName, NewLoad, RestUpdated).

% Calculate metrics
calculate_rebalancing_metrics(InitialLoads, FinalLoads, Assignments, SortBy, Time, Metrics) :-
    % Count reassignments
    count_reassignments(Assignments, ReassignmentCount),
    
    % Calculate load statistics
    extract_loads(FinalLoads, Loads),
    length(Loads, NumDocks),
    sum_list(Loads, TotalLoad),
    AvgLoad is TotalLoad / NumDocks,
    max_list(Loads, MaxLoad),
    min_list(Loads, MinLoad),
    LoadRange is MaxLoad - MinLoad,
    
    % Calculate standard deviation (measure of balance)
    calculate_std_dev(Loads, AvgLoad, StdDev),
    
    % Count vessels
    length(Assignments, VesselCount),
    
    Metrics = #{
        sortBy: SortBy,
        vesselCount: VesselCount,
        dockCount: NumDocks,
        reassignments: ReassignmentCount,
        avgLoad: AvgLoad,
        maxLoad: MaxLoad,
        minLoad: MinLoad,
        loadRange: LoadRange,
        stdDev: StdDev,
        computationTime: Time
    },
    
    format(user_error, '~nMetrics:~n', []),
    format(user_error, '  Vessels: ~w~n', [VesselCount]),
    format(user_error, '  Docks: ~w~n', [NumDocks]),
    format(user_error, '  Reassignments: ~w~n', [ReassignmentCount]),
    format(user_error, '  Avg Load: ~3f~n', [AvgLoad]),
    format(user_error, '  Load Range: ~3f (min: ~3f, max: ~3f)~n', [LoadRange, MinLoad, MaxLoad]),
    format(user_error, '  Std Dev: ~3f~n', [StdDev]).

count_reassignments([], 0).
count_reassignments([Assignment|Rest], Count) :-
    count_reassignments(Rest, RestCount),
    ( Assignment.current_dock \= Assignment.proposed_dock ->
        Count is RestCount + 1
    ;
        Count = RestCount
    ).

extract_loads([], []).
extract_loads([Load|Rest], [Load.total_load|RestLoads]) :-
    extract_loads(Rest, RestLoads).

calculate_std_dev(Values, Mean, StdDev) :-
    maplist(squared_diff(Mean), Values, SquaredDiffs),
    sum_list(SquaredDiffs, SumSquares),
    length(Values, N),
    Variance is SumSquares / N,
    StdDev is sqrt(Variance).

squared_diff(Mean, Value, Diff) :-
    Diff is (Value - Mean) ** 2.

% Format result
format_rebalancing_result(Assignments, FinalLoads, Result) :-
    format_assignments(Assignments, FormattedAssignments),
    format_dock_loads(FinalLoads, FormattedLoads),
    Result = #{
        assignments: FormattedAssignments,
        dockLoads: FormattedLoads
    }.

format_assignments([], []).
format_assignments([Assignment|Rest], [Dict|RestFormatted]) :-
    % Evaluate the boolean expression for requiresReassignment
    ( Assignment.current_dock \= Assignment.proposed_dock ->
        RequiresReassignment = true
    ;
        RequiresReassignment = false
    ),
    Dict = #{
        vessel: Assignment.vessel,
        currentDock: Assignment.current_dock,
        proposedDock: Assignment.proposed_dock,
        loadContribution: Assignment.load_contribution,
        requiresReassignment: RequiresReassignment
    },
    format_assignments(Rest, RestFormatted).

format_dock_loads([], []).
format_dock_loads([Load|Rest], [Dict|RestFormatted]) :-
    length(Load.vessels, VCount),
    Dict = #{
        dock: Load.dock,
        cranes: Load.cranes,
        totalLoad: Load.total_load,
        vesselCount: VCount,
        vessels: Load.vessels
    },
    format_dock_loads(Rest, RestFormatted). 