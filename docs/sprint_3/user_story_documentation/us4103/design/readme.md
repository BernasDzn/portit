# US4103 - As a Logistics Operator, I want to search and list Operation Plans for a given day or period, so that I can quickly review all scheduled activities within that timeframe.

### Process View
- SPA sends filter/sort parameters to OEM to fetch a page of Operation Plan summaries.
- OEM validates inputs, applies filters to the Operation Plan repository, and returns paged results together with total counts.
- Sort and search preferences are preserved in query strings so the SPA can reconstruct the last view.

### REST Endpoints
- `GET /operation-plans`: supports `from`, `to`, `vesselId`, and sorting instructions (e.g., `sort=startTime,asc`).
- `GET /operation-plans/{id}`: returns the full plan when the operator drills down.

### SPA Behavior
- Table columns include vessel, dock, start/end time, assigned resources, status, and expected delay.
- Search box filters across vessel names and resource fields, while column filters handle date ranges or dock selections.
- Sorting is available via column headers, triggering requests with the desired `sort` parameter.
