# US4108 - As a Logistics Operator, I want to update an in progress VVE with the actual berth time and dock used, so that discrepancies from the planned dock assignment are recorded.

### Process View
- Operator edits the in-progress VVE details to reflect the actual berth/dock information.
- The SPA calls `PATCH /vves/{vveId}/berth` to persist the new dock ID and berth timestamp.
- If the dock differs from the planned location, the API flags the difference, adds a warning note, and returns the note to the SPA.

### REST Endpoints
- `PATCH /vves/{vveId}/berth`: updates `actualBerthTime` and `actualDockId`, records the operator/timestamp, and optionally stores a warning message.
- `GET /vves/{vveId}/warnings`: allows the SPA to refresh warnings without re-sending the update payload.

### SPA Behavior
- Shows a warning banner when the actual dock differs from the planned dock and offers a short text box for the operator to explain the change.
- Displays the timestamp of the last berth update and the user who performed it.
