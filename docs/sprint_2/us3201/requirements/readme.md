# US3201 - As a (Non-Authenticated) System User, I want to authenticate using the external IAM provider, so that I can securely access the system without managing separate credentials.

## 1. User Story Description *(from project statement)*
> As a (Non-Authenticated) System User, I want to authenticate using the external IAM provider, so that I can securely access the system without managing separate credentials.

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

- The SPA must integrate with the selected IAM (e.g., via OAuth2/OpenID Connect).
- Unauthenticated users must be redirected to the IAM login page.
- The system must not handle the password storage.
- After successful authentication, a valid access token must be available to the front-end.
- Logout must also be supported, clearing tokens/session data.

</p>
</details> 


## 4. Business Value
> Integrating with an external Identity and Access Management provider eliminates the need for separate credential management, reducing security risks associated with password storage and simplifying user experience. Single sign-on capabilities improve productivity by reducing authentication friction, while centralized identity management ensures consistent security policies across systems and reduces administrative overhead for user account management.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../../global_docs/def_of_ready.md)

## 6. Definition of Done
- IAM integration is configured (OAuth2/OpenID Connect)
- Unauthenticated users are redirected to IAM login page
- Access tokens are received and stored after successful authentication
- Logout functionality clears tokens and session data
- Password storage is not handled by the application
- The implementation checks all acceptance criteria

