# US4104 - As a Logistics Operator, I want to manually update the Operation Plan of a given VVN, so that last-minute adjustments (e.g., resource or timing changes) can be made when needed.

## 1. User Story Description *(from project statement)*
> As a Logistics Operator, I want to manually update the Operation Plan of a given VVN, so that last-minute adjustments (e.g., resource or timing changes) can be made when needed.

## 2. Customer Specifications and Clarifications

<details><summary>From the specifications document</summary>
<p>

- The REST API must expose update endpoints for operation plans.
- The SPA must allow editing crane assignment, start/end times, staff, and similar key attributes.
- Any change must be validated to avoid resource conflicts and invalid intervals, logged with date, author, and a justification.
- Operators should be warned if the update introduces potential consistency issues with other VVNs or resource availability.

</p>
</details>

<details><summary>From the client clarifications</summary>
<p>

No client clarifications were captured for this story.

</p>
</details>

## 3. Acceptance Criteria:

<details><summary>Show acceptance criteria</summary>
<p>

- Update endpoints (`PUT`/`PATCH`) exist for Operation Plans and validate incoming adjustments.
- SPA editing UI focuses on crane assignment, start/end time windows, and assigned personnel.
- Every accepted change is timestamped, associated with the acting user, and stores a short reason for the modification.
- The system computes and surfaces warnings when an update may create overlaps with other VVNs or exceed resource availability (e.g., crane double-booking).
- Audit trails are retrievable for each change.

</p>
</details>

## 4. Business Value
> Allowing controlled updates keeps Operation Plans aligned with reality when situations change, minimizing delays while keeping a trustworthy audit trail.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../../../global_docs/def_of_ready.md)

## 6. Definition of Done
- Update API endpoints validate dates, resources, and staff availability before persisting changes.
- Audit records capturing date, author, and reason exist for each update and can be queried alongside the plan.
- SPA form highlights warnings when conflicting VVNs or resource overloads are detected.
- Tests cover validation logic, warning generation, and audit logging.
