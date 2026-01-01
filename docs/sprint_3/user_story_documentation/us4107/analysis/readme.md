# US4107 - As a Logistics Operator, I want to create a Vessel Visit Execution (VVE) record when a vessel arrives at the port, so that the actual start of operations can be logged and monitored.

### Domain Concepts
- **Vessel Visit Execution (VVE)**: ties to a VVN, tracks the real lifecycle of a vessel visit, and evolves through statuses such as In Progress and Completed.
- **VVE Identifier**: follows the same pattern as VVN IDs to maintain traceability, e.g., prefixes and sequential numbering.
- **Arrival Metadata**: includes vessel identifier, actual arrival timestamp, creator user, and optional notes about the arrival conditions.

### Business Rules
1. Creation requires referencing an existing VVN so resources and planning context are accessible.
2. VVEs default to the “In Progress” status and expose links to the corresponding Operation Plan and Operation Plan ID.
3. SPA should look up VVNs and prefill the vessel name, ETA, and scheduled berth for the operator.
