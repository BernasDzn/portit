# Glossary

## Abbreviations
- **Shipping Agent Organization**: SAO
- **Shipping Agent Organization Representative**: SAOR
- **Vessel Visit Notification**: VVN
- **Port Authority**: PA
- **Logistics Operator**: LO

## Business Terms
|            Term             | Classification | Business definition                                                                                                                                                      |
| :-------------------------: | :------------: | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
|        Administrator        |     Actor      | Responsible for managing user accounts, permissions, system parameters, and maintaining operational data integrity.                                                      |
|       Cargo Manifest        |     Value      | A structured document listing containers to be unloaded (Unloading Manifest) or loaded (Loading Manifest), including their IDs, contents, and positions on the vessel.   |
|       Cargo Transport       |     Value      | Information detailing the movement of a container, including its origin (e.g., vessel bay/row/tier) and destination (e.g., specific yard location or warehouse).         |
|         Cargo Type          |     Value      | A descriptor categorizing the contents of a container (e.g., refrigerated goods, general products, hazardous materials, electronics).                                    |
|          Container          |     Entity     | A standardized unit for cargo, identified by an ISO 6346:2022 code, stored in a grid structure on vessels (bays, rows, tiers).                                           |
|      Container Number       |     Value      | The unique code identifying a specific container, conforming to the ISO 6346:2022 standard (e.g., owner code, category ID, serial number, check digit).                  |
|       Container Yards       |  Entity Class  | Designated areas for the temporary storage of containers within the port.                                                                                                |
|            Crew             |     Value      | The personnel aboard a vessel; for most visits, this includes the captain's name and total number of crew members.                                                       |
|            Dock             |     Entity     | A berthing facility (quay) where vessels moor for loading/unloading operations, equipped with a specific number of STS cranes.                                           |
|      Docking Schedule       |     Entity     | The planned assignment of a vessel to a specific dock for its visit, considering vessel type, cargo volume, and dock capacity.                                           |
|         IMO Number          |     Value      | Unique identifier of a vessel                                                                                                                                            |
|     Logistics Operator      |     Actor      | Responsible for defining, scheduling, and monitoring operational tasks (loading/unloading), and allocating resources (cranes, trucks, staff) for approved Vessel Visits. |
|    Mechanographic number    |     Value      | A unique international identifier assigned to a vessel by the International Maritime Organization                                                                        |
|     Operational Window      |     Value      | The defined periods of availability for resources or staff across the week, reflecting shifts, maintenance, or contractual limits.                                       |
|  Physical Characteristics   |     Value      | The attributes of a dock, such as its length, depth, and the number of STS cranes available.                                                                             |
|     Physical Resources      |     Entity     | Equipment such as cranes, trucks, and terminal tractors used in port operations, each with attributes like operational window, capacity, and status.                     |
|      Port Authorities       |     Actor      | Responsible for reviewing/approving Vessel Visits, assigning docks, managing shipping agent registrations, and overseeing compliance with port regulations.              |
|        Qualification        |     Entity     | Certifications or skills held by staff (e.g., STS crane operator, truck driver) that determine which resources they are authorized to operate.                           |
|       Safety Officers       |     Value      | Designated crew members responsible for safety, required to be identified in the Vessel Visit Notification when the vessel carries dangerous cargo.                      |
|     SAO Representative      |     Actor      | An individual authorized by a Shipping Agent Organization to interact with the system and submit Vessel Visit notifications.                                             |
|    Ship-To-Shore cranes     |  Entity Class  | Large fixed cranes installed at docks for loading/unloading containers directly from vessels.                                                                            |
| Shipping Agent Organization |     Actor      | An organization representing vessel owners/operators, authorized by the Port Authority to submit Vessel Visit notifications on behalf of vessels.                        |
|            Staff            |     Entity     | Human resources (operating staff) with qualifications, availability windows, and status, required to operate physical resources like cranes and trucks.                  |
|        Storage Areas        |     Entity     | General term for areas where cargo is stored, including container yards and warehouses.                                                                                  |
|         System user         |     Actor      | Any individual interacting with the system, categorized by roles such as Port Authority Officer, Shipping Agent Representative, Logistics Operator, or Administrator.    |
|         Task Types          |     Value      | A descriptor categorizing an operational task (e.g., unloading from vessel, transport to yard, stacking in yard, loading onto vessel).                                   |
|            Tasks            |     Entity     | Operational activities such as unloading/loading containers, moving cargo, sequenced and allocated to resources and staff during a Vessel Visit.                         |
| Trucks & Terminal tractors  |  Entity Class  | Vehicles used for transporting containers between docks, yards, and warehouses.                                                                                          |
|        Vessel Types         |     Entity     | Categories of container-carrying vessels (e.g., Feeder, Panamax, Post-Panamax, ULCV) defined by size, capacity, and operational requirements.                            |
|  Vessel Visit Notification  |     Entity     | A notification submitted by a Shipping Agent for a vessel's planned arrival/departure, including ETA, ETD, cargo details, and basic crew information.                    |
|           Vessels           |     Entity     | A ship identified by a unique IMO number, with type, size, and cargo capacity influencing its operational needs at the port.                                             |
|        VVN Decision         |     Value      | The Port Authority's decision to either approve or reject a submitted Vessel Visit Notification, with a reason provided in case of rejection.                            |
|         Warehouses          |  Entity Class  | Facilities within the port for cargo requiring additional handling, inspection, or storage.                                                                              |
|     Yard Gantry cranes      |  Entity Class  | Mobile cranes used for stacking and moving containers within container yards.                                                                                            |
