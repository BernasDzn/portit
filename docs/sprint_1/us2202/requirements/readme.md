# US 2202 - Create and manage vessels

## 1. User Story Description *(from project statement)*
> As a Port Authority Officer, I want to register and update vessel records, so that valid vessels can be referenced in visit notifications.

## 2. Customer Specifications and Clarifications

<details><summary>From the specifications document</summary>
<p>

> Data needed include: vessel name, IMO number, vessel type*, vessel owner*, lenght, depth, draft

> Vessel Type: information about the ship max dimensions for each type of vessel, name and description

> Vessel Owner: information about the owner of the vessel, name, address, contact details, representatives...

</p>
</details>

<details><summary>From the client clarifications</summary>
<p>

> **Q:** What attributes can be updated in a vessel record?
<br> **A:** Most often, it is the operator/owner. Other information such as name and type is rare and most of the type due user input mistake.

> **Q:** Are the vessel dimensions (length, depth, draft) dictated by the vessel type, or can they be specified independently when registering a vessel?
<br> **A:** They can be specified independently, as some vessels of the same type may have different dimensions.

> **Q:** Can we treat the operator and owner as the same entity for simplicity, or do they need to be distinct?
<br> **A1:** They need to be distinct, as a vessel can have different entities as owner and operator.
<br> **A2:** (Later clarification) The professor said later in class that they should be "distinct", but for this first implementation, we can treat them as the same entity to simplify the process.

> **Q:** Can a single operator/owner be associated with multiple vessels?
<br> **A:** Yes, a single operator/owner can be associated with multiple vessels.

> **Q:** Should searching by name, for example, "Ever", return all vessels with names containing that substring, such as "Ever Given" and "Ever Glory"?
<br> **A:** Yes, searching by a substring should return all matching vessels.

</p>
</details> 

## 3. Business Value
> This feature is of utmost importance as it enables the accurate registration and management of vessel records, which are essential for the efficient operation of port activities. By ensuring that valid vessels can be referenced in visit notifications, the system enhances operational efficiency, safety, and compliance with maritime regulations. This functionality directly supports the core operations of the port authority, facilitating better planning and resource allocation.

## 4. Definition of Done
- Vessel records can be created with all required attributes.
- Vessel records can be updated with new information.
- Validation is in place to ensure data integrity (e.g., unique IMO numbers).
- Unit and integration tests are implemented to verify functionality.