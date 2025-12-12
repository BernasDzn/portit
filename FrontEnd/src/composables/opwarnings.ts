import { computed, type Ref } from 'vue';
import type { OperationPlanDto } from '@/model/dto/OperationPlanDto';
import type { STSCrane } from '@/model/PhysicalResource';

export function useOperationValidation(
    plan: Ref<OperationPlanDto | null>,
    allSTSCranes: Ref
) {

    const isTimeInShift = (
        dateTime: string,
        shift: { day: number; startTime: string; endTime: string }
    ): boolean => {
        const date = new Date(dateTime);
        const dayOfWeek = date.getDay();
        
        if (dayOfWeek !== shift.day) return false;
        
        const timeStr = date.toTimeString().split(' ')[0]; // Get HH:MM:SS
        return timeStr >= shift.startTime && timeStr <= shift.endTime;
    };

    const getOperationalWindowWarnings = (): string[] => {
        if (!plan.value) return [];

        const warns: string[] = [];

        plan.value.operationSchedule.forEach((operation, index) => {
            const operationStart = new Date(operation.startTime);
            const operationEnd = new Date(operation.endTime);
            
            operation.resources.forEach((resource) => {
                const crane = allSTSCranes.value.find(c => c.code === resource.name);
                
                if (crane && crane.operationalWindow?.shifts) {
                    const shifts = crane.operationalWindow.shifts;
                    
                    const startInShift = shifts.some(shift => 
                        isTimeInShift(operation.startTime, shift)
                    );
                    const endInShift = shifts.some(shift => 
                        isTimeInShift(operation.endTime, shift)
                    );
                    
                    if (!startInShift || !endInShift) {
                        const opTime = `${operationStart.toLocaleTimeString([], { 
                            hour: '2-digit', 
                            minute: '2-digit' 
                        })} - ${operationEnd.toLocaleTimeString([], { 
                            hour: '2-digit', 
                            minute: '2-digit' 
                        })}`;
                        
                        warns.push(
                            `Operation #${index + 1} (${opTime}) uses resource "${resource.name}" outside its operational window`
                        );
                    }
                }
            });
        });

        return warns;
    };

    const hasOverlappingOperations = (): boolean => {
        if (!plan.value) return false;

        const operations = plan.value.operationSchedule;

        for (let i = 0; i < operations.length; i++) {
            const opAStart = new Date(operations[i].startTime).getTime();
            const opAEnd = new Date(operations[i].endTime).getTime();

            for (let j = i + 1; j < operations.length; j++) {
                const opBStart = new Date(operations[j].startTime).getTime();
                const opBEnd = new Date(operations[j].endTime).getTime();

                // Check for overlap: A starts before B ends AND B starts before A ends
                if (opAStart < opBEnd && opBStart < opAEnd) {
                    return true;
                }
            }
        }

        return false;
    };

    const warnings = computed(() => {
        const warns: string[] = [];
        if (!plan.value) return warns;

        // Check for operations outside resource operational windows
        warns.push(...getOperationalWindowWarnings());

        // Check for overlapping operations
        if (hasOverlappingOperations()) {
            warns.push('Some operations have overlapping schedules.');
        }

        return warns;
    });

    return {
        warnings,
        isTimeInShift,
        getOperationalWindowWarnings,
        hasOverlappingOperations
    };
}