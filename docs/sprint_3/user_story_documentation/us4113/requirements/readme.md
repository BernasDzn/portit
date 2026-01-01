# US4113 - As a Logistics Operator, I want to record and manage incidents that affect the execution of port operations, so that delays and operational disruptions can be accurately tracked, scoped, and analyzed.

## 1. User Story Description *(from project statement)*
> As a Logistics Operator, I want to record and manage incidents that affect the execution of port operations, so that delays and operational disruptions can be accurately tracked, scoped, and analyzed.

## 2. Customer Specifications and Clarifications

<details><summary>From the specifications document</summary>
<p>

- CRUD operations for incidents must be available through the REST API.
- The SPA must allow filtering and listing by vessel, date range, severity, and status (active/resolved).
- Operators must be able to associate or detach VVEs, highlight active incidents, and capture impact scope.
- Incident records include an ID, Incident Type reference, start/end timestamps, severity, description, responsible user, and affected VVEs (all, specific, or upcoming).
- When marked resolved, the incident duration must compute automatically.

</p>
</details>

<details><summary>From the client clarifications</summary>
<p>

No clarifications were made available.

</p>
</details>

## 3. Acceptance Criteria:

<details><summary>Show acceptance criteria</summary>
<p>

- API supports full CRUD on incidents with mandatory fields (Incident Type, timestamps, severity, description).
- Filtering options include vessel, period, severity, and status, and respond quickly even with growing incident volumes.
- Each incident captures which VVEs are affected: all ongoing, specific selections, or upcoming ones for the same period.
- Active incidents are highlighted in the SPA and can be linked/detached from VVEs in a few clicks.
- When an incident is resolved via end timestamp, its duration is automatically derived and stored.
- All records store the creating user and support audit traces.

</p>
</details>

## 4. Business Value
> Incident management with strong linking to VVEs improves situational awareness, supports root-cause analysis, and ensures safer, more resilient operations.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../../../global_docs/def_of_ready.md)

## 6. Definition of Done
- The incidents API exposes filtering endpoints plus the ability to associate/detach VVEs.
- Active incidents are surfaced in the SPA, and resolution triggers automatic duration calculation.
- Severity and status metadata are validated and stored with every incident.
- Tests cover CRUD operations and filter combos; UI tests cover the active incident highlighting and VVE association flows.
