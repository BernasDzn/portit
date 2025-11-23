# US3401 - As a Project Manager, I want the team to develop a dedicated back-end module that provides planning and scheduling algorithms through a REST-based API, consuming information from the existing back-end modules, so that operational plans can be computed dynamically and consistently without duplicating data.

## 1. User Story Description *(from project statement)*
> As a Project Manager, I want the team to develop a dedicated back-end module that provides planning and scheduling algorithms through a REST-based API, consuming information from the existing back-end modules, so that operational plans can be computed dynamically and consistently without duplicating data.

## 2. Customer Specifications and Clarifications

<details><summary>From the specifications document</summary>
<p>

> none

</p>
</details>

<details><summary>From the client clarifications</summary>
<p>

> none

</p>
</details> 

## 3. Acceptance Criteria:

<details><summary>Show acceptance criteria</summary>
<p>

- The module must expose its algorithms / functionalities through a REST-based API.
- The module must consume existing data from other back-end services via their exposed APIs (e.g., staff, resources).
- The module must not persist operational data — it only computes and returns scheduling results upon request.
- Input and output payloads must follow JSON format and use consistent identifiers with other modules (e.g., resource IDs).
- The module API must be properly documented (e.g. via OpenAPI/Swagger) and accessible.

</p>
</details> 


## 4. Business Value
> ...

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../../global_docs/def_of_ready.md)

## 6. Definition of Done
- The implementation checks all acceptance criteria.

