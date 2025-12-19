<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
import Loading from '@/components/Loading.vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IVesselVisitExecutionService } from '@/service/IService/IVesselExecutionService';
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n'

const vveService = container.get<IVesselVisitExecutionService>(TYPES.vesselVisitExecutionService);

const { t } = useI18n()

const numberOfVesselVisitExecutions = ref(0);
const loading = ref(true)
onMounted(async () => {
    try {
        loading.value = true;
        numberOfVesselVisitExecutions.value = await vveService.count();
    } catch (err) {
        console.error('Failed to load vessel executions', err);
    } finally {
     
        loading.value = false;
    }
});
</script>

<template>
    <div>
        <h1 class="title">{{ t('execution.tabs.dashboard') }}</h1>
        <p class="subtitle">{{ t('execution.subtitle.dashboard') }}</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    :title="t('execution.tabs.search')"
                    :description="t('execution.subtitle.search')"
                    icon="search"
                    to="/vessel-visit-executions/search"
                />
            </div>
        </sl-card>
        <sl-card class="dashboard-statistics">
            <div class="stats-overview" v-if="!loading">
                <p>{{numberOfVesselVisitExecutions}}</p>
                <p>{{ t('execution.registeredvesselVisitExecutions') }}</p>
            </div>
            <Loading v-if="loading"/>
        </sl-card>
    </div>
</template>

<style scoped>

.stats-overview {
    text-align: center;
    margin-bottom: 1rem;
    border-radius: 0.5rem;
    padding: 1rem;
    background-color: #f5f6fa;
}

.stats-overview p {
    margin: 0;
    padding: 0.5rem;
    color: #485ea9;
}

.stats-overview p:nth-child(1) {
    font-size: 2rem;
}
</style>