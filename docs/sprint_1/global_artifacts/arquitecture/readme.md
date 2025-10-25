# Views
This section documents the system architecture using two modeling approaches: the C4, and the 4+1 architectural view model.
The adopted architectural frameworks are:
- [C4 Model](https://c4model.com/)
- [4 + 1 Model](https://en.wikipedia.org/wiki/4%2B1_architectural_view_model)

The levels are structured with the following logic:
1. Description of the system as a whole
2. Descpription of each macro container of the system
3. Description of all the components of each container
4. Granular description of the code 

We have decided to include only 4 levels to our documentation. Not all levels are fully documented, for reasons that should become apparent later, but these are the views we've decided were worthy of generating:
1. Level 1
    - Logic View
    - Process View
    - Scenario View
2. Level 2
    - Logic View
    - Implementation
3. Level 3
    - Logic View
    - Physical View
    - Process View
4. Level 4
    - Logic View
    - Process View
    - Implementation View

## Views

### Level 1
#### Logic View
![N1_Logic_0](./level_1/logic_views/svg/l1_logicview/logicview_n1.svg)
#### Process View
![N1_Process_0](./level_1/process_views/svg/l1_process_view_delete/processview_n1.svg)
![N1_Process_1](./level_1/process_views/svg/l1_process_view_get/processview_n1.svg)
![N1_Process_2](./level_1/process_views/svg/l1_process_view_post/processview_n1.svg)
![N1_Process_3](./level_1/process_views/svg/l1_process_view_put/processview_n1.svg)
#### Scenario View
![N1_Scenario_0](./level_1/scenario_views/svg/l1_scenario_view/use_case_diagram.svg)

### Level 2
#### Logic View
![N2_Logic](./level_2/logic_views/svg/l2_logic_view/logicview_n4.svg)
#### Implementation View
![N2_Implementation](./level_2/implementation_view/svg/l2_implementation_view/logicview_n4.svg)
### Level 3
#### Logic View
![N3_Logic_0](./level_3/logic_views/svg/l3_logic_view/logicview_n4.svg)
#### Physical View
![N3_Phys](./level_3/physical_views/svg/l3_physical_view/logicview_n4.svg)
#### Process View
![N3_Process_0](./level_3/process_views/svg/l3_process_view_get/sequence_diagram_get.svg)
![N3_Process_1](./level_3/process_views/svg/l3_process_view_post/sequence_diagram_post.svg)
![N3_Process_2](./level_3/process_views/svg/l3_process_view_put/sequence_diagram_put.svg)
![N3_Process_3](./level_3/process_views/svg/l3_process_view_delete/sequence_diagram_delete.svg)
![N3_Process_4](./level_3/process_views/svg/l3_process_view_filter/sequence_diagram_filter.svg)

### Level 4
#### Logic View
![N4_Logic_0](./level_4/logic_views/svg/l4_logic_view_1/logicview_n4.svg)
#### Process View
![N4_Process_0](./level_4/process_views/svg/l4_process_view_get/sequence_diagram_get.svg)
![N4_Process_1](./level_4/process_views/svg/l4_process_view_post/sequence_diagram_post.svg)
![N4_Process_2](./level_4/process_views/svg/l4_process_view_put/sequence_diagram_put.svg)
![N4_Process_3](./level_4/process_views/svg/l4_process_view_delete/sequence_diagram_delete.svg)
![N4_Process_4](./level_4/process_views/svg/l4_process_view_filter/sequence_diagram_filter.svg)
#### Implementation View
![N4_Impl_0](./level_4/implementation_views/svg/l4_implementation_view_1/logicview_n4.svg)

## Mapping between views
![N2_Implementation_Logic](./level_2/mapping_view/svg/l2_implementation_logic_mapping_view/mapping_view_n2.svg)
![N3_Logical_Physical](./level_3/mapping_view/svg/l3_logic_physical_view/mapping_view_n3.svg)
![N4_Implementation_Logic](./level_4/mapping_view/svg/l4_implementation_logic_mapping_view/mapping_view_n4.svg)