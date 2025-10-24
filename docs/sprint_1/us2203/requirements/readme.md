# US2203 - Register and update docks

## 1. User Story Description *(from project statement)*
> As a Port Authority Officer, I want to register and update docks, so that the system accurately reflects the docking capacity of the port. 

## 2. Customer Specifications and Clarifications

<details><summary>From the specifications document</summary>
<p>

> Data needed include: unique identifier, name/number, location within the port, and physical characteristics (e.g., length, depth, max draft), the vessel types allowed to berth there
> Docks must be searchable and filterable by name, vessel type, and location.
</p>
</details>

<details><summary>From the client clarifications</summary>
<p>

> **Q:** As mentioned in the document, the concepts "Dock", "Storage Area", "Shipping Agent Organization" and "Qualifications" have an unique identifier. For instance, is there any example you could give or is it up to us?
<br> **A:** Unless stated otherwise (check other questions/answers), you should consider identifiers/codes as alphanumeric.
Different ports may have distinct conventions to identify these concepts.
Thus, at most, the system must validate the compliance of the inputted identifiers through a configurable regular expression (by concept).


> **Q:** Which fields of a dock are allowed to be updated once it is registered?
<br> **A:** All excepting the id.

> **Q:** Should the system maintain a log of dock updates, recording who made the changes and when?
<br> **A:** Yes, for compliance with the statement "All user interactions must be carefully logged, producing detailed records of every significant action
performed in the system. These logs are not only essential for auditing and traceability but also serve
as an important tool for diagnosing issues and analyzing user behavior." (cf. System Description document, section 3.3)

> **Q:** Regarding the user story for registering and updating a dock, we are not sure what is meant by "location within the port." Should this be stored as geographic coordinates, or as a relative/semantic position (e.g., area, zone) within the port?
<br> **A:** In this case, you may consider the "location within the port" as a free text.


> **Q:** Regarding this user story, can you confirm if a dock supports only one vessel type?
<br> **A:** No! That is clearly wrong.
An acceptance criteria states that "The officer must specify the vessel types allowed to berth there.".
On a given dock may berth several vessel types (e.g. Feeder and Panamax).


</p>
</details> 

## 3. Business Value
> This feature is important because it ensures the port’s docking capacity is accurately managed, allowing vessels to be matched with suitable docks, improving operational efficiency, safety, and planning reliability.

## 4. Definition of Ready
> This US follows the defined [global definition of ready](../../global_artifacts/def_of_ready.md)

## 5. Definition of Done
- Each dock record include a unique identifier, a name/number, a location within the port, and physical characteristics.
- The officer can/must specify the vessel types allowed to berth there.
- Docks are searchable.
- Docks are filterable by name, vessel type, and location.
- Port Authority Officer can register and update dock records.
- Feature tested and reviewed.
