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
> Restricting system access to internal networks or VPN connections significantly reduces attack surface by preventing exposure to public internet threats. Network-level access control provides defense-in-depth alongside authentication, ensuring only authorized clients from trusted locations can reach the system. Logging unauthorized access attempts enables security monitoring and incident detection. This approach aligns with institutional security policies and compliance requirements for protecting sensitive operational systems.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../../global_docs/def_of_ready.md)

## 6. Definition of Done
- Network access is restricted via VPN or IP whitelisting
- Configuration is implemented at host or proxy level
- External IAM authentication is still enforced
- Unauthorized external access attempts are logged and denied
- Restrictions apply to development and staging environments
- The implementation checks all acceptance criteria

