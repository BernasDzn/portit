# US2209 - Change / complete a Vessel Visit Notification 

## 1. User Story Description *(from project statement)*
> As a Shipping Agent Representative, I want to change / complete a Vessel Visit Notification while it is still in progress, so that I can correct errors or withdraw requests if necessary.

## 2. Customer Specifications and Clarifications

<details><summary>From the specifications document</summary>
<p>

> Status can be maintained "in progress" or changed to "submitted / approval pending" by the representative.

</p>
</details>

<details><summary>From the client clarifications</summary>
<p>

> **Q:** Can crew information or other data be changed or added later? Or can only certain information be added/changed?
<br> **A:** While the Vessel Visit Notification status is "in progress" (US 2.2.8 and US 2.2.9), all its data can be changed/added. Once submitted, it can no longer be changed by the Shipping Agent Representative.

> **Q:** In the US, the term "withdraw request" is often used. Could you clarify what this action consists of?
<br> **A:** Under the US 2.2.9, the mention to "withdraw request" refers to the ability of the Shipping Agent Representative to mark a given Vessel Visit Notification as having no intention to complete it til the point of submitting it for approval.
As so, (s)he does not see that Notification as being "in progress" any more. However, the Notification should not be deleted since, occasionally, (s)he may change her/his mind a resume it from there. After being submitted, the Shipping Agent Representative cannot change the Notification.

> **Q:** Should the shipping agent representative who requests to modify or remove a Vessel Visit Notification be allowed to change only the notifications they created, or any notification in the system, regardless of who created it?
<br> **A:** Most of the time, Shipping Agent Representative work on the Vessel Visit Notifications created by themselves.
However, it may be possible to work on Vessel Visit Notifications submitted by other representatives working for the same shipping agent organization.


</p>
</details> 

## 3. Acceptance Criteria:

<details><summary>Show acceptance criteria</summary>
<p>

-  Status can be maintained "in progress" or changed to "submitted / approval pending" by the representative.

</p>
</details> 

## 4. Business Value
> This feature is important to keep the business information updated and progress the lifecycle of berthings and sailings

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../global_artifacts/def_of_ready.md)

## 6. Definition of Done
- The system is able to update the vessel visit notification
- The feature implementation follows the Acceptance Criteria

