import { computed, type Ref } from 'vue';
import type { OperationPlanDto } from '@/model/dto/OperationPlanDto';
import type { STSCrane } from '@/model/PhysicalResource';

export function useOperationValidation(
    plan: Ref<OperationPlanDto | null>,
    allSTSCranes: Ref,
    allStaff: Ref,
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
            const opA = operations[i];
            
            for (let j = i + 1; j < operations.length; j++) {
                const opB = operations[j];
                
                for (let resA of opA.resources) {
                    const resAStart = new Date(resA.startTime || opA.startTime).getTime();
                    const resAEnd = new Date(resA.endTime || opA.endTime).getTime();
                    
                    for (let resB of opB.resources) {
                        // Only check if it's the same resource
                        if (resA.name === resB.name && resA.type === resB.type) {
                            const resBStart = new Date(resB.startTime || opB.startTime).getTime();
                            const resBEnd = new Date(resB.endTime || opB.endTime).getTime();
                            
                            // Check for time overlap
                            if (resAStart < resBEnd && resBStart < resAEnd) {
                                console.warn(
                                    `Resource conflict: ${resA.type} "${resA.name}" is scheduled in both ` +
                                    `operation #${i + 1} and operation #${j + 1} at overlapping times`
                                );
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    };

    const getStaffOperationalWindowWarnings = (): string[] => {
        if (!plan.value) return [];

        const warns: string[] = [];

        plan.value.operationSchedule.forEach((operation, opIndex) => {
            const operationStart = new Date(operation.startTime);
            const operationEnd = new Date(operation.endTime);
            
            operation.resources.forEach((resource) => {
                // Only check staff resources
                if (resource.type !== 'Staff') return;
                
                const staff = allStaff.value.find(s => s.email === resource.name);
                if (!staff) {
                    console.warn(`Staff not found: ${resource.name}`);
                    return;
                }

                // Check if staff has operational window
                if (!staff.operationalWindow?.shifts || staff.operationalWindow.shifts.length === 0) {
                    console.warn(`No operational window for staff: ${resource.name}`);
                    return;
                }

                const shifts = staff.operationalWindow.shifts;
                
                // Use resource-specific times if available, otherwise use operation times
                const checkStartTime = resource.startTime || operation.startTime;
                const checkEndTime = resource.endTime || operation.endTime;

                console.log(`Checking staff ${resource.name} for operation #${opIndex + 1}:`, {
                    checkStartTime,
                    checkEndTime,
                    shifts
                });
                
                const startInShift = shifts.some(shift => 
                    isTimeInShift(checkStartTime, shift)
                );
                const endInShift = shifts.some(shift => 
                    isTimeInShift(checkEndTime, shift)
                );
                
                if (!startInShift || !endInShift) {
                    const startDate = new Date(checkStartTime);
                    const endDate = new Date(checkEndTime);
                    const opTime = `${startDate.toLocaleTimeString([], { 
                        hour: '2-digit', 
                        minute: '2-digit' 
                    })} - ${endDate.toLocaleTimeString([], { 
                        hour: '2-digit', 
                        minute: '2-digit' 
                    })}`;
                    
                    const staffName = staff.name || resource.name;
                    warns.push(
                        `Operation #${opIndex + 1} (${opTime}) assigns staff "${staffName}" outside their working hours`
                    );
                }
            });
        });

        return warns;
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

        // Check for operations outside staff operational windows
        warns.push(...getStaffOperationalWindowWarnings());

        return warns;
    });

    const graveWarnings = computed(() => {
        const warns: string[] = [];
        
        if (!plan.value) return warns;
        
        return warns;
    });

    return {
        warnings,
        graveWarnings,
        isTimeInShift,
        getOperationalWindowWarnings,
        hasOverlappingOperations
    };
}