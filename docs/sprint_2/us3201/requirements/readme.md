# US 3201 - Authenticate using External IAM Provider

## 1. User Story Description *(from project statement)*  
> As a (Non-Authenticated) System User, I want to authenticate using the external IAM provider, so that I can securely access the system without managing separate credentials.

## 2. Customer Specifications and Clarifications

<details><summary>From the specifications document</summary>
<p>

> There are no additional specifications provided for this User Story.

</p>
</details>

<details><summary>From the client clarifications</summary>
<p>

> **Q:** Are there any specific recommendations regarding the IAM provider to be used, or will further details be provided?  
> **A:** It is the team’s responsibility to select the external IAM provider, as long as it meets the intended purpose. Integration with providers such as Google, Facebook, or Microsoft is acceptable. The IAM must support standard authentication protocols (e.g., OAuth2/OpenID Connect). Integration procedures vary by provider, and this should be considered when selecting the IAM.

</p>
</details>

## 3. Acceptance Criteria:

<details><summary>Show acceptance criteria</summary>
<p>

- The SPA must integrate with the selected IAM.  
- Unauthenticated users must be redirected to the IAM login page.  
- The system must not handle password storage.  
- After successful authentication, a valid access token must be available to the front-end.  
- Logout must be supported, clearing tokens and/or session data.

</p>
</details>

## 4. Business Value  
> This feature provides secure access without requiring the user to manage new credentials, reduces security risks associated with password storage, simplifies onboarding, and leverages trusted identity providers to streamline authentication.

## 5. Definition of Ready  
> This US follows the defined [global definition of ready](../../../global_docs/def_of_ready.md)

## 6. Definition of Done  
- Integration with the selected IAM provider is implemented using the appropriate authentication standard (OAuth2/OpenID Connect).  
- Unauthenticated users are properly redirected to the IAM login page.  
- The system does not store or process user passwords.  
- The SPA receives and securely stores a valid access token after authentication.  
- Logout functionality is implemented and clears tokens/session data.  
- The implementation satisfies all acceptance criteria, is tested, and has been reviewed.
