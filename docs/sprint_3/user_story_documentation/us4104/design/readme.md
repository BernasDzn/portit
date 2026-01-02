# US4104 - As a Logistics Operator, I want to manually update the Operation Plan of a given VVN, so that last-minute adjustments (e.g., resource or timing changes) can be made when needed.

### Process View
- SPA shows editable fields (cranes, staff counts, start/end times) when the operator chooses a plan.
- `PUT /operation-plans/{id}` validates the input and checks for resource overlaps before committing updates.
- If conflicts are detected, the API returns warnings and requires a confirmation/comment to proceed.

### REST Endpoints
- `PUT /operation-plans/{planId}`: updates detailed plan attributes, enforces validation, and writes audit logs.
- `GET /operation-plans/{planId}/alerts`: optionally surfaces detected inconsistencies (double-booking, missing staff) without modifying data.

### SPA Behavior
- Warning banners highlight resource conflicts and show a short list of affected VVNs or time windows.
- A small form in the banner captures the update reason, which attaches to the change log when the user confirms the override.
