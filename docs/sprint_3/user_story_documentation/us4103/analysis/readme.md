# US4103 - As a Logistics Operator, I want to search and list Operation Plans for a given day or period, so that I can quickly review all scheduled activities within that timeframe.

### Domain Concepts
- **Operation Plan Summary**: includes vessel, dock, start and end time, assigned resources, and status metadata used by the SPA table.
- **Query Scope**: date ranges, vessel identifiers, and optional status filters define the subset of plans returned.
- **Sort Fields**: start time, vessel name, and expected delay are important for prioritization during reviews.

### Business Rules
1. OEM exposes a query API that validates date ranges and optionally filters by vessel.
2. When too many plans match, results are paginated and sorted according to SPA preferences.
3. Sorting is supported on at least the most-critical columns so operators can quickly spot delays or urgent vessels.
4. SPA keeps table responsive by fetching only requested page/filters at a time.
