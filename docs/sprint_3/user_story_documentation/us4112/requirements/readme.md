# US4112 - As a Port Authority Officer, I want to manage the catalog of Incident Types so that the classification of operational disruptions remains standardized, hierarchical, and clearly distinct from complementary tasks

## 1. User Story Description *(from project statement)*
> As a Port Authority Officer, I want to manage the catalog of Incident Types so that the classification of operational disruptions remains standardized, hierarchical, and clearly distinct from complementary tasks.

## 2. Customer Specifications and Clarifications

<details><summary>From the specifications document</summary>
<p>

Specifications:

- Incident Types must be stored in a hierarchical model: an Incident Type may reference a parent Incident Type, forming a tree that supports grouping and filtering by parent.
- The system MUST expose CRUD operations for Incident Types via a REST API.
- The SPA must provide an intuitive UI to list, filter, create and update Incident Types.

</p>
</details>

<details><summary>From the client clarifications</summary>
<p>

No client clarifications for this user story.

</p>
</details>

## 3. Acceptance Criteria:

<details><summary>Show acceptance criteria</summary>
<p>

- AC1: Incident Types are persisted with optional parent and subtype references, enabling hierarchy.
- AC2: REST API provides CRUD operations for Incident Types with validation (unique `code`, required `name`, permitted `severity` values).
- AC3: SPA UI allows listing, filtering (by severity, name), and managing the hierarchy (assign/change parent, create child types).
- AC4: Only authorized users (Port Authority Officers) can create and update Incident Types.
- AC5: Tests (unit + integration) cover the API and data model; UI tests cover main CRUD flows.

</p>
</details>


## 4. Business Value
> Standardizing incident type classification reduces ambiguity when reporting and responding to operational disruptions, improves filtering and analytics and ensures consistent incident handling on vessel visit executions.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../../global_docs/def_of_ready.md)

## 6. Definition of Done
- All Acceptance Criteria are implemented and verified.
- REST API endpoints for Incident Types exist and are documented (OpenAPI/Swagger) with examples.
- SPA UI implemented for listing, filtering, and managing Incident Types, including hierarchy handling.
- Unit tests and integration tests are added and passing; end-to-end tests for main CRUD flows pass where applicable.
- Documentation updated in the repository (requirements + README sections).
- Role-based access control enforced for management operations.

