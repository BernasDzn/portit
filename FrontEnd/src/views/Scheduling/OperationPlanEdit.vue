<script setup lang="ts">
import { useRoute, RouterLink } from 'vue-router';
import { useAlerts } from '@/composables/alerts';
import { ref, onMounted, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IOperationPlanService } from '@/service/IService/IOperationPlanService';
import type { OperationPlanDto } from '@/model/dto/OperationPlanDto';
import GanttChart, { type GanttItem } from '@/components/GanttChart.vue';
import EntityForm from '@/components/crud/EntityForm.vue';
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import type { IStaffService } from '@/service/IService/IStaffService';
import type { IPhysicalResourceService } from '@/service/IService/IPhysicalResourceService';
import type { Staff } from '@/model/Staff';
import type { STSCrane } from '@/model/PhysicalResource';
import { useTaskCategories } from '@/composables/taskcats';
import type { GanttBarObject } from '@infectoone/vue-ganttastic';
import type { IStorageAreaService } from '@/service/IService/IStorageAreaService';
import ObjectSelector from '@/components/crud/ObjectSelector.vue';

const notifications = useAlerts();
const route = useRoute();
const { t } = useI18n();
const { getGanttItems, getGanttRowConfigs } = useTaskCategories();

const planId = String(route.params.id || '');

const planService = container.get<IOperationPlanService>(TYPES.operationPlanService);
const staffService = container.get<IStaffService>(TYPES.staffService);
const physicalResourceService = container.get<IPhysicalResourceService>(TYPES.physicalResourceService);
const storageAreaService = container.get<IStorageAreaService>(TYPES.storageAreaService);

const plan = ref<OperationPlanDto | null>(null);
const selectedStaff = ref<string[]>([]);
const selectedSTSCranes = ref<string[]>([]);
const allStaff = ref<Staff[]>([]);
const allSTSCranes = ref<STSCrane[]>([]);

const ganttItems = computed(() => plan.value ? getGanttItems(plan.value) : []);
const ganttRowConfigs = computed(() => plan.value ? getGanttRowConfigs(plan.value) : []);

const onItemUpdated = (updatedItem: GanttItem) => {
    if (!plan.value) return;
    
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
        plan.value = await planService.getOperationPlanById(planId);

        const [staffPage, resourcesPage] = await Promise.all([
            staffService.getStaffs(),
            physicalResourceService.getPhysicalResources()
        ]);

        allStaff.value = staffPage.items;
        allSTSCranes.value = resourcesPage.items.filter((r: any) => 
            r.liftingCapacity !== undefined && r.status === 0
        );

        const usedResources = new Set<string>();
        plan.value.operationSchedule.forEach(op => {
            op.resources.forEach(res => usedResources.add(res.name));
        });

        selectedStaff.value = allStaff.value
            .filter(s => usedResources.has(s.mechanographicNumber) || usedResources.has(s.name))
            .map(s => s.mechanographicNumber);

        selectedSTSCranes.value = allSTSCranes.value
            .filter(c => usedResources.has(c.code) || usedResources.has(c.description))
            .map(c => c.code);

    } catch (error) {
        console.error('Error loading resources:', error);
        notifications.enqueueNotification(
            'Failed to load operation plan', 
            notifications.notificationTypes.DANGER
        );
    }
});

const savePlan = async () => {
    console.log('Saving plan:', plan.value);
};

const editingOperation = ref<number | null>(null);
const onOperationClick = (value: {
    bar: GanttBarObject;
    e: MouseEvent;
    datetime?: string | Date | undefined;
}) => {
    
    const index = value.bar.ganttBarConfig.id.split("-")[0].slice(2);

    // Find operation on list
    const operations = plan.value?.operationSchedule;
    if (!operations) return;

    const operation = operations[parseInt(index)];
    if (!operation) return;

    editingOperation.value = parseInt(index);

    // Open drawer
    const drawer = document.querySelector('sl-drawer') as any;
    if (drawer) {
        drawer.show();
    }
}

const closeDrawer = () => {
    const drawer = document.querySelector('sl-drawer') as any;
    if (drawer) {
        drawer.hide();
    }
    editingOperation.value = null;
}

const newRow = () => {

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
        
        <h3>{{ t('operationPlan.schedule.title') }}</h3>

        <div style="display: flex; gap: 1em">
            <sl-button variant="default" type="submit" @click="newRow">
                <sl-icon slot="prefix" name="plus-circle"></sl-icon>
                Add operation
            </sl-button>
    
            <sl-button variant="default" type="submit" @click="newRow">
                <sl-icon slot="prefix" name="plus-circle"></sl-icon>
                Add operation track
            </sl-button>
        </div>

        <EntityForm :object="plan" :submit-function="savePlan" :editing-id="planId">
            <div class="form-fields">
                <div class="schedule-section">
                    <GanttChart
                        :items="ganttItems"
                        :row-configs="ganttRowConfigs"
                        @item-updated="onItemUpdated"
                        @bar-click="onOperationClick"
                    />
                </div>
            </div>
        </EntityForm>

        <sl-drawer label="Drawer" class="drawer-overview">
            <h3>
                Operation Details
            </h3>

            <div v-if="plan && editingOperation && plan?.operationSchedule[editingOperation!]">
                <p>
                    Start date: 
                    {{ new Date(plan?.operationSchedule[editingOperation!]?.startTime).toLocaleString().split(',')[1].split(":").slice(0,2).join(":") }}
                </p>
                <p>
                    End date: 
                    {{ new Date(plan?.operationSchedule[editingOperation!]?.endTime).toLocaleString().split(',')[1].split(":").slice(0,2).join(":") }}
                </p>
    
                <p><strong>Category:</strong> {{ plan?.operationSchedule[editingOperation!]?.type.description }} operation</p>
    
                <sl-divider></sl-divider>
                <h3>Resources</h3>
    
                <sl-divider></sl-divider>
                <h3>Operation playload</h3>
                <div v-if="plan?.operationSchedule[editingOperation!]?.type.category.value == 'LOAD' || plan?.operationSchedule[editingOperation!]?.type.category.value == 'UNLOAD'">
                        
                    <sl-input 
                        name="containerId" 
                        label="Container ID:"
                    ></sl-input>
    
                    <p>Storage area</p>
                    <ObjectSelector
                        class="field-dropdown"
                        :name="t('dock.fields.supportedVesselTypes.vesselTypes.title') + '*'"
                        :fetch-function="() => storageAreaService.getStorageAreas()"
                        :placeholderText="'Select a storage area'"
                        labelKey="name"
                    />
    
                </div>
                <div v-else>
                    <i>No payload specifiable for this operation category</i>
                </div>
            </div>


            <sl-button @click="closeDrawer" variant="danger" slot="footer">Remove Operation</sl-button>
            <sl-button @click="closeDrawer" slot="footer" variant="primary">Close</sl-button>
          </sl-drawer>          
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