# US4114 - As a Port Operations Supervisor, I want to manage the catalog of Complementary Task Categories so that non-cargo-related activities are consistently classified and can be properly recorded during vessel visits.

## 1. User Story Description *(from project statement)*
> As a Port Operations Supervisor, I want to manage the catalog of Complementary Task Categories so that non-cargo-related activities are consistently classified and can be properly recorded during vessel visits.

## 2. Customer Specifications and Clarifications

<details><summary>From the specifications document</summary>
<p>

- CRUD operations for Complementary Task Categories must be available via the REST API.
- SPA must allow searching and managing these categories.
- Each category contains a unique code, name, description, and optionally a default duration or expected impact.
- Example categories include safety/security, maintenance, and cleaning/housekeeping tasks.

</p>
</details>

<details><summary>From the client clarifications</summary>
<p>

No clarifications were provided.

</p>
</details>

## 3. Acceptance Criteria:

<details><summary>Show acceptance criteria</summary>
<p>

- API supports create, read, update, and delete operations for task categories, enforcing unique codes.
- SPA enables quick filtering, searching by code/name, and inline edits for name/description.
- Categories may define an optional default duration or impact level, which informs scheduling decisions.
- The list of categories can be grouped by type (Safety/Security, Maintenance, Cleaning) for easier navigation.

</p>
</details>

## 4. Business Value
> A consistent catalog keeps complementary tasks clearly defined and helps planners understand how non-cargo work affects vessel visit timelines.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../../../global_docs/def_of_ready.md)

## 6. Definition of Done
- REST API endpoints enforce unique codes and validate optional duration values.
- SPA lists categories with search/filters and exposes the default duration field in the form.
- CRUD flows are covered by automated tests.
