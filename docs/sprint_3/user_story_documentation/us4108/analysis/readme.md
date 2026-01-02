# US4108 - As a Logistics Operator, I want to update an in progress VVE with the actual berth time and dock used, so that discrepancies from the planned dock assignment are recorded.

### Domain Concepts
- **VVE status**: remains “In Progress” while berth information is being captured and transitions to later stages once operations start.
- **Berth Assignment**: planned docks are part of the Operation Plan; actual dock updates must be captured with reasons if they diverge.
- **Audit Trail**: each dock/berth update stores the operator, timestamp, and any formed warnings.

### Business Rules
1. Berth updates are restricted to VVEs that are In Progress; completed visits are locked.
2. If the dock changes, the system appends a warning message and optionally triggers stakeholder notifications.
3. Updates log the operator’s identity and the exact timestamp for compliance.
