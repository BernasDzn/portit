# US2211 - Register and manage operating staff members (create, update, deactivate) 


### Description
> As a Logistics Operator, I want to register and manage operating staff members (create, update, deactivate), so that the system can accurately reflect staff availability and ensure that only qualified personnel are assigned to resources during scheduling.

### Acceptance Criteria:
- Each staff member must have a unique mecanographic number (ID), short name, contact details (email, phone), qualifications, operational window, and current status (e.g., available, unavailable).
- Deactivation/reactivation must not delete staff data but preserve it for audit and historical
planning purposes.
- Staff members must be searchable and filterable by id, name, status, and qualifications.

###  Business Value
This feature is important as to ensure accurate, efficient scheduling and compliance by ensuring only qualified and available personnel are considered for operations.

### Definition of Done
- The system allows for the authorized users to create a staff member
- The system allows for the authorized users to update a staff member
- The system allows for the authorized users to deactivate a staff member
- The changes made are effectivated on persistent storage

### Dependencies
- The System is able to register qualifications

### Notes
- N/A
