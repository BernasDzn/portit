<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IOperationPlanService } from '@/service/IService/IOperationPlanService';
import type { ISchedulingService } from '@/service/IService/ISchedulingService';
import type { IVesselVisitNotificationService } from '@/service/IService/IVesselVisitNotificationService';
import OperationPlanPrinter from '@/components/printers/OperationPlanPrinter.vue';
import VesselVisitNotificationPrinter from '@/components/printers/VesselVisitNotificationPrinter.vue';
import CalendarEvents from '@/components/crud/CalendarEvents.vue';
import { computed, onMounted, ref } from 'vue';
import type { Filter, Page } from '@/model/Page';
import ListingBox from '@/components/crud/ListingBox.vue';
import type { VesselVisitNotification } from '@/model/VesselVisitNotification';
import type { OperationPlanDto } from '@/model/dto/OperationPlanDto';

const operationPlanService = container.get<IOperationPlanService>(TYPES.operationPlanService);
const schedulingService = container.get<ISchedulingService>(TYPES.schedulingService);
const vvnService = container.get<IVesselVisitNotificationService>(TYPES.vesselVisitNotificationService);

const { t, locale } = useI18n();

const operationPlans = ref<Page<OperationPlanDto>>({ items: [], 
    pageCount: 0, pageNumber: 0, pageSize: 0
});
const plansByDate = ref<{ date: string; plans: OperationPlanDto[] }[]>([]);
const selectedDate = ref(new Date());
const events = ref<Array<{ title: string, start: string }>>([]);
const unplannedVVNIds = ref<string[]>([]);
const unplannedVVNs = ref<VesselVisitNotification[]>([]);
const isLoadingUnplanned = ref(false);

const fetchOperationPlans = async (filtering?: Filter<null>): Promise<Page<OperationPlanDto>> => {
    const plans = await operationPlanService.getAllOperationPlans(filtering);
    operationPlans.value = plans;
    return plans;
};

const fetchPlansByDate = async () => {
    plansByDate.value = await operationPlanService.groupOperationPlansByDate();
    
    // Create calendar events
    events.value = plansByDate.value.map(group => ({
        title: `${group.plans.length} ${t('operationPlan.title')}${group.plans.length > 1 ? 's' : ''}`,
        start: group.date
    }));
};

const plansOnDate = computed(() => {
    if (!selectedDate.value) return [];
    const selectedDateStr = selectedDate.value.toISOString().split('T')[0];
    const group = plansByDate.value.find(g => g.date === selectedDateStr);
    return group ? group.plans : [];
});

const fetchUnplannedVVNs = async () => {
    isLoadingUnplanned.value = true;
    try {
        // Get the list of unplanned VVN IDs
        unplannedVVNIds.value = await schedulingService.getUnplannedVVNs();
        
        // Fetch the full VVN details for each ID
        const vvnPromises = unplannedVVNIds.value.map(id => vvnService.getVesselVisitNotificationById(id));
        unplannedVVNs.value = await Promise.all(vvnPromises);
    } catch (error) {
        console.error('Error fetching unplanned VVNs:', error);
        unplannedVVNs.value = [];
    } finally {
        isLoadingUnplanned.value = false;
    }
};

onMounted(async () => {
    await fetchOperationPlans();
    await fetchPlansByDate();
    await fetchUnplannedVVNs();
});

</script>

<template>
  <div>

    <sl-breadcrumb>
      <sl-breadcrumb-item><RouterLink to="/scheduling-dashboard" class="breadcrumb-link">{{ t('scheduling.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
      <sl-breadcrumb-item>{{ t('scheduling.tabs.search') }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
        <h1 class="title">{{ t('scheduling.tabs.search') }}</h1>
        <p class="subtitle">{{ t('scheduling.subtitles.search') }}</p>

        <sl-tab-group>
            <sl-tab slot="nav" panel="general">{{ t('notification.tabs.general') }}</sl-tab>
            <sl-tab slot="nav" panel="byDate">{{ t('notification.tabs.byDate') }}</sl-tab>
            <sl-tab slot="nav" panel="unplanned">{{ t('operationPlan.tabs.unplannedVVNs') }}</sl-tab>

            <sl-tab-panel name="general">
                <ListingBox listing-style="listing-triples" :fetch-function="fetchOperationPlans" v-slot="{elements}">
                    <li v-for="(plan, index) in operationPlans.items" :key="index" class="link">
                        <OperationPlanPrinter :operation-plan="plan" :link="`/scheduling/plans-edit/${plan.id}`" />
                    </li>
                </ListingBox>
            </sl-tab-panel>

            <sl-tab-panel name="byDate">
                <div class="calendar-events">
                    <CalendarEvents class="calendar" :events="events" v-model="selectedDate" />
                    <div class="mt-4">
                        <h2 class="subtitle">{{ t('operationPlan.plansOnDate', { date: selectedDate.toDateString() }) }}</h2>
                        <ul class="plans-list" v-if="plansOnDate.length > 0">
                            <li v-for="(plan, index) in plansOnDate" :key="index">
                                <OperationPlanPrinter class="listing-box" 
                                    :operation-plan="plan"
                                    :link="`/scheduling/plans-edit/${plan.id}`"
                                />
                            </li>
                        </ul>
                        <p v-else>{{ t('operationPlan.noPlansOnDate') }}</p>
                    </div>
                </div>
            </sl-tab-panel>

            <sl-tab-panel name="unplanned">
                <div class="unplanned-section">
                    <div v-if="isLoadingUnplanned" class="loading-state">
                        <sl-spinner></sl-spinner>
                        <p>{{ t('common.loading') }}</p>
                    </div>
                    <div v-else>
                        <ul class="vvn-list" v-if="unplannedVVNs.length > 0">
                            <li v-for="vvn in unplannedVVNs" :key="vvn.notificationId">
                                <VesselVisitNotificationPrinter 
                                    :notification="vvn"
                                    :link="`/vessel-visit-notifications/view/${vvn.notificationId}`"
                                />
                            </li>
                        </ul>
                        <p v-else class="no-data">{{ t('operationPlan.unplannedVVNs.noUnplanned') }}</p>
                    </div>
                </div>
            </sl-tab-panel>
        </sl-tab-group>

    </header>
  </div>
</template>

<style scoped> 
.link {
  text-decoration: none;
  color: inherit;
}

.plans-grid {
    margin-top: 1rem;
}

.plans-list {
    list-style-type: none;
    padding: 0;
    margin: 0;
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
    gap: 1rem;
}

.no-data {
    text-align: center;
    color: var(--sl-color-neutral-500);
    padding: 2rem;
}

.calendar-events {
    margin-top: 1rem;
    margin-left: auto;
    margin-right: auto;
    display: flex;
    align-items: flex-start;
    justify-content: center;
    gap: 5rem;
}

.calendar {
    min-width: 350px;
    height: 600px;
}

.mt-4 {
    flex: 1;
    height: 600px;
    overflow-y: auto;
    scrollbar-width: none;
    padding: 0 20px;
    -ms-overflow-style: none;
}

.mt-4::-webkit-scrollbar {
    display: none;
}

.mt-4 ul {
    list-style-type: none;
    padding: 0;
    margin: 0;
}

.mt-4 li {
    margin-bottom: 1rem;
}

.unplanned-section {
    margin-top: 1rem;
    padding: 1rem;
}

.loading-state {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 1rem;
    padding: 3rem;
}

.unplanned-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 1.5rem;
    padding: 1rem;
    background-color: var(--sl-color-neutral-100);
    border-radius: var(--sl-border-radius-medium);
}

.info-text {
    margin: 0;
    color: var(--sl-color-neutral-700);
}

.vvn-list {
    list-style-type: none;
    padding: 0;
    margin: 0;
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
    gap: 1rem;
}

.vvn-list li {
    text-decoration: none;
}

.vvn-list a {
    text-decoration: none;
    color: inherit;
}
</style>