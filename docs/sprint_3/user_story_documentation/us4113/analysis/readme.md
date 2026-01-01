# US4113 - As a Logistics Operator, I want to record and manage incidents that affect the execution of port operations, so that delays and operational disruptions can be accurately tracked, scoped, and analyzed.

### Domain Concepts
- **Incident**: contains a unique ID, reference to an Incident Type, severity, description, responsible user, affected VVEs, and a time window (start/end). Active incidents have a null `endTime`.
- **Incident Type**: links to US4112 and provides the hierarchical classification used for filtering and severity interpretation.
- **Impact Scope**: allows marking whether an incident affects all ongoing VVEs, specific ones, or upcoming VVNs for the same day.

### Business Rules
1. CRUD operations guard against inconsistent data, ensuring mandatory associations (Incident Type, severity, start time).
2. Active incidents (no `endTime`) should be easily identifiable in the SPA and highlight any ongoing operations they impact.
3. Any incident resolution (i.e., setting `endTime`) automatically computes duration for reporting.
4. Linking/detaching VVEs updates their incident reference lists, enabling quick context changes.
