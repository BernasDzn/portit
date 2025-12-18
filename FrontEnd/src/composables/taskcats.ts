import type { GanttItem, GanttRowConfig } from "@/components/GanttChart.vue";
import type { OperationPlanDto } from "@/model/dto/OperationPlanDto";
import type { STSCrane } from "@/model/PhysicalResource";
import type { Staff } from "@/model/Staff";

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
    
    for (let i = 0; i < plan.operationSchedule.length; i++) {
        let op = plan.operationSchedule[i];
        
        if (!op.startTime || !op.endTime || !op.resources || op.resources.length === 0) {
            continue;
        }
        
        const opColor = operationColorMap[op.type.category] || '#3498db';
        
        op.resources.forEach((resource, resIndex) => {
            const startTime = resource.startTime || op.startTime;
            const endTime = resource.endTime || op.endTime;
            
            // Only apply time constraints to staff resources
            const isStaff = resource.type === 'Staff';
            
            items.push({
                id: `op${i}-res${resIndex}`,
                startTime: startTime,
                endTime: endTime,
                name: `Op. #${i + 1}`,
                group: resource.name || 'Unassigned',
                color: opColor,
                // Only constrain staff members to operation time boundaries
                ...(isStaff && {
                    minTime: op.startTime,
                    maxTime: op.endTime
                })
            });
        });
    }
    
    return items;
};

const getGanttRowConfigs = (
    plan: OperationPlanDto,
    allSTSCranes: STSCrane[],
    allStaff: Staff[]
): GanttRowConfig[] => {
    // Collect all unique resource names
    const resourceNames = new Map<string, number>();
    
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
    
    const colors = ['#3498db', '#e74c3c', '#2ecc71', '#f39c12', '#9b59b6', '#1abc9c'];
    
    return Array.from(resourceNames.keys()).map((name, index) => {
        // Find the resource's operational window
        let operationalWindow = null;
        
        // Check if it's a crane
        const crane = allSTSCranes.find(c => c.code === name);
        if (crane?.operationalWindow) {
            operationalWindow = crane.operationalWindow;
        }
        
        // Check if it's staff
        const staff = allStaff.find(s => s.email === name);
        if (staff?.operationalWindow) {
            operationalWindow = staff.operationalWindow;
        }
        
        return {
            name,
            color: colors[index % colors.length],
            operationalWindow: operationalWindow
        };
    });
};

export function useTaskCategories() {
    return {
        colorMapCategory,
        getGanttItems,
        getGanttRowConfigs
    };
}