# US3202 - As a System User, I want the system to automatically load my internal authorization role after authentication, so that I gain access only to my permitted features.

## 1. User Story Description *(from project statement)*
> As a System User, I want the system to automatically load my internal authorization role after authentication, so that I gain access only to my permitted features.

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

- After IAM login, the SPA must call a backend endpoint to retrieve the user's assigned role and render the respective menu options.
- If the user has no assigned role or it is inactive, access must be denied with an appropriate message.

</p>
</details> 


## 4. Business Value
> Automatic role loading after authentication streamlines user access by eliminating manual role selection steps and ensures users immediately see only their permitted features. This reduces onboarding friction, prevents unauthorized access attempts, and maintains consistent enforcement of role-based access control policies across the system, enhancing both security and user experience.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../../global_docs/def_of_ready.md)

## 6. Definition of Done
- Backend endpoint returns user's assigned role after IAM authentication
- SPA calls this endpoint and stores role information
- Menu options render based on retrieved role
- Users without assigned roles or with inactive roles receive appropriate error message
- Access is properly denied for unauthorized users
- The implementation checks all acceptance criteria

