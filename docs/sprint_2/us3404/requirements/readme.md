# US3404 - As a Logistics Operator, I want an alternative scheduling algorithm for the loading and unloading operations of vessels arriving at the port on a given day, that produces a good (but not necessarily optimal) solution efficiently, so that the system can handle larger problem instances or time-constrained planning scenarios.

## 1. User Story Description *(from project statement)*
> As a Logistics Operator, I want an alternative scheduling algorithm for the loading and unloading operations of vessels arriving at the port on a given day, that produces a good (but not necessarily optimal) solution efficiently, so that the system can handle larger problem instances or time-constrained planning scenarios.

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

- This algorithm must be available for selection on the dedicated interface of the SPA and reuse the same data inputs and interfaces defined for the existing scheduling module.
- This algorithm must aim to minimize vessel departure delays but prioritize computational efficiency over optimality. Suitable approaches may include greedy strategies, local search, or other informed heuristics.
- Results must be comparable (e.g., total delay, computation time) against the previous algorithm using summary metrics.
- At this stage, results do not need to be persisted anywhere— they can be recomputed on demand.

</p>
</details> 


## 4. Business Value
> Alternative heuristic scheduling algorithms provide fast, good-quality solutions for large-scale or time-critical scenarios where optimal algorithms are too slow. By trading guaranteed optimality for computational efficiency, heuristics enable responsive scheduling even during peak operational periods or when planning involves many vessels and resources. Comparative metrics between optimal and heuristic solutions help operators choose appropriate algorithms based on operational urgency and solution quality requirements.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../../global_docs/def_of_ready.md)

## 6. Definition of Done
- Alternative heuristic algorithm is implemented and selectable in SPA
- Algorithm reuses same data inputs and interfaces as optimal algorithm
- Algorithm prioritizes computational efficiency over optimality
- Results include comparison metrics (total delay, computation time)
- The implementation checks all acceptance criteria

