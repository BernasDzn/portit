# US4108 - As a Logistics Operator, I want to update an in progress VVE with the actual berth time and dock used, so that discrepancies from the planned dock assignment are recorded.

## 1. User Story Description *(from project statement)*
> As a Logistics Operator, I want to update an in progress VVE with the actual berth time and dock used, so that discrepancies from the planned dock assignment are recorded.

## 2. Customer Specifications and Clarifications

<details><summary>From the specifications document</summary>
<p>

- The REST API must support updating the berth time and dock ID of VVEs.
- If the actual dock differs from the planned dock a warning or explanatory note must be added to the record.
- Every update must be timestamped and logged for auditing.

</p>
</details>

<details><summary>From the client clarifications</summary>
<p>

No clarifications captured yet.

</p>
</details>

## 3. Acceptance Criteria:

<details><summary>Show acceptance criteria</summary>
<p>

- API supports patching berth-related fields on VVEs (`PATCH /vves/{id}/berth`).
- Updates store the actual berth time and dock identifier together with the acting user and timestamp.
- The system automatically creates a note or warning when the chosen dock does not match what was assigned in the Operation Plan.
- The SPA surfaces the warning and allows operators to attach a short comment explaining the change.

</p>
</details>

## 4. Business Value
> Recording real berth information and discrepancies keeps operational history accurate while surfacing deviations for future analysis.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../../../global_docs/def_of_ready.md)

## 6. Definition of Done
- The berth update API enforces timestamped logging and links to the operator making the change.
- Dock mismatch warnings feed into both the API response and the SPA UI.
- Tests verify warning generation, note persistence, and audit metadata.
