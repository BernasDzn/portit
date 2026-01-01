# US4109 - As a Logistics Operator, I want to update an in progress VVE with executed operations, so that the system reflects real execution progress and performance.

### Process View
- SPA fetches the planned operations for a VVE and shows editable fields such as actual start/end times and resource usage.
- Operators confirm or adjust the values and submit them in batches.
- OEM updates both the execution log and the linked Operation Plan, advancing the planned statuses to started/completed/delayed accordingly.

### REST Endpoints
- `PATCH /vves/{vveId}/operations`: accepts an array of executed operation records, enforces timestamps, and persists them with operator metadata.
- `GET /vves/{vveId}/operations`: returns both planned and executed operations with their current statuses to allow the SPA to display differences.

### Data Synchronization
- Each execution record contains `planOperationId`, `actualStart`, `actualEnd`, `resourcesUsed`, and `operatorId`.
- The API infers delays by comparing actual times against plan windows and updates the Operation Plan status (e.g., set to `DELAYED`).
