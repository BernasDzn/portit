# US3502 - As a System Administrator, I want access to the solution to be restricted to clients connected to the DEI internal network (wired or via VPN), so that the system remains secure and compliant with institutional access policies.

## 1. User Story Description *(from project statement)*
> As a System Administrator, I want access to the solution to be restricted to clients connected to the DEI internal network (wired or via VPN), so that the system remains secure and compliant with institutional access policies.

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

- Network access must be enforced through, for instance, VPN or IP whitelisting, configured at the host or proxy level.
- Authentication must still be handled by the external IAM, but authorization is only granted if the client is within the approved network context.
- Unauthorized external access attempts must be logged and denied.
- This restriction applies to the development and staging environments.

</p>
</details> 


## 4. Business Value
> ...

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../global_artifacts/def_of_ready.md)

## 6. Definition of Done
- The implementation checks all acceptance criteria.

