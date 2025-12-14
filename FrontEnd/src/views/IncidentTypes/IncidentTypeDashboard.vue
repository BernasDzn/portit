<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
import { useI18n } from 'vue-i18n'
import Loading from '@/components/Loading.vue';
import { ref, onMounted } from 'vue';
import type { IIncidentTypeService } from '@/service/IService/IIncidentTypeService';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';

const { t } = useI18n();

const incidentTypeService = container.get<IIncidentTypeService>(TYPES.incidentTypeService);
const numberOfIncidentTypes = ref(0);
const loading = ref(true);
onMounted(async () => {
    try {
        const types = await incidentTypeService.getAllIncidentTypes();
        numberOfIncidentTypes.value = types.length;
        loading.value = false;
    } catch (err) {
        console.error('Failed to load incident types', err);
    }
});
</script>

<template>
    <div>
        <h1 class="title">Incident Types Dashboard</h1>
        <p class="subtitle">Overview of incident type statistics and activities</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    title="View Incident Types"
                    description="Manage and view all registered incident types"
                    icon="search"
                    to="/incident-types/search"
                />
                <DashboardItem
                    title="Create Incident Type"
                    description="Register a new incident type into the system"
                    icon="add"
                    to="/incident-types/create"
                />
            </div>
        </sl-card>
        <sl-card class="dashboard-statistics">
            <div class="stats-overview" v-if="!loading">
                <p>{{numberOfIncidentTypes}}</p>
                <p>Registered Incident Types</p>
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
