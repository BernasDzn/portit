<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
import Loading from '@/components/Loading.vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IVesselTypeService } from '@/service/IService/IVesselTypeService';
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n'

const vesselTypeService = container.get<IVesselTypeService>(TYPES.vesselTypeService);
const { t } = useI18n();

const numberOfVesselTypes = ref(0);
const loading = ref(true)
onMounted(async () => {
    try {
        numberOfVesselTypes.value = await vesselTypeService.getNumberOfVesselTypes();
        loading.value = false;
    } catch (err) {
        console.error('Failed to load vessels', err);
    }
});
</script>

<template>
    <div>
        <h1 class="title">{{ t('vesselType.tabs.dashboard') }}</h1>
        <p class="subtitle">{{ t('vesselType.subtitle.dashboard') }}</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    :title="t('vesselType.tabs.view_dashboard')"
                    :description="t('vesselType.subtitle.search')"
                    icon="search"
                    to="/vessel-types/search"
                />
                <DashboardItem
                    :title="t('vesselType.tabs.create')"
                    :description="t('vesselType.subtitle.create')"
                    icon="add"
                    to="/vessel-types/create"
                />
            </div>
        </sl-card>
        <sl-card class="dashboard-statistics">
            <div class="stats-overview" v-if="!loading">
                <p>{{numberOfVesselTypes}}</p>
                <p>{{ t('vesselType.registeredVesselTypes') }}</p>
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