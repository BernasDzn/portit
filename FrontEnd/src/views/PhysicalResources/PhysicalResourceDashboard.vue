<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
import Loading from '@/components/Loading.vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IPhysicalResourceService } from '@/service/IService/IPhysicalResourceService';
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

const physicalResourceService = container.get<IPhysicalResourceService>(TYPES.physicalResourceService);

const { t } = useI18n();

const numberOfPhysicalResources = ref(0);
const loading = ref(true)
onMounted(async () => {
    try {
        numberOfPhysicalResources.value = await physicalResourceService.getNumberOfPhysicalResources();
        loading.value = false;
    } catch (err) {
        console.error('Failed to load physical resources', err);
    }
});
</script>

<template>
    <div>
        <h1 class="title">{{ t('physicalResource.tabs.dashboard') }}</h1>
        <p class="subtitle">{{ t('physicalResource.subtitle.dashboard') }}</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    :title="t('physicalResource.tabs.view_dashboard')"
                    :description="t('physicalResource.subtitle.search')"
                    icon="search"
                    to="/resources/search"
                />
                <DashboardItem
                    :title="t('physicalResource.tabs.create')"
                    :description="t('physicalResource.subtitle.create')"
                    icon="add"
                    to="/resources/create"
                />
            </div>
        </sl-card>
        <sl-card class="dashboard-statistics">
            <div class="stats-overview" v-if="!loading">
                <p>{{ numberOfPhysicalResources }}</p>
                <p>{{ t('physicalResource.registeredPhysicalResources') }}</p>
            </div>
            <Loading v-if="loading" />
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