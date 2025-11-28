<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
import Loading from '@/components/Loading.vue';
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n'
import { container } from '@/inversify.config';
import type { IVesselService } from '@/service/IService/IVesselService';
import type { IVesselTypeService } from '@/service/IService/IVesselTypeService';
import TYPES from '@/inversify/types';

const vesselService = container.get<IVesselService>(TYPES.vesselService);
const vesselTypeService = container.get<IVesselTypeService>(TYPES.vesselTypeService);
const { t } = useI18n()

const numberOfVessels = ref(0);
const numberOfVesselTypes = ref(0);
const loading = ref(true)
onMounted(async () => {
    try {
        // assign to the ref's value so Vue reactivity updates the template
        numberOfVessels.value = await vesselService.getNumberOfVessels();
        numberOfVesselTypes.value = await vesselTypeService.getNumberOfVesselTypes();
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
        <sl-card class="dashboard-overview" style="margin-top: 1rem;">
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
            <div class="dashboard-statistics-inner" v-if="!loading">
                <div class="stats-overview">
                <p>{{numberOfVessels}}</p>
                <p>{{ t('vessel.registeredVessels') }}</p>
                </div>
                <div class="stats-overview">
                    <p>{{numberOfVesselTypes}}</p>
                    <p>{{ t('vesselType.registeredVesselTypes') }}</p>
                </div>
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
</style>