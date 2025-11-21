<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
import Loading from '@/components/Loading.vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';

import AxiosHttpService from '@/service/AxiosHttpService';
import type { IStorageAreaService } from '@/service/IService/IStorageAreaService';
import { onMounted, ref } from 'vue';
// @ts-ignore: missing type declarations for 'vue-i18n' in this project
import { useI18n } from 'vue-i18n'

const storageAreaService = container.get<IStorageAreaService>(TYPES.storageAreaService);

const { t } = useI18n();

const numberOfStorageAreas = ref(0);
const loading = ref(true);
onMounted(async () => {
    try {
        numberOfStorageAreas.value = await storageAreaService.getNumberOfStorageAreas();
        loading.value = false;
    } catch (err) {
        console.error('Failed to load staff members', err);
    }
});
</script>

<template>
    <div>
        <h1 class="title">{{ t('storageArea.tabs.dashboard') }}</h1>
        <p class="subtitle">{{ t('storageArea.subtitle.dashboard') }}</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    :title="t('storageArea.tabs.view_dashboard')"
                    :description="t('storageArea.subtitle.search')"
                    icon="search"
                    to="/storage-areas/search"
                />
                <DashboardItem
                    :title="t('storageArea.tabs.create')"
                    :description="t('storageArea.subtitle.create')"
                    icon="add"
                    to="/storage-areas/create"
                />
            </div>
        </sl-card>
        <sl-card class="dashboard-statistics">
            <div class="stats-overview" v-if="!loading">
                <p>{{numberOfStorageAreas}}</p>
                <p>{{ t('storageArea.registeredStorageAreas') }}</p>
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