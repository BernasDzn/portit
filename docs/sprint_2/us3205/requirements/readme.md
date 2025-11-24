# US3205 - As an Administrator, I want to assign (or update) the internal role(s) of a given user, so that they can access only the features appropriate to their responsibilities.

## 1. User Story Description *(from project statement)*
> As an Administrator, I want to assign (or update) the internal role(s) of a given user, so that they can access only the features appropriate to their responsibilities.

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

- Users are identified by IAM-provided attributes (userId, email, name).
- When authorizing a user for the first time:
  - A unique activation link is sent to their email.
  - By default, the users are set to a "deactivated" status.
- Internal roles determine system access level.

</p>
</details> 


## 4. Business Value
> Centralized role assignment capabilities enable administrators to efficiently manage user permissions and ensure proper access control aligned with organizational responsibilities. Email-based activation with secure links verifies user identity before granting access, while default deactivation status prevents unauthorized access from incomplete registrations. This workflow supports security best practices, maintains audit trails, and provides flexible user lifecycle management.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../../global_docs/def_of_ready.md)

## 6. Definition of Done
- Administrator interface allows role assignment and updates
- Users are identified by IAM attributes (userId, email, name)
- Activation links are generated and sent via email
- New users default to "deactivated" status
- Role assignments determine system access levels
- The implementation checks all acceptance criteria

