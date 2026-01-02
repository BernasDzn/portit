# US4102 - As a Logistics Operator, I want to automatically generate and store Operation Plans for all Vessel Visit Notifications (VVNs) scheduled for a given day using one of the available scheduling algorithms, so that cargo operations are efficiently organized and can later be monitored or adjusted.

## 1. User Story Description *(from project statement)*
> As a Logistics Operator, I want to automatically generate and store Operation Plans for all Vessel Visit Notifications (VVNs) scheduled for a given day using one of the available scheduling algorithms, so that cargo operations are efficiently organized and can later be monitored or adjusted.

## 2. Customer Specifications and Clarifications

<details><summary>From the specifications document</summary>
<p>

- The Planning & Scheduling module must produce a sequence of operations per VVN for a selected day, including assigned machinery, berths, and personnel with planned time windows.
- Metadata such as generation date, requesting user, and selected scheduling algorithm must be recorded for every saved plan.
- The front-end must show a preview of generated plans before they are persisted in the OEM module.

</p>
</details>

<details><summary>From the client clarifications</summary>
<p>

No clarifications were captured for this story.

</p>
</details>

## 3. Acceptance Criteria:

<details><summary>Show acceptance criteria</summary>
<p>

- The operator can select a target day and trigger generation for all VVNs scheduled for that day.
- The Planning & Scheduling module provides the selected algorithm to build each Operation Plan, covering every operation related to the VVN.
- Each Operation Plan includes assigned resources, time windows for loading/unloading, and links to the source VVN.
- The SPA allows operators to preview generated plans and review the planned resources before committing them to the OEM module.
- Saved Operation Plans include audit metadata such as creation date, author, and the scheduling algorithm used.

</p>
</details>

## 4. Business Value
> Automating plan generation guarantees the Logistics Operator can organize daily activities consistently, reduces manual scheduling effort, and keeps an auditable trail of how each plan was built.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../../../global_docs/def_of_ready.md)

## 6. Definition of Done
- API endpoints exist to request generation (`POST /operation-plans/generate`) and to persist reviewed plans (`POST /operation-plans`).
- The Planning & Scheduling module integration handles the chosen algorithm and returns structured operations, resources, and time windows per VVN.
- Metadata tracking (creation timestamp, user, algorithm identifier) is stored together with each saved plan.
- SPA workflow provides a preview list of generated plans and a confirmation step before saving.
- Unit/integration tests cover the generation API and metadata persistence; UI tests cover the preview/save flow.
