# US4109 - As a Logistics Operator, I want to update an in progress VVE with executed operations, so that the system reflects real execution progress and performance.

### Domain Concepts
- **Executed Operation**: tracks what actually happened for each planned step (start/end times, used resources) and is linked to both the VVE and the Operation Plan.
- **Plan Synchronization**: the status of each planned operation is kept in sync as “started,” “completed,” or “delayed” depending on execution updates.
- **Audit and Timestamps**: every execution update records the operator’s ID and the precise timestamp for accountability.

### Business Rules
1. SPA builds executed operation entries from the existing plan but lets operators edit the key fields before saving.
2. Execution updates trigger downstream recalculations of overall completion status and delays.
3. Plan vs. actual reporting must reflect the latest executed operation statuses so performance dashboards stay accurate.
