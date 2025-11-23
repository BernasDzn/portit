# US3204 - As a System User, I want the system to restrict access to actions and features based on my role, so that I cannot perform unauthorized operations.

## 1. User Story Description *(from project statement)*
> As a System User, I want the system to restrict access to actions and features based on my role, so that I cannot perform unauthorized operations.

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

**On the back-end side:**
- Each REST API route must enforce role-based access control (RBAC) and/or attribute-based access control (ABAC) as needed to enforce the applicable business rules.
- Unauthorized requests must return proper HTTP status codes (e.g., 403 Forbidden).
- Logs must record unauthorized attempts.

**On the front-end side:**
- Front-end routes must check for the user's authorization before rendering pages.
- Direct URL access to unauthorized pages must be prevented.
- A default "Access Denied" or "Not Authorized" page must be shown when needed.

</p>
</details> 


## 4. Business Value
> ...

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../../global_docs/def_of_ready.md)

## 6. Definition of Done
- The implementation checks all acceptance criteria.

