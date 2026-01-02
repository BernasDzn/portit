# US4104 - As a Logistics Operator, I want to manually update the Operation Plan of a given VVN, so that last-minute adjustments (e.g., resource or timing changes) can be made when needed.

### Domain Concepts
- **Operation Plan**: maintains planned operations for a VVN, including crane, dock, staff assignments, and planned time windows.
- **Resource Availability**: cranes, docks, and staff cannot overlap across VVNs without a documented justification; the system compares proposed updates against booked slots.
- **Audit Record**: every edit writes a timestamped log entry containing the user, update reason, and detected conflicts.

### Business Rules
1. The API ensures the updated time range fits within the VVN’s working day and does not clash with other assigned operations.
2. Resource conflict detection runs before persistence; if conflicts exist, the response includes warnings to the SPA.
3. Operators must explain why they override warnings, and the explanation is stored alongside the audit record.
