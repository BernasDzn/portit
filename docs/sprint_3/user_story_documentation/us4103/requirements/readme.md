# US4103 - As a Logistics Operator, I want to search and list Operation Plans for a given day or period, so that I can quickly review all scheduled activities within that timeframe.

## 1. User Story Description *(from project statement)*
> As a Logistics Operator, I want to search and list Operation Plans for a given day or period, so that I can quickly review all scheduled activities within that timeframe.

## 2. Customer Specifications and Clarifications

<details><summary>From the specifications document</summary>
<p>

- The REST API must allow querying Operation Plans by date range and vessel identifier.
- The SPA must present a searchable and filterable datatable summarizing the plans.
- Results must expose sortable columns, including start time, vessel name, and expected delay.

</p>
</details>

<details><summary>From the client clarifications</summary>
<p>

No additional clarifications were provided.

</p>
</details>

## 3. Acceptance Criteria:

<details><summary>Show acceptance criteria</summary>
<p>

- Query parameters for date range (`from`, `to`) and vessel identifier return matching Operation Plans.
- The API response contains essential summary data (vessel, dock, start/end time, assigned resources, status).
- The SPA table supports text search, filters, and sortable columns for at least start time, vessel name, and projected delay.
- Pagination or infinite scroll keeps the table responsive even when many plans exist.
- Sorting must be enabled in both directions for the most important columns.

</p>
</details>

## 4. Business Value
> Providing fast, filterable search helps operators monitor planned work, spot conflicts, and prepare adjustments ahead of execution.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../../../global_docs/def_of_ready.md)

## 6. Definition of Done
- `GET /operation-plans` supports date range and vessel filters plus pagination metadata.
- API responses include summary fields for display, and sort order can be passed from the SPA.
- SPA table exposes filtering, searching, and sortable columns with accessible labels.
- API and UI tests cover filtering, sorting, and pagination scenarios.
