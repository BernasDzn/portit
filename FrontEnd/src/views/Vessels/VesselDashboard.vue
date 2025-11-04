<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';

import AxiosHttpService from '@/service/AxiosHttpService';
import { VesselService } from '@/service/VesselService';
import Loading from '@/components/Loading.vue';
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n'

const http = new AxiosHttpService();
const vesselService = new VesselService(http);
const { t } = useI18n()

const numberOfVessels = ref(0);
const loading = ref(true)
onMounted(async () => {
    try {
        // assign to the ref's value so Vue reactivity updates the template
        numberOfVessels.value = await vesselService.getNumberOfVessels();
        loading.value = false;
    } catch (err) {
        console.error('Failed to load vessels', err);
    }
});
</script>

<template>
    <div>
        <h1 class="title">{{ t('vessel.tabs.dashboard') }}</h1>
        <p class="subtitle">{{ t('vessel.subtitle.dashboard') }}</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    :title="t('vessel.tabs.view_dashboard')"
                    :description="t('vessel.subtitle.search')"
                    icon="search"
                    to="/vessels/search"
                />
                <DashboardItem
                    :title="t('vessel.tabs.create')"
                    :description="t('vessel.subtitle.create')"
                    icon="add"
                    to="/vessels/create"
                />
            </div>
        </sl-card>
        <sl-card class="dashboard-statistics">
            <div class="stats-overview" v-if="!loading">
                <p>{{numberOfVessels}}</p>
                <p>{{ t('vessel.registeredVessels') }}</p>
            </div>
            <Loading v-if="loading"/>
        </sl-card>
    </div>
</template>

<style scoped>
.info-card{
    border-radius: 0.5rem;
    height: fit-content;
    flex: fit-content;
}

.info-grid {
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
}

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