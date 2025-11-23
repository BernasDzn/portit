# US3505 - As a System Administrator, I want to control and monitor logins to the remote shells of Linux-based systems, so that I can prevent and report potential unauthorized access or misuse.

## 1. User Story Description *(from project statement)*
> As a System Administrator, I want to control and monitor logins to the remote shells of Linux-based systems, so that I can prevent and report potential unauthorized access or misuse.

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

- User authentication shall be permitted only between 08:00 and 22:00 local time. Access attempts outside this period must be denied.
- Following any failed authentication attempt, the system shall require Google Authenticator for multi-factor authentication before allowing subsequent login attempts.
- In the event of more than three consecutive failed authentication attempts, the system shall automatically generate an email alert to the system administrator.

</p>
</details> 


## 4. Business Value
> ...

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../../global_docs/def_of_ready.md)

## 6. Definition of Done
- The implementation checks all acceptance criteria.

