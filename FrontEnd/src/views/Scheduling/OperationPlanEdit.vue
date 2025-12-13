<script setup lang="ts">
import { useRoute, RouterLink } from 'vue-router';
import { useAlerts } from '@/composables/alerts';
import { ref, onMounted, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IOperationPlanService } from '@/service/IService/IOperationPlanService';
import type { IStaffService } from '@/service/IService/IStaffService';
import type { IPhysicalResourceService } from '@/service/IService/IPhysicalResourceService';
import type { OperationPlanDto } from '@/model/dto/OperationPlanDto';
import type { Staff } from '@/model/Staff';
import type { STSCrane } from '@/model/PhysicalResource';
import { useTaskCategories } from '@/composables/taskcats';
import GanttChart, { type GanttItem } from '@/components/GanttChart.vue';
import EntityForm from '@/components/crud/EntityForm.vue';
import Loading from '@/components/Loading.vue';
import type { GanttBarObject } from '@infectoone/vue-ganttastic';
import { useOperationValidation } from '@/composables/opwarnings';
import OperationWarnings from './PlanTools/OperationWarnings.vue';
import OperationDetailsDrawer from './PlanTools/OperationDetailsDrawer.vue';
import OperationPlanToolbar from './PlanTools/OperationPlanToolbar.vue';

const notifications = useAlerts();
const route = useRoute();
const { t } = useI18n();
const { getGanttItems, getGanttRowConfigs } = useTaskCategories();

const planId = String(route.params.id || '');
const planService = container.get<IOperationPlanService>(TYPES.operationPlanService);
const staffService = container.get<IStaffService>(TYPES.staffService);
const physicalResourceService = container.get<IPhysicalResourceService>(TYPES.physicalResourceService);

const plan = ref<OperationPlanDto | null>(null);
const allStaff = ref<Staff[]>([]);
const allSTSCranes = ref<STSCrane[]>([]);
const loading = ref(false);
const editingOperation = ref<number | null>(null);

const ganttItems = computed(() => plan.value ? getGanttItems(plan.value) : []);
const ganttRowConfigs = computed(() => plan.value ? getGanttRowConfigs(plan.value) : []);

const { warnings } = useOperationValidation(plan, allSTSCranes);

onMounted(async () => {
    try {
        loading.value = true;

        plan.value = await planService.getOperationPlanById(planId);
        
        originalSchedule.value = JSON.parse(JSON.stringify(plan.value));

        const [staffPage, resourcesPage] = await Promise.all([
            staffService.getStaffs(),
            physicalResourceService.getPhysicalResources()
        ]);

        allStaff.value = staffPage.items;
        allSTSCranes.value = resourcesPage.items.filter((r: any) => 
            r.liftingCapacity !== undefined && r.status === 0
        );

    } catch (error) {
        console.error('Error loading resources:', error);
        notifications.enqueueNotification(
            'Failed to load operation plan', 
            notifications.notificationTypes.DANGER
        );
    } finally {
        loading.value = false;
    }
});

const onItemUpdated = (updatedItem: GanttItem) => {
    if (!plan.value) return;
    
    const match = updatedItem.id.match(/^op(\d+)/);
    if (!match) return;
    
    const opIndex = parseInt(match[1]);
    
    if (opIndex >= 0 && opIndex < plan.value.operationSchedule.length) {
        plan.value.operationSchedule = plan.value.operationSchedule.map((op, idx) => {
            if (idx === opIndex) {
                return {
                    ...op,
                    startTime: updatedItem.startTime,
                    endTime: updatedItem.endTime
                };
            }
            return op;
        });
    }
};

const onOperationClick = (value: {
    bar: GanttBarObject;
    e: MouseEvent;
    datetime?: string | Date | undefined;
}) => {
    const index = value.bar.ganttBarConfig.id.split("-")[0].slice(2);
    const operations = plan.value?.operationSchedule;
    
    if (!operations || !operations[parseInt(index)]) return;

    editingOperation.value = parseInt(index);
    
    const drawer = document.querySelector('sl-drawer') as any;
    drawer?.show();
};

const closeDrawer = () => {
    const drawer = document.querySelector('sl-drawer') as any;
    drawer?.hide();
    editingOperation.value = null;
};

const addStaff = (operationIndex: number, staffSelected: Staff) => {
    if (!plan.value) return;
    
    const staff = allStaff.value.find(s => s.email === staffSelected.email);
    if (!staff) return;

    // Check if already assigned
    const alreadyAssigned = plan.value.operationSchedule[operationIndex].resources.some(
        r => r.name === staff.email && r.type === 'Staff'
    );
    if (alreadyAssigned) {

        notifications.enqueueNotification(
            `Staff member ${staff.name} is already assigned to this operation.`,
            notifications.notificationTypes.WARNING
        );
        return;
    }

    // Add staff as a resource
    const updatedSchedule = [...plan.value.operationSchedule];
    updatedSchedule[operationIndex] = {
        ...updatedSchedule[operationIndex],
        resources: [
            ...updatedSchedule[operationIndex].resources,
            {
                name: staff.email,
                type: 'Staff'
            }
        ]
    };
    
    plan.value.operationSchedule = updatedSchedule;
}

const removeStaff = (operationIndex: number, staffSelected: string) => {
    if (!plan.value) return;

    const updatedSchedule = [...plan.value.operationSchedule];
    updatedSchedule[operationIndex] = {
        ...updatedSchedule[operationIndex],
        resources: updatedSchedule[operationIndex].resources.filter(
            r => r.name !== staffSelected || r.type !== 'Staff'
        )
    };
    plan.value.operationSchedule = updatedSchedule;
};

const savePlan = async () => {
    console.log('Saving plan:', plan.value);
    // TODO: Implement save logic
};

// Operations
const originalSchedule = ref<OperationPlanDto | null>(null);
const ganttKey = ref(0);

const handleShiftOperations = (minutes: number) => {
    if (!plan.value) return;

    const updatedSchedule = plan.value.operationSchedule.map(op => {
        const startTime = new Date(op.startTime);
        const endTime = new Date(op.endTime);
        
        startTime.setMinutes(startTime.getMinutes() + minutes);
        endTime.setMinutes(endTime.getMinutes() + minutes);
        
        return {
            ...op,
            startTime: startTime.toISOString(),
            endTime: endTime.toISOString()
        };
    });
    
    plan.value.operationSchedule = updatedSchedule;
    
    notifications.enqueueNotification(
        `All operations shifted ${Math.abs(minutes)} minutes ${minutes > 0 ? 'forward' : 'backward'}`,
        notifications.notificationTypes.SUCCESS
    );

    ganttKey.value += 1; // Force Gantt chart to re-render
};

const handleOptimizeSchedule = () => {
    if (!plan.value || plan.value.operationSchedule.length === 0) return;
    
    // Sort operations by start time and remove gaps
    const sorted = [...plan.value.operationSchedule].sort(
        (a, b) => new Date(a.startTime).getTime() - new Date(b.startTime).getTime()
    );
    
    // Pack operations
    let currentTime = new Date(sorted[0].startTime).getTime();
    const optimized = sorted.map(op => {
        const duration = new Date(op.endTime).getTime() - new Date(op.startTime).getTime();
        const newStart = new Date(currentTime);
        const newEnd = new Date(currentTime + duration);
        
        currentTime = newEnd.getTime();
        
        return {
            ...op,
            startTime: newStart.toISOString(),
            endTime: newEnd.toISOString()
        };
    });
    
    plan.value = {
        ...plan.value,
        operationSchedule: optimized
    };
    
    ganttKey.value++; // Force re-render
    
    notifications.enqueueNotification(
        'Schedule optimized',
        notifications.notificationTypes.PRIMARY
    );
};

const handleResetSchedule = () => {
    if (!originalSchedule.value) return;
    
    const cloned = JSON.parse(JSON.stringify(originalSchedule.value));
    const updatedSchedule = cloned.operationSchedule.map((op: any) => ({
        ...op
    }));

    plan.value.operationSchedule = updatedSchedule;
    
    notifications.enqueueNotification(
        'Schedule reset to original',
        notifications.notificationTypes.PRIMARY
    );

    ganttKey.value += 1; // Force Gantt chart to re-render
};

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/scheduling-dashboard" class="breadcrumb-link">
                    {{ t('scheduling.tabs.dashboard') }}
                </RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink to="/scheduling/plans-search" class="breadcrumb-link">
                    {{ t('scheduling.tabs.search') }}
                </RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink :to="`/scheduling/plans-view/${planId}`" class="breadcrumb-link">
                    {{ planId }}
                </RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('operationPlan.tabs.edit') }}</sl-breadcrumb-item>
        </sl-breadcrumb>
        
        <h1 class="title">{{ t('operationPlan.tabs.edit') }}</h1>
        <p class="subtitle">{{ t('operationPlan.subtitle.edit') }}</p>
        
        <Loading v-if="loading" />
        <EntityForm 
            v-else 
            :object="plan" 
            :submit-function="savePlan" 
            :editing-id="planId"
        >
            <h3>Allocated resources</h3>

            <h3>{{ t('operationPlan.schedule.title') }}</h3>
            
            <div class="form-fields">
                <div class="schedule-section">

                    <OperationPlanToolbar
                        @shift-operations="handleShiftOperations"
                        @optimize-schedule="handleOptimizeSchedule"
                        @reset-schedule="handleResetSchedule"
                    />
                

                    <GanttChart
                        :key="ganttKey"
                        :items="ganttItems"
                        :row-configs="ganttRowConfigs"
                        @item-updated="onItemUpdated"
                        @bar-click="onOperationClick"
                    />
                </div>
                
                <OperationWarnings :warnings="warnings" />
            </div>
        </EntityForm>

        <OperationDetailsDrawer
            :plan="plan" 
            :operation-index="editingOperation"
            @close="closeDrawer"
            :available-staff="allStaff"
            @add-staff="addStaff"
            @remove-staff="removeStaff"
        />
    </div>
</template>

<style scoped>
.form-fields {
    
    display: flex;
    flex-direction: column;
    gap: 1rem;

    margin-top: -50px;
}

.schedule-section {
    margin-top: 2rem;
}
</style>