<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
import { useI18n } from 'vue-i18n'
import AxiosHttpService from '@/service/AxiosHttpService';
import { DockService } from '@/service/DockService';
import Loading from '@/components/Loading.vue';
import { ref, onMounted } from 'vue';

const { t } = useI18n();

const http = new AxiosHttpService();
const dockService = new DockService(http);
const numberOfDocks = ref(0);
const loading = ref(true);
onMounted(async () => {
    try {
        numberOfDocks.value = await dockService.getNumberOfDocks();
        loading.value = false;
    } catch (err) {
        console.error('Failed to load docks', err);
    }
});
</script>

<template>
    <div>
        <h1 class="title">{{ t('dock.tabs.dashboard') }}</h1>
        <p class="subtitle">{{ t('dock.subtitle.dashboard') }}</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    :title="t('dock.tabs.view_dashboard')"
                    :description="t('dock.subtitle.search')"
                    icon="search"
                    to="/docks/search"
                />
                <DashboardItem
                    :title="t('dock.tabs.create')"
                    :description="t('dock.subtitle.create')"
                    icon="add"
                    to="/docks/create"
                />
            </div>
        </sl-card>
        <sl-card class="dashboard-statistics">
            <div class="stats-overview" v-if="!loading">
                <p>{{numberOfDocks}}</p>
                <p>{{ t('dock.registeredDocks') }}</p>
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