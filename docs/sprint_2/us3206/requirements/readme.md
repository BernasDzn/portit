# US3206 - As a System User receiving an activation link, I want to complete my first access securely through authentication, so that I can start using the system.

## 1. User Story Description *(from project statement)*
> As a System User receiving an activation link, I want to complete my first access securely through authentication, so that I can start using the system.

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

- The activation link redirects the user to authenticate via IAM.
- Once authenticated, the system must confirm that the authenticated user data matches the user identity related to the link being used:
  - In case of success, the system completes the activation process (status update).
  - Otherwise, an error must be presented, preventing system access.
- Expired or invalid links must show an error message.
- After activation, the user gains role-based access.

</p>
</details> 


## 4. Business Value
> ...

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../../global_docs/def_of_ready.md)

## 6. Definition of Done
- The implementation checks all acceptance criteria.

