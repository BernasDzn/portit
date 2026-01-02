# US4114 - As a Port Operations Supervisor, I want to manage the catalog of Complementary Task Categories so that non-cargo-related activities are consistently classified and can be properly recorded during vessel visits.

### Domain Concepts
- **Complementary Task Category**: defines codes like `CTC001`, a name, description, and optional default duration or impact level to guide planning.
- **Category Grouping**: categories belong to broader buckets (Safety/Security, Maintenance, Cleaning) to aid filtering in the SPA.
- **Default Duration**: optional field that signals a typical delay or time window when the category is applied.

### Business Rules
1. Codes must be unique and follow a predictable pattern (prefix + sequence) for compliance.
2. CRUD operations are allowed only for authorized supervisors, and deletions are soft (mark as inactive) to avoid breaking historical Complementary Task references.
3. Optional default duration can be used by other modules (e.g., scheduling algorithms) but is not mandatory.
