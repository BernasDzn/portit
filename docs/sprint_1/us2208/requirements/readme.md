# US 2208 - Register and update vessel visit notifications 

## 1. User Story Description *(from project statement)*
> As a Port Authority Officer, I want to register and manage representatives of a shipping agent organization (create, update, deactivate), so that the right individuals are authorized to interact with the system on behalf of their organization.

## 2. Customer Specifications and Clarifications

<details><summary>From the specifications document</summary>
<p>

> Data needed include: notification id, expected arrival and departure dates, crew, cargo manifests, representatives, vessel information (linked to vessel records)

</p>
</details>

<details><summary>From the client clarifications</summary>
<p>

> **Q:** Can crew information be updated after the initial notification is created?
<br> **A:** While the notification isnt submitted for approval, all information, including crew details, can be updated as needed.

> **Q:** Does withdrawing a notification mean it is completely removed from the system, or is it marked as inactive?
<br> **A:** Withdrawing a notification marks it as inactive but retains the record in the system for auditing purposes.

> **Q:** Do representatives manage notifications created by their organization only, or can they manage notifications for other organizations as well?
<br> **A:** Representatives can only manage notifications created by their own organization.

> **Q:** Should a representative be able to see all notifications from their organization, or only those they created?
<br> **A:** Representatives should be able to see all notifications from their organization, as defined in the filter criteria.

> **Q:** 
In the assignment, it is stated that, for most visits, the crew information to be stored is limited to the captain's name and the number of crewmates, and that, should the vessel carry hazardous or dangerous cargo, that it should also include information regarding safety officers on board.\
Is this all the necessary crew information? Do we need additional crew information of anyone who isn't a safety officer?
<br> **A:** Yes. By now, no need for more crew information than that.

> **Q:** What is the attribute that uniquely identifies a vessel visit notification?
<br> **A:**Great question!!!
Different approaches are usually followed.
However, in this case, each Vessel Visit Notificationmust have a unique business identifier following the pattern
{YEAR}-{PORT_CODE}-{SEQUENTIAL_NUMBER}
where:\
-- YEAR: the calendar year when the visit is first registered.\
-- PORT_CODE: a short alphanumeric code uniquely identifying the port (e.g., “PTLEI” for "Porto de Leixões").\
-- SEQUENTIAL_NUMBER: a zero-padded integer (e.g., 000001, 000002, …) assigned incrementally per port and year.\
The combination (YEAR, PORT_CODE, SEQUENTIAL_NUMBER) must be unique across the system.
Once assigned, the identifier must remain immutable even if other visit details (dates, vessel, etc.) are updated.

</p>
</details> 

## 3. Business Value
> This feature is crucial for the efficient management of vessel visit notifications, which are essential for port operations. By enabling the registration and updating of these notifications, the system ensures that all relevant information is accurately captured and maintained. This functionality supports effective communication between port authorities and shipping agents, enhances operational planning, and ensures compliance with maritime regulations. Ultimately, it contributes to the smooth functioning of port activities and improves overall service quality.

## 4. Definition of Done
- Vessel visit notifications can be created with all required attributes.
- Vessel visit notifications can be updated with new information.
- Validation is in place to ensure data integrity (e.g., unique notification IDs).
- Unit and integration tests are implemented to verify functionality.
