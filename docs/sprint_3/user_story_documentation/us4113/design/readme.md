# US4113 - As a Logistics Operator, I want to record and manage incidents that affect the execution of port operations, so that delays and operational disruptions can be accurately tracked, scoped, and analyzed.

### Process View
- SPA provides a dedicated incident board where filters for vessel, date, severity, and status narrow the list.
- Incident creation starts with the selected Incident Type and a decision about which VVEs it affects (all ongoing, specific ones, or upcoming ones for the day).
- Resolution is performed by setting the end timestamp; the OEM module then computes the duration automatically and closes the incident.

### REST Endpoints
- `POST /incidents`: creates an incident with mandatory fields and optional affected VVEs list.
- `GET /incidents`: supports filtering by vessel, period, severity, status, and optionally `withActive= true/false`.
- `PATCH /incidents/{id}`: updates description, severity, end time, and affected VVEs.
- `DELETE /incidents/{id}`: removes an incident when it was entered in error (subject to RBAC).
- `POST /incidents/{id}/associate-vves`: (helper) attaches or detaches specific VVEs in bulk to keep the relationships clear.

### SPA Behavior
- Highlights active incidents (no `endTime`) with badges and visual cues.
- Shows severity-level chips and allows quick filtering by severity.
- Provides quick actions for associating/detaching VVEs and marking incidents as resolved while displaying the computed duration.
