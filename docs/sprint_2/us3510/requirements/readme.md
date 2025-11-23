# US3510 - As a System Administrator, I want to control the logins to the remote shells of Linux-based systems, so that I can prevent brute-force, dictionary, and time-based attacks.

## 1. User Story Description *(from project statement)*
> As a System Administrator, I want to control the logins to the remote shells of Linux-based systems, so that I can prevent brute-force, dictionary, and time-based attacks.

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

### Password Policy Requirements
All users should comply with the rules for password strength. The policy defines that the passwords should:
- Be at least 12 characters long
- Contain at least one uppercase, one lowercase, one number, and one special character (!@#$%^&*()-_=+[]{};:,<.>)
- Must not contain your username, name, or any obvious word

### Failed Login Protection
- Following any failed authentication attempt from any user except root, the system shall introduce a waiting time of 10 seconds before allowing subsequent login attempts.
- The system shall block any user account (except root) after 5 failed login attempts. The process to unlock should require manual interaction from the root.

</p>
</details> 


## 4. Business Value
> Implementing strong authentication controls and password policies significantly reduces the risk of unauthorized access through brute-force, dictionary, and time-based attacks. By enforcing password complexity requirements, the system ensures that user credentials are resistant to common attack patterns. The delayed retry mechanism and account lockout policies make automated attacks impractical by introducing time costs and requiring administrative intervention. These security measures protect critical infrastructure, maintain system integrity, and ensure compliance with security best practices and regulatory requirements.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../global_artifacts/def_of_ready.md)

## 6. Definition of Done
- The implementation checks all acceptance criteria.

