import type { GanttItem, GanttRowConfig } from "@/components/GanttChart.vue";
import type { OperationPlanDto } from "@/model/dto/OperationPlanDto";

const categoryColorMap = {
    'LOAD': 'success',
    'UNLOAD': 'warning',
};

const operationColorMap = {
    'LOAD': '#27ae60',
    'UNLOAD': '#c0392b',
};

function colorMapCategory(cat: string) {
    return categoryColorMap[cat] || 'primary';
}

const getGanttItems = (plan: OperationPlanDto): GanttItem[] => {
    const items: GanttItem[] = [];
    
    console.log('Generating Gantt items from operation plan:', plan);
    
    for (let i = 0; i < plan.operationSchedule.length; i++) {
        let op = plan.operationSchedule[i];
        
        if (!op.startTime || !op.endTime || !op.resources || op.resources.length === 0) {
            continue; // Use continue instead of return to skip this operation
        }
        
        const opColor = operationColorMap[op.type.category] || '#3498db';
        
        // Create an item for each resource in this operation
        op.resources.forEach((resource, resIndex) => {
            // Use resource-specific times if they exist, otherwise fall back to operation times
            const startTime = resource.startTime || op.startTime;
            const endTime = resource.endTime || op.endTime;
            
            items.push({
                id: `op${i}-res${resIndex}`,
                startTime: startTime,
                endTime: endTime,
                name: `Op. #${i + 1}`,
                group: resource.name || 'Unassigned',
                color: opColor
            });
        });
    }
    
    console.log('Generated Gantt items:', items);
    
    return items;
};

const getGanttRowConfigs = (plan: OperationPlanDto): GanttRowConfig[] => {
    // Collect all unique resource names in the order they appear
    const resourceNames = new Map<string, number>(); // name -> first appearance index
    
    plan.operationSchedule.forEach((op, opIndex) => {
        op.resources.forEach(res => {
            const name = res.name || 'Unassigned';
            if (!resourceNames.has(name)) {
                resourceNames.set(name, opIndex);
            }
        });
        if (op.resources.length === 0) {
            if (!resourceNames.has('Unassigned')) {
                resourceNames.set('Unassigned', opIndex);
            }
        }
    });
    
    // Create a row config for each resource with consistent colors
    const colors = ['#3498db', '#e74c3c', '#2ecc71', '#f39c12', '#9b59b6', '#1abc9c'];
    
    console.log('Resource names for rows:', Array.from(resourceNames.keys()));
    
    return Array.from(resourceNames.keys()).map((name, index) => ({
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