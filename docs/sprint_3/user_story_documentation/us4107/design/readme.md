# US4107 - As a Logistics Operator, I want to create a Vessel Visit Execution (VVE) record when a vessel arrives at the port, so that the actual start of operations can be logged and monitored.

### Process View
- Operator selects a scheduled VVN in the SPA and clicks “Record Arrival.”
- The SPA calls `POST /vves` with the VVN reference, actual arrival time, and the user credentials.
- The OEM API validates the VVN linkage, generates a VVE ID, marks the status as In Progress, and returns the new execution record.

### REST Endpoints
- `POST /vves`: creates the VVE, enforces the VVN reference, and sets defaults for status, creator, and timestamps.
- `GET /vvns/{id}/context`: (helper endpoint) supplies the SPA with any missing VVN data to ease creation.

### SPA Behavior
- Prepopulates the vessel name, ETA, and planned dock; operator only adds the actual arrival time and optional notes.
- Shows success toast with the new VVE ID and in-progress badge after creation.
