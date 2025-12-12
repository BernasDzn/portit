import type { GanttItem, GanttRowConfig } from "@/components/GanttChart.vue";
import type { OperationPlanDto } from "@/model/dto/OperationPlanDto";
import type TaskCategoryDto from "@/model/dto/TaskCategoryDto";

const categoryColorMap = {
    'LOAD': '#27ae60',
    'UNLOAD': '#c0392b',
};

function colorMapCategory(cat: TaskCategoryDto) {
    return categoryColorMap[cat.category.value] || 'primary';
}

const getGanttItems = (plan: OperationPlanDto): GanttItem[] => {
    const items: GanttItem[] = [];
    
    console.log('Generating Gantt items from operation plan:', plan);
//    plan.operationSchedule.forEach((op, opIndex) => {
    for (let i = 0; i < plan.operationSchedule.length; i++) {
        let op = plan.operationSchedule[i];

        if (!op.startTime || !op.endTime || !op.resources || op.resources.length === 0) {
            return;
        }
        
        const opColor = colorMapCategory(op.type);
        
        // Create an item for each resource in this operation
        op.resources.forEach((resource, resIndex) => {
            items.push({
                id: `op${i}-res${resIndex}`,
                startTime: op.startTime,
                endTime: op.endTime,
                name: `${op.type.category.value} Operation`,
                group: resource.name || 'Unassigned',
                color: opColor
            });
        });
    }

    console.log();
    
    return items;
};

const getGanttRowConfigs = (plan: OperationPlanDto): GanttRowConfig[] => {
    // Collect all unique resource names
    const resourceNames = new Set<string>();
    
    plan.operationSchedule.forEach(op => {
        op.resources.forEach(res => {
            resourceNames.add(res.name || 'Unassigned');
        });
        if (op.resources.length === 0) {
            resourceNames.add('Unassigned');
        }
    });
    
    // Create a row config for each resource with alternating colors
    const colors = ['#3498db', '#e74c3c', '#2ecc71', '#f39c12', '#9b59b6', '#1abc9c'];

    console.log(resourceNames);
    
    return Array.from(resourceNames).map((name, index) => ({
        name,
        color: colors[index % colors.length]
    }));
};

export function useTaskCategories() {
    return {
        colorMapCategory,
        getGanttItems,
        getGanttRowConfigs
    };
}