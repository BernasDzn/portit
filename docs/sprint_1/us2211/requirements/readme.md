# US 2211 - Register and manage operating staff members

## 1. User Story Description *(from project statement)*
> As a Logistics Operator, I want to register and manage operating staff members (create, update, deactivate), so that the system can accurately reflect staff availability and ensure that only qualified personnel are assigned to resources during scheduling.

## 2. Customer Specifications and Clarifications

<details><summary>From the specifications document</summary>
<p>

> Data needed include: mecanographic number, short name, email, phone, operational window*, qualifications*, current status*.

> Operational Window: Like physical resources, operating staff have weekly schedules that define their availability (e.g., “Monday–Friday, 08:00–16:00”).

> Qualification: Each staff member is registered with the qualifications they hold (e.g., STS crane operator, yard gantry cranes operator, truck driver, yard planner). Certain resources may only be operated by staff with matching qualifications (e.g., an STS crane requires a certified STS crane operator).

> Current Status: Staff may be marked as available, unavailable (on leave, training), or temporarily reassigned.

</p>
</details>

<details><summary>From the client clarifications</summary>
<p>

> **Q:** What's the format for the mechanographic number ?
<br> **A:** No specific format but must be Alphanumeric only.

> **Q:** How do we represent the operational window? In other words, do we have worker availability and calculate shifts from there, or do we have predefined shifts that workers perform, and then calculate the operational window from there?
<br> **A:** Well, there are different forms (techniques) of representation. Adopt the one that is most interesting...
Examples of the information to be captured can help with the decision.<br><br>
Example 1:
<br>\- Monday: from 7:00 AM to 10:00 AM; from 10:30 AM to 1:30 PM; from 3:00 PM to 6:00 PM;
<br>\- Tuesday: from 7:00 AM to 10:00 AM; from 10:30 AM to 1:30 PM; from 3:00 PM to 6:00 PM;
<br>\- Wednesday: from 9:30 AM to 12:30 PM; from 2:00 PM to 5:00 PM;
<br>\- Thursday: from 2:00 PM to 5:00 PM; from 5:30 PM to 8:30 PM;
<br>\- Friday: from 2:00 PM to 5:00 PM; from 5:30 PM to 8:30 PM;
<br>\- Saturday: from 9:30 AM to 12:30 PM;
<br>\- Sunday: n/a;
<br><br>
Example 2:
<br>\- Monday: from 1 PM to 5 PM; from 6:00 PM to 9:00 PM;
<br>\- Tuesday: from 1 PM to 5 PM; from 6:00 PM to 9:00 PM;
<br>\- Wednesday: n/a;
<br>\- Thursday: from 1 PM to 5 PM; from 6:00 PM to 9:00 PM;
<br>\- Friday: from 1 PM to 5 PM; from 6:00 PM to 9:00 PM;
<br>\- Saturday: from 1 PM to 5 PM; from 6:00 PM to 9:00 PM;
<br>\- Sunday: n/a;

> **Q:** When updating a staff member, can all previously entered information be modified?
<br> **A:** The mecanographic number cannot be modified. Everything else might be modified.

> **Q:** Is it possible to leave a staff member’s record incomplete — for example, register some information now and complete the rest later — given that this could occur with the update action?
<br> **A:**  Mandatory information comprehends, the mecanographic number , short name, contacts and status.

> **Q:** When a staff member is registered, do they automatically become available (with the status "available" )?
<br> **A:**  When registering a staff member, (s)he must be, by default, available.

> **Q:** When does the action of changing a staff member’s status take place? For example, the statement mentioned the statuses on leave and in training. In what situations would someone change a staff member’s status? Should this be addressed in this sprint?
<br> **A:** The user - logistic operator - may choose. 

</p>
</details> 

## 3. Acceptance Criteria:

<details><summary>Show acceptance criteria</summary>
<p>

- Given an authenticated Logistics Operator, When they create a staff member with valid mecanographic number, short name, at least one contact, qualifications, operational window, and optional status, Then the system persists the staff member and returns HTTP 201 Created.
- Given a create request without an explicit status, When the staff member is created, Then the stored status defaults to "available".
- Given a create request with a mecanographic number containing non-alphanumeric characters, When the request is validated, Then the system returns HTTP 400 Bad Request with a validation error for mecanographic number format.
- Given a staff member already exists with mecanographic number "X", When another create request uses the same mecanographic number "X", Then the system returns HTTP 409 Conflict indicating duplicate mecanographic number.
- Given a create request missing the mecanographic number, When the request is validated, Then the system returns HTTP 400 Bad Request indicating the missing mandatory field.
- Given a create or update request with an invalid email format, When the request is validated, Then the system returns HTTP 400 Bad Request with a descriptive validation error for email.
- Given a create or update request that includes empty qualification entries, When the request is validated, Then the system returns HTTP 400 Bad Request indicating invalid qualifications.
- Given an operational window is provided in create or update, When the operational window is malformed (not following the agreed weekday->timeslot schema), Then the system returns HTTP 400 Bad Request with an operational window validation error.
- Given a staff member exists with mecanographic number "Y", When the operator updates allowed fields (short name, contacts, qualifications, operational window, status), Then the changes are persisted and the system returns HTTP 200 OK with the updated representation.
- Given a staff member exists with mecanographic number "Y", When an update attempts to change mecanographic number to a different value, Then the system rejects the change with HTTP 400/422 and leaves the original mecanographic number unchanged.
- Given a staff member is deactivated by the operator, When the deactivation is performed, Then the system preserves the full staff record (soft-delete or status change), records audit metadata (who/when), and returns HTTP 200 OK.
- Given staff members include deactivated records, When a query for active staff is executed, Then deactivated staff are excluded unless the query explicitly requests inactive/deactivated records.
- Given many staff members exist, When the operator searches or filters by mecanographic number (exact), short name (case-insensitive partial, min 3 chars), status, and/or qualifications, Then the system returns a paginated list of matching staff with HTTP 200 OK and a default page size of 20.

</p>
</details> 


## 4. Business Value
> This feature is important as to ensure accurate, efficient scheduling and compliance by ensuring only qualified and available personnel are considered for operations.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../global_artifacts/requirements/def_of_ready.md).

## 6. Definition of Done
- The system is able to create a staff member
- The system is able to update a staff member
- The system is able to deactivate a staff member
- The changes made are effectivated on persistent storage
- The CRUD operations follow the Acceptance Criteria

