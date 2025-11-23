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
> Time-based access restrictions and multi-factor authentication after failed attempts create layered security defenses against unauthorized access. Limiting authentication to business hours reduces exposure to automated attacks during off-hours, while MFA requirements after failures prevent credential compromise. Automated email alerts enable rapid response to suspicious activity. These measures protect critical infrastructure while balancing security with operational usability during normal business operations.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../../global_docs/def_of_ready.md)

## 6. Definition of Done
- User authentication is restricted to 08:00-22:00 local time
- Access attempts outside time window are denied
- Google Authenticator MFA is required after failed authentication attempts
- Email alerts are sent to admin after >3 consecutive failed attempts
- The implementation checks all acceptance criteria

