# US3303 - As a Logistics Operator or Port Authority Officer, I want to see vessels and major resources (e.g., ship-to-shore cranes, yard gantry cranes) displayed in the 3D environment, so that I can visualize operational elements.

## 1. User Story Description *(from project statement)*
> As a Logistics Operator or Port Authority Officer, I want to see vessels and major resources (e.g., ship-to-shore cranes, yard gantry cranes) displayed in the 3D environment, so that I can visualize operational elements.

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

- Items must appear in default or assigned positions (e.g., docked vessel on its berth).
- The system must fetch required data from the existing REST APIs.
- Regarding resources, consider only those that have an assigned area (e.g. dock A, Yard B).
- Regarding vessels, consider the information on the approved vessel visit notifications only.
- Models representing items can either be procedurally created or imported.

</p>
</details> 


## 4. Business Value
> Visualizing vessels and operational resources in 3D context provides logistics operators and port authorities with real-time situational awareness of port operations. Seeing vessel positions at berths and resource locations enables quick assessment of operational status, supports coordination between teams, and facilitates identification of potential conflicts or optimization opportunities. This visual intelligence enhances decision-making speed and accuracy in dynamic operational environments.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../../global_docs/def_of_ready.md)

## 6. Definition of Done
- Vessels appear at their assigned berth positions
- Resources (cranes, equipment) are displayed at assigned areas
- Data is fetched from existing REST APIs
- Only resources with assigned areas are displayed
- Only approved vessel visit notifications are shown
- Models are either procedurally created or imported
- The implementation checks all acceptance criteria

