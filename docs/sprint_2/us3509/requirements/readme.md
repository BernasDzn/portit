# US3509 - As a System Administrator, I want to register all the remote logins to the shells of Linux-based systems using SSH.

## 1. User Story Description *(from project statement)*
> As a System Administrator, I want to register all the remote logins to the shells of Linux-based systems using SSH.

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

- Any connection attempt via SSH should be registered (including, at least, the time, remote IP, and the user).
- If more than 5 connection attempts within 20 seconds are verified, this should trigger an alert to all logged-in users and to the admin email.

</p>
</details> 


## 4. Business Value
> Monitoring and logging SSH login attempts enhances system security by providing visibility into access patterns and potential security threats. By tracking connection attempts with timestamps, IP addresses, and user information, administrators can detect suspicious activity and respond quickly to potential breaches. The alert mechanism for multiple failed attempts helps identify brute-force attacks in real-time, enabling rapid response to security incidents and protecting critical infrastructure from unauthorized access.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../global_artifacts/def_of_ready.md)

## 6. Definition of Done
- The implementation checks all acceptance criteria.

