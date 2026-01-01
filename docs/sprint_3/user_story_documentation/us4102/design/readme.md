# US4102 - As a Logistics Operator, I want to automatically generate and store Operation Plans for all Vessel Visit Notifications (VVNs) scheduled for a given day using one of the available scheduling algorithms, so that cargo operations are efficiently organized and can later be monitored or adjusted.

### Process View
- SPA triggers plan generation for a given day and algorithm.
- OEM service forwards the request to Planning & Scheduling, receives candidate plans, and returns them for operator review.
- After confirmation, OEM persists plans with audit metadata and notifies any downstream modules via REST webhooks if needed.

### REST Endpoints
- `POST /operation-plans/generate`: accepts `date` and `algorithmId`, proxies to Planning & Scheduling and returns plan previews without saving.
- `GET /operation-plans/preview`: optional, allows the SPA to poll generation progress or fetch prepared execution windows.
- `POST /operation-plans`: persists reviewed plans with `createdBy`, `createdAt`, and `algorithmId` fields.

### SPA Behavior
- Displays generated operations grouped by VVN, allowing inline validation before persisting.
- Shows warnings if planning returns missing resources or overlaps, requiring operator confirmation.

### Data Consistency
- Persistence includes referential integrity to VVN, assigned resources (crane, dock, staff), and the chosen algorithm.
- Audit logs capture `operationPlanId`, `creator`, timestamp, and algorithm, facilitating later analysis or rollback.
