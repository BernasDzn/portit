# US 2204 - Manage storage areas

## 1. User Story Description *(from project statement)*
> As a Port Authority Officer, I want to register and update storage areas, so that (un)loading and storage operations can be assigned to the correct locations.

## 2. Customer Specifications and Clarifications

<details><summary>From the specifications document</summary>
<p>

> Each storage area must have a unique identifier, type (e.g., yard, warehouse), and location within the port.

> Storage areas must specify maximum capacity (in TEUs) and current occupancy.

> By default, a storage area serves the entire port (i.e., all docks). However, some storage areas (namely yards) may be constrained to serve only a few docks, usually the closest ones.

> Complementary information, such as the distance between docks and storage areas, must be manually recorded to support future logistics planning and optimization.

> Updates to storage areas must not allow the current occupancy to exceed maximum capacity.

</p>
</details>

<details><summary>From the client clarifications</summary>
<p>

> **Q:** It is stated in the assignment that we must keep track of the capacity and occupancy of storage areas such as yards and warehouses: However, is the specific layout of said storage (in rows, bays, and tiers) also relevant?<br><br>If said layout is relevant, do we assume that these elements are organized in consistent grids (as in, the same amount of tiers across all rows and bays, the same amount of bays across all rows and tiers, etc.), or can there be areas with more or less tiers, bays, etc.?
<br> **A:** If the storage area serves all docks, you need to know those distances.

> **Q:** Is it necessary to keep the distance between storage areas?
<br> **A:** By the moment, that is not necessary.

</p>
</details> 

## 3. Acceptance Criteria:

<details><summary>Show acceptance criteria</summary>
<p>

- Each storage area must have a unique identifier, type (e.g., yard, warehouse), and location within the port.
- Storage areas must specify maximum capacity (in TEUs) and current occupancy.
- By default, a storage area serves the entire port (i.e., all docks). However, some storage areas (namely yards) may be constrained to serve only a few docks, usually the closest ones.
- Complementary information, such as the distance between docks and storage areas, must be manually recorded to support future logistics planning and optimization.
- Updates to storage areas must not allow the current occupancy to exceed maximum capacity

</p>
</details> 


## 4. Business Value
> This feature is important because it ensures storage areas are accurately registered and managed, enabling correct assignment of (un)loading operations, preventing overcapacity issues, and supporting efficient logistics planning within the port.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../global_artifacts/def_of_ready.md)

## 6. Definition of Done
- When registering/updating a storage area the specified fields are mandatory (unique identifier, type, location, maximum capacity, current occupancy and complementary information).
- When registering/updating a storage area the specified validations are implemented (e.g., ensure maximum capacity is in TEUs).
- Port Authority Officer can register and update storage areas.
- Feature tested and reviewed.
