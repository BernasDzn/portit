# US4114 - As a Port Operations Supervisor, I want to manage the catalog of Complementary Task Categories so that non-cargo-related activities are consistently classified and can be properly recorded during vessel visits.

### Process View
- SPA presents a searchable list of categories grouped by type (Safety/Security, Maintenance, Cleaning).
- Supervisors use inline actions to add, edit, or deactivate categories while keeping the default duration field visible.
- The OEM API enforces unique codes and returns the last modified metadata for display.

### REST Endpoints
- `POST /complementary-task-categories`: creates a new category with `code`, `name`, `description`, and optional `defaultDuration`.
- `GET /complementary-task-categories`: returns the catalog, supports search by code/name, and allows filtering by grouping or impact level.
- `PUT /complementary-task-categories/{id}`: updates editable fields and optionally toggles the active flag.
- `DELETE /complementary-task-categories/{id}`: marks the category as inactive so existing Complementary Tasks can still reference it.

### SPA Behavior
- Displays default duration hints (e.g., “typically 1h delay”) in the list view for easy scanning.
- Provides a modal form for creation/editing that validates unique codes before saving and offers dropdowns for grouping examples.
