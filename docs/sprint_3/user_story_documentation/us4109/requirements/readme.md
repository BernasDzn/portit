# US4109 - As a Logistics Operator, I want to update an in progress VVE with executed operations, so that the system reflects real execution progress and performance.

## 1. User Story Description *(from project statement)*
> As a Logistics Operator, I want to update an in progress VVE with executed operations, so that the system reflects real execution progress and performance.

## 2. Customer Specifications and Clarifications

<details><summary>From the specifications document</summary>
<p>

- Executed operations are derived from the planned ones, and the SPA should make it easy to confirm/adjust start and end times plus resource usage.
- Planned operations must be marked as “started,” “completed,” or “delayed” depending on execution updates.
- Execution updates capture timestamps and operator ID.
- Completion status synchronizes with the linked Operation Plan for comparison and reporting.

</p>
</details>

<details><summary>From the client clarifications</summary>
<p>

No clarifications were provided.

</p>
</details>

## 3. Acceptance Criteria:

<details><summary>Show acceptance criteria</summary>
<p>

- The API allows recording executed operations for an in-progress VVE, defaulting values from the related plan.
- Operators can adjust start/end times and resource usage before confirming each executed operation.
- The API updates the planned operation status to “started,” “completed,” or “delayed” as appropriate.
- Execution updates store timestamps and the acting operator ID for every change.
- Operation Plan versus actual statuses stay synchronized to enable performance comparisons.

</p>
</details>

## 4. Business Value
> Tracking what actually happened during execution enables accurate reports, highlights delays, and keeps the plan vs. reality comparison reliable.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../../../global_docs/def_of_ready.md)

## 6. Definition of Done
- API endpoints supporting executed operation updates persist status transitions and resource-hour details.
- SPA view pre-populates planned operations and lets operators confirm or edit execution data.
- Audit metadata (timestamps, operator ID) is captured for every update.
- Tests validate synchronization between the Operation Plan and the recorded execution statuses.
