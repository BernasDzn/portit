# US 2214 - Register and manage physical resources

## 1. User Story Description *(from project statement)*
> As a Logistics Operator, I want to register and manage physical resources (create, update, deactivate), so that they can be accurately considered during planning and scheduling operations.

## 2. Customer Specifications and Clarifications

<details><summary>From the specifications document</summary>
<p>

> *"Efficient port operations depend either on physical resources (cf. 3.1.2.1) and on human resources (cf. 3.1.2.2)."*

> *"Efficient operation of a port relies on a variety of logistic resources used to move cargo between vessels, yards, and warehouses. Among the most critical are ship-to-shore (STS) cranes, which are large fixed cranes permanently installed at the docks... In addition to these fixed cranes, ports rely on mobile resources such as yard gantry cranes and trucks or terminal tractors."*

> *"Resource characteristics directly affect the planning and execution of loading and unloading tasks. Each resource registered in the system must capture a set of standardized attributes that allow both day-to-day management and the later application of intelligent scheduling and planning algorithms."*

> *"Physical Resources include: Operational Window (weekly basis), Operational Capacity (type-dependent), Current Availability Status, Operating Staff Requirements, Setup Time."*

</p>
</details>

<details><summary>From the client clarifications</summary>
<p>

> **Q:** "Um physical resource deverá ter sempre alguma qualificação associada? Por exemplo um carrinho de mão precisaria de alguma qualificação associada?"
<br> **A:** "A physical resource may require no qualifications to be operated."

> **Q:** "When creating a physical resource, should its status be automatically assigned to available or should the Logistics Operator choose it."
<br> **A:** "By default, it can be available."

</p>
</details> 

## 3. Acceptance Criteria:

<details><summary>Show acceptance criteria</summary>
<p>

- Resources include cranes (fixed and mobile), trucks, and other equipment directly involved in vessel and yard operations
- Each resource must have a unique alpha-numeric code and a description
- Each resource must store its operational capacity, which varies according to the kind of resource
- Each resource must store its assigned area (e.g., Dock A, Yard B) if applicable
- Additional properties must include:
  - Current availability status (active, inactive, under maintenance)
  - Setup time (in minutes), if relevant, before starting operations
  - (Staff) Qualification requirements, ensuring only properly certified staff can be scheduled with the resource
- Deactivation/reactivation must not delete resource data but preserve it for audit and historical planning purposes
- Resources must be searchable and filterable by code, description, kind of resource, status

</p>
</details> 

## 4. Business Value
> This feature is as important as keeping inventory is in our business. It ensures optimal utilization of port equipment, prevents scheduling conflicts, and enables efficient allocation of resources during vessel operations.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../global_artifacts/def_of_ready.md)

## 6. Definition of Done
- The system allows for the authorized users to create a physical resource
- The system allows for the authorized users to update a physical resource
- The system allows for the authorized users to deactivate a physical resource
- The system preserves historical data for deactivated resources
- The system enforces qualification requirements for resource-staff assignments
- The changes made are effectuated on persistent storage
- All acceptance criteria are met

## 7. Dependencies
- (Qualifications Management)
- (Dock Management)

## 8. Notes
- Resource types: STS Cranes, Yard Gantry Cranes, Trucks
- Operational capacity measurements vary by resource type (containers/hour for cranes, containers/trip for trucks)
- Setup time is critical for scheduling accuracy
- Status management is essential for maintenance planning and operational readiness