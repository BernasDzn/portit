# US 2210 - View Vessel Visit Notification Status

## 1. User Story Description *(from project statement)*
> As a Shipping Agent Representative, I want to view the status of all my submitted Vessel Visit Notifications (in progress, pending, approved with current dock assignment, or rejected with reason), so that I am always informed about the decisions of the Port Authority.

## 2. Customer Specifications and Clarifications

<details><summary>From the specifications document</summary>
<p>

> *"The process begins when a shipping agent representative submits a Vessel Visit notification for an authorized vessel, providing key information such as expected arrival (ETA), departure (ETD), cargo type and volume, and any special handling requirements."*

> *"The Port Authority reviews the notification and decides to approve or reject it. If the visit is rejected, a reason must be provided to the agent..."*

> *"If the visit is approved, a dock is assigned, potentially with support from an intelligent algorithm that considers pending visits, vessel type, dock capacity, and other operational constraints."*

> *"Each shipping agent organization may have multiple representatives authorized to interact with the system on its behalf."*

> *"Shipping agents... can update or cancel notifications before approval, provide required documentation, and monitor the status of approved visits."*

</p>
</details>

<details><summary>From the client clarifications</summary>
<p>

> **Q:** "Boa tarde, quando diz que as Vessel Visit Notifications devem ser filtráveis por tempo, o que é exatamente o tempo? Devemos ter uma data de início e uma data de fim ou devemos ter apenas um tempo respetivo à demora da Vessel Visit?"
<br> **A:** "Searching by time means time period which implies a begin date and end date."

> **Q:** "In the US, the term "withdraw request" is often used. Could you clarify what this action consists of?"
<br> **A:** "Under the US 2.2.9, the mention to "withdraw request" refers to the ability of the Shipping Agent Representative to mark a given Vessel Visit Notification as having no intention to complete it til the point of submitting it for approval."

> **Q:** "When a Shipping agent representative wants to check the status of a Vessel Visit Notification from another representative in the same organization, should he have the possibility of choosing from who he wants to see or just be presented to him all the Vessel Visit Notifications of every representative in the same organization?"
<br> **A:** "According to the US acceptance criteria, "Vessel Visit Notifications must be searchable and filterable by vessel, status, representative and time."
So, you may show all (s)he can view and allow filtering by representative."

</p>
</details> 

## 3. Acceptance Criteria:

<details><summary>Show acceptance criteria</summary>
<p>

- Shipping Agent Representatives can view status of all their submitted Vessel Visit Notifications
- Status options include: in progress, pending, approved (with dock assignment), rejected (with reason)
- Representatives can view Vessel Visit Notifications submitted by other representatives from the same shipping agent organization
- Vessel Visit Notifications must be searchable and filterable by:
  - Vessel name or IMO number
  - Status (all status types)
  - Representative name
  - Time range (submission date, ETA, ETD)
- System displays dock assignment details for approved visits
- System displays rejection reasons for rejected visits
- Historical archive of all previous interactions is maintained and accessible

</p>
</details> 

## 4. Business Value
> This feature is important because a shipping agent should be able to have an archive of its history with our business in case any problem happens, and to be informed of our decisions. It enhances transparency, improves communication, and provides audit capability for both the shipping agent and port authority.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../global_artifacts/def_of_ready.md)

## 6. Definition of Done
- The system displays an archive in any human intuitive format of the shipping agent's previous interactions with our port authority
- Shipping Agent Representatives can successfully view and filter all VVN statuses
- Organization-wide visibility is properly implemented (same shipping agent representatives)
- Search and filter functionality works for all specified criteria
- Dock assignment information is displayed for approved visits
- Rejection reasons are clearly displayed for rejected visits
- All data is retrieved from persistent storage
- Access control ensures users only see authorized VVNs

## 7. Dependencies
- (Vessel Visit Notification Submission)
- (Port Authority VVN Review & Approval)
- (Vessel Management)
- (Dock Management)

## 8. Notes
N/A