# US3304 - As a System User, I want 3D models to be rendered with appropriate textures or visual styling, so that different port elements (e.g., docks, vessels, storage areas, cranes) are easily distinguishable.

## 1. User Story Description *(from project statement)*
> As a System User, I want 3D models to be rendered with appropriate textures or visual styling, so that different port elements (e.g., docks, vessels, storage areas, cranes) are easily distinguishable.

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

- Each category of 3D object (e.g., vessels, docks, storage areas, cranes) must have distinct textures and materials.
- Regarding procedurally created models, texture and material properties and locations must be retrieved from the backend as JSON-formatted content. Additionally, textures must include at least two maps: a color map and either a roughness map, a bump map, or a normal map.
- Textures or materials must not significantly degrade performance or loading time.

</p>
</details> 


## 4. Business Value
> Distinctive visual styling and textures for different port elements enable rapid visual identification and categorization, reducing cognitive effort required to interpret 3D scenes. Clear visual differentiation between vessels, docks, storage areas, and equipment supports quick situational assessment and reduces the risk of misidentification during operational planning. Professional rendering with appropriate materials enhances system credibility and user engagement while maintaining performance standards.

## 5. Definition of Ready
> This US follows the defined [global definition of ready](../../../global_docs/def_of_ready.md)

## 6. Definition of Done
- Each object category has distinct textures and materials
- Procedural models use textures from backend JSON configuration
- Textures include color map and at least one additional map (roughness/bump/normal)
- Visual styling does not significantly degrade performance
- Different port elements are easily distinguishable
- The implementation checks all acceptance criteria

