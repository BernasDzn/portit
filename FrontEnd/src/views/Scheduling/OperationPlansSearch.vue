<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IOperationPlanService } from '@/service/IService/IOperationPlanService';
import OperationPlanPrinter from '@/components/printers/OperationPlanPrinter.vue';
import CalendarEvents from '@/components/crud/CalendarEvents.vue';
import { computed, onMounted, ref } from 'vue';

const operationPlanService = container.get<IOperationPlanService>(TYPES.operationPlanService);

const { t, locale } = useI18n();

const operationPlans = ref<any[]>([]);
const selectedDate = ref(new Date());
const events = ref<Array<{ title: string, start: string }>>([]);

const fetchOperationPlans = async () => {
    const plans = await operationPlanService.getAllOperationPlans();
    operationPlans.value = plans;
    
    // Create calendar events from operation plans
    events.value = plans.map((plan: any) => ({
        title: `${t('operationPlan.title')} - ${plan.dockPlanMap?.length || 0} ${t('operationPlan.docks')}`,
        start: typeof plan.date === 'string' ? plan.date : new Date(plan.date).toISOString()
    }));
};

const plansOnDate = computed(() => {
    if (!selectedDate.value) return [];
    const selected = selectedDate.value;
    return operationPlans.value.filter(plan => {
        const planDate = new Date(plan.date);
        return planDate.getFullYear() === selected.getFullYear() &&
            planDate.getMonth() === selected.getMonth() &&
            planDate.getDate() === selected.getDate();
    });
});

onMounted(async () => {
    await fetchOperationPlans();
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

            <sl-tab-panel name="general">
                <div class="plans-grid">
                    <ul class="plans-list" v-if="operationPlans.length > 0">
                        <li v-for="(plan, index) in operationPlans" :key="index" class="link">
                            <OperationPlanPrinter :operation-plan="plan" />
                        </li>
                    </ul>
                    <p v-else class="no-data">{{ t('common.noData') }}</p>
                </div>
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
                                />
                            </li>
                        </ul>
                        <p v-else>{{ t('operationPlan.noPlansOnDate') }}</p>
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
</style>