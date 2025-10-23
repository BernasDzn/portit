# US 2213 - Register and manage qualifications 

## 1. User Story Description *(from project statement)*
> As a Logistics Operator, I want to register and manage qualifications (create, update), so that staff members and resources can be consistently associated with the correct skills and certifications required for port operations.

## 2. Customer Specifications and Clarifications

<details><summary>From the specifications document</summary>
<p>

> *"Qualification: Each staff member is registered with the qualifications they hold (e.g., STS crane operator, yard gantry cranes operator, truck driver, yard planner). Certain resources may only be operated by staff with matching qualifications (e.g., an STS crane requires a certified STS crane operator)."*

> Each qualification has a unique code and a descriptive name (e.g., "STS Crane Operator,"Truck Driver").

> Qualifications must be searchable and filterable by code or name.

> A qualification must exist before it can be assigned to staff members or resources.

</p>
</details>

<details><summary>From the client clarifications</summary>
<p>

> **Q:** Could you please clarify the business rules for qualifications’ codes and names? Specifically, is the code format always predefined (e.g., Q-000) or should users be free to choose any format? Also, can different qualifications share the same name but have different codes, or must names be unique as well? Finally, should all codes follow the same pattern, or can multiple formats coexist?
<br> **A:** Let's keep it simple:<br>- Code: alphanumeric, with a maximum length of 15;<br> - Description: free text with at least two words and a maximum length of 150.

> **Q:** In relation to the update action for qualifications, a question has arisen: when performing an update, is it possible to change the code, the name, or both?
<br> **A:** Both are updatable. However, you need to ensure that:<br> 1. The code remains unique;<br> 2. Existing relationships of either resources and/or of staff to qualifications must remain valid.

</p>
</details> 

## 3. Acceptance Criteria:

<details><summary>Show acceptance criteria</summary>
<p>

- Each qualification has a unique code and a descriptive name (e.g., "STS Crane Operator,"
"Truck Driver").
- Qualifications must be searchable and filterable by code or name.
- A qualification must exist before it can be assigned to staff members or resources.

</p>
</details> 


## 4. Business Value
> This feature is important because knowing the qualifications of each staff member allows for smarter mobilizing of man-power for specific tasks.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../global_artifacts/requirements/def_of_ready.md).

## 6. Definition of Done
- The system is able to create a qualification
- The system is able to update a qualification
- The changes made are effectivated on persistent storage
- The CRUD operations follow the Acceptance Criteria
