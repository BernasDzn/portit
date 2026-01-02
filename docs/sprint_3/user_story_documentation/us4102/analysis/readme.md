# US4102 - As a Logistics Operator, I want to automatically generate and store Operation Plans for all Vessel Visit Notifications (VVNs) scheduled for a given day using one of the available scheduling algorithms, so that cargo operations are efficiently organized and can later be monitored or adjusted.

### Domain Concepts
- **Operation Plan**: aggregates the sequence of cargo operations for a VVN, including assigned dock, crane, staff, start/end windows, and a reference to the original VVN.
- **Planning Algorithm**: encapsulates different scheduling strategies exposed by the Planning & Scheduling module; each generated plan must capture the algorithm identifier for traceability.
- **VVN**: the scheduling unit that triggers plan generation; only VVNs scheduled for the requested day are part of the operation batch.
- **Audit Metadata**: each saved plan stores creator, creation timestamp, and algorithm used to provide accountability.

### Business Rules and Flow
1. Operator selects the target day and scheduling algorithm via the SPA.
2. The SPA calls `POST /operation-plans/generate` to obtain generated plans from Planning & Scheduling without persisting them.
3. The SPA shows a preview; only after confirmation does it call `POST /operation-plans` to save plans in OEM.
4. Only saved plans contribute to reporting and downstream modules.
