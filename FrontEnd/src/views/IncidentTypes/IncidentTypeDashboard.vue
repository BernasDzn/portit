<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
import { useI18n } from 'vue-i18n'
import Loading from '@/components/Loading.vue';
import { ref, onMounted } from 'vue';
import type { IIncidentTypeService } from '@/service/IService/IIncidentTypeService';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { Filter, Page } from '@/model/Page';
import type { IncidentTypeDto } from '@/model/IncidentType';

const { t } = useI18n();

const incidentTypeService = container.get<IIncidentTypeService>(TYPES.incidentTypeService);
const numberOfIncidentTypes = ref(0);
const loading = ref(true);

const fetchIncidentTypes = async (filtering?: Filter<IncidentTypeDto>): Promise<Page<IncidentTypeDto>> => {
    return await incidentTypeService.getAllIncidentTypes(filtering);
}

onMounted(async () => {
    try {
        const types = await fetchIncidentTypes();
        numberOfIncidentTypes.value = types.items.length;
        loading.value = false;
    } catch (err) {
        console.error('Failed to load incident types', err);
    }
});

</script>

<template>
    <div>
        <h1 class="title">{{ t('incidentType.tabs.dashboard') }}</h1>
        <p class="subtitle">{{ t('incidentType.subtitle.dashboard') }}</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    :title="t('incidentType.tabs.view_dashboard')"
                    :description="t('incidentType.subtitle.search')"
                    icon="search"
                    to="/incident-types/search"
                />
                <DashboardItem
                    :title="t('incidentType.tabs.create')"
                    :description="t('incidentType.subtitle.create')"
                    icon="add"
                    to="/incident-types/create"
                />
            </div>
        </sl-card>
        <sl-card class="dashboard-statistics">
            <div class="stats-overview" v-if="!loading">
                <p>{{numberOfIncidentTypes}}</p>
                <p>{{ t('incidentType.registeredIncidentTypes') }}</p>
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
