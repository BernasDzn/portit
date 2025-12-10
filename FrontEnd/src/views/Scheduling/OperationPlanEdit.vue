<script setup lang="ts">
import { useRoute, RouterLink } from 'vue-router';
import { useAlerts } from '@/composables/alerts';
import { ref, onMounted, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IOperationPlanService } from '@/service/IService/IOperationPlanService';
import type { OperationPlanDto } from '@/model/dto/OperationPlanDto';
import GanttChart, { type GanttItem, type GanttRowConfig } from '@/components/GanttChart.vue';
import EntityForm from '@/components/crud/EntityForm.vue';
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import type { IStaffService } from '@/service/IService/IStaffService';
import type { IPhysicalResourceService } from '@/service/IService/IPhysicalResourceService';
import type { Staff } from '@/model/Staff';
import type { STSCrane } from '@/model/PhysicalResource';

const notifications = useAlerts();
const route = useRoute();
const { t } = useI18n();

const planId = String(route.params.id || '');

const planService = container.get<IOperationPlanService>(TYPES.operationPlanService);
const staffService = container.get<IStaffService>(TYPES.staffService);
const physicalResourceService = container.get<IPhysicalResourceService>(TYPES.physicalResourceService);

const plan = ref<OperationPlanDto | null>(null);

const selectedStaff = ref<string[]>([]);
const selectedSTSCranes = ref<string[]>([]);
const allStaff = ref<Staff[]>([]);
const allSTSCranes = ref<STSCrane[]>([]);

const ganttItems = computed<GanttItem[]>(() => {
    if (!plan.value || !plan.value.operationSchedule) {
        return [];
    }
    
    const items: GanttItem[] = [];
    
    plan.value.operationSchedule.forEach((op, opIndex) => {
        if (!op.startTime || !op.endTime || !op.resources || op.resources.length === 0) {
            return;
        }
        
        const opColor = op.type.category.value === 'LOAD' ? '#7BF1A8' : '#FFA2A2';
        
        // Create an item for each resource in this operation
        op.resources.forEach((resource, resIndex) => {
            items.push({
                id: `op${opIndex}-res${resIndex}`,
                startTime: op.startTime,
                endTime: op.endTime,
                name: `${op.type.category.value} Operation`,
                group: resource.name || resource.type || 'Unassigned',
                color: opColor
            });
        });
    });
    
    return items;
});

const ganttRowConfigs = computed<GanttRowConfig[]>(() => {
    if (!plan.value) return [];
    
    // Collect all unique resource names
    const resourceNames = new Set<string>();
    
    plan.value.operationSchedule.forEach(op => {
        op.resources.forEach(res => {
            resourceNames.add(res.name || res.type || 'Unassigned');
        });
        if (op.resources.length === 0) {
            resourceNames.add('Unassigned');
        }
    });
    
    // Create a row config for each resource with alternating colors
    const colors = ['#3498db', '#e74c3c', '#2ecc71', '#f39c12', '#9b59b6', '#1abc9c'];
    
    return Array.from(resourceNames).map((name, index) => ({
        name,
        color: colors[index % colors.length]
    }));
});

const onItemUpdated = (updatedItem: GanttItem) => {
    if (!plan.value) return;
    
    // Parse the item ID (format: op{opIndex}-res{resIndex})
    const match = updatedItem.id.match(/^op(\d+)/);
    if (!match) return;
    
    const opIndex = parseInt(match[1]);
    
    if (opIndex >= 0 && opIndex < plan.value.operationSchedule.length) {
        plan.value.operationSchedule[opIndex].startTime = updatedItem.startTime;
        plan.value.operationSchedule[opIndex].endTime = updatedItem.endTime;
        
        console.log('Updated operation schedule:', plan.value.operationSchedule);
    }
};

onMounted(async () => {
    try {
        // Fetch plan
        const fetchedPlan = await planService.getOperationPlanById(planId);
        plan.value = fetchedPlan;

        // Fetch all staff and STS cranes
        const staffPage = await staffService.getStaffs();
        allStaff.value = staffPage.items;

        // Fetch STS cranes without filter first to see what we get
        const resourcesPage = await physicalResourceService.getPhysicalResources();
        console.log('All resources fetched:', resourcesPage);
        
        // Filter for STS Cranes (type 0) on the client side
        allSTSCranes.value = resourcesPage.items.filter((r: any) => r.liftingCapacity !== undefined && r.status === 0);
        console.log('Filtered STS Cranes:', allSTSCranes.value);

        // Extract currently used resources from plan
        const usedResources = new Set<string>();
        plan.value.operationSchedule.forEach(op => {
            op.resources.forEach(res => {
                usedResources.add(res.name);
            });
        });

        // Mark used resources as selected
        selectedStaff.value = allStaff.value
            .filter(s => usedResources.has(s.mechanographicNumber) || usedResources.has(s.name))
            .map(s => s.mechanographicNumber);

        selectedSTSCranes.value = allSTSCranes.value
            .filter(c => usedResources.has(c.code) || usedResources.has(c.description))
            .map(c => c.code);

        console.log('Selected staff:', selectedStaff.value);
        console.log('Selected STS cranes:', selectedSTSCranes.value);

    } catch (error) {
        console.error('Error loading resources:', error);
        notifications.enqueueNotification('Failed to load operation plan', notifications.notificationTypes.DANGER);
    }
});

const savePlan = async () => {
    console.log('Saving plan:', plan.value);
};

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item><RouterLink to="/scheduling-dashboard" class="breadcrumb-link">{{ t('scheduling.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item><RouterLink to="/scheduling/plans-search" class="breadcrumb-link">{{ t('scheduling.tabs.search') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item><RouterLink :to="`/scheduling/plans-view/${planId}`" class="breadcrumb-link">{{ planId }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('operationPlan.tabs.edit') }}</sl-breadcrumb-item>
        </sl-breadcrumb>
        
        <h1 class="title">{{ t('operationPlan.tabs.edit') }}</h1>
        <p class="subtitle">{{ t('operationPlan.subtitle.edit') }}</p>
        
        <EntityForm :object="plan" :submit-function="savePlan" :editing-id="planId">
            <div class="form-fields">
                <!-- Resource Selection -->
                <div class="resources-section">
                    <h3>{{ t('operationPlan.resources') }}</h3>
                    <div class="resource-selectors">
                        <EntityDropdown
                            :name="t('operationPlan.cranes')"
                            v-model="selectedSTSCranes"
                            :items="allSTSCranes"
                            valueKey="code"
                            labelKey="code"
                            :multiple="true"
                            :placeholderText="'Select STS Cranes'"
                            class="resource-dropdown"
                        />
                        <EntityDropdown
                            :name="t('staff.title')"
                            v-model="selectedStaff"
                            :items="allStaff"
                            valueKey="mechanographicNumber"
                            labelKey="name"
                            :multiple="true"
                            :placeholderText="'Select Staff Members'"
                            class="resource-dropdown"
                        />
                    </div>
                </div>

                <!-- Editable schedule via Gantt Chart -->
                <div class="schedule-section">
                    <h3>{{ t('operationPlan.schedule.title') }}</h3>
                    <GanttChart
                        :items="ganttItems"
                        :row-configs="ganttRowConfigs"
                        @item-updated="onItemUpdated"
                    />
                </div>
            </div>
        </EntityForm>
    </div>
</template>

<style scoped>
.form-fields {
    display: flex;
    flex-direction: column;
    gap: 1rem;
}

.resources-section {
    margin-top: 1rem;
}

.resources-section h3,
.schedule-section h3 {
    margin-bottom: 1rem;
    font-size: 1.2rem;
    font-weight: 600;
}

.resource-selectors {
    display: flex;
    gap: 1rem;
    flex-wrap: wrap;
}

.resource-dropdown {
    flex: 1;
    min-width: 300px;
}

.schedule-section {
    margin-top: 2rem;
}
</style>