# US3405 - As a Logistics Operator, I want the scheduling module to support the use of multiple cranes when a single-crane solution cannot eliminate vessel departure delays, so that total delay is minimized while using additional cranes only when strictly necessary.

## 1. User Story Description *(from project statement)*
> As a Logistics Operator, I want the scheduling module to support the use of multiple cranes when a single-crane solution cannot eliminate vessel departure delays, so that total delay is minimized while using additional cranes only when strictly necessary.

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

- The system must first attempt to generate a schedule using a single-crane allocation strategy.
- If the computed schedule still results in non-zero departure delays, the module must (automatically or optionally) re-evaluate the plan allowing multiple cranes per vessel.
- The multi-crane scheduling approach must aim to:
  - Minimize the total sum of vessel departure delays.
  - Minimize the additional time windows where more than one crane is required (i.e., minimize multi-crane usage intensity).
- The output must clearly indicate where and when additional cranes were allocated to meet schedule objectives.
- The operator must be able to compare results between single-crane and multi-crane strategies via summary metrics (e.g., total delay, number of crane-hours used).
- At this stage, results do not need to be persisted anywhere— they can be recomputed on demand.

</p>
</details> 


## 4. Business Value
> ...

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../../global_docs/def_of_ready.md)

## 6. Definition of Done
- The implementation checks all acceptance criteria.

