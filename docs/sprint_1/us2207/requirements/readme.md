# US2207 - Review pending Vessel Visit Notifications and approve or reject them

## 1. User Story Description *(from project statement)*
> As a Port Authority Officer, I want to review pending Vessel Visit Notifications and approve or reject them, so that docking schedules remain under port control.

## 2. Customer Specifications and Clarifications

<details><summary>From the specifications document</summary>
<p>

> Data needed when approved include: assigned (temporarily) dock, date of the decision
> Data needed when rejected include: reason for rejection (e.g., information is missing), indicator if the decision is permantent or not, date of the decision

</p>
</details>

<details><summary>From the client clarifications</summary>
<p>

> **Q:** Knowing that a Vessel Visit Notification (VVN) can be modified after being rejected (and this may happen multiple times for the same VVN), is it necessary for the system to store the information of the VVN for each update stage it goes through? I am not referring to the log with timestamp, officer ID, and decision outcome for auditing purposes, but rather to the VVN’s actual information, so that in the case of historical review, one can know the VVN’s details at each of its stages.
> 
<br> **A:** It would be a nice feature.
I'll only consider it later, as a system improvement.

</p>
</details> 

## 3. Business Value
> This feature is important because it keeps vessel docking under port authority control, prevents scheduling conflicts, ensures clear communication with agents, and provides an auditable decision trail.

## 4. Definition of Ready
> This US follows the defined [global definition of ready](../../global_artifacts/def_of_ready.md)

## 5. Definition of Done
- Officer can approve with dock assignment.
- Officer can reject with mandatory reason.
- Agents can update and resubmit rejected notifications.
- All decisions are logged with timestamp, officer ID, and outcome.
- Feature tested and reviewed.
