<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';

import AxiosHttpService from '@/service/AxiosHttpService';
import { VesselService } from '@/service/VesselService';
import Loading from '@/components/Loading.vue';
import { ref, onMounted } from 'vue';

const http = new AxiosHttpService();
const vesselService = new VesselService(http);

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
        <h1 class="title">Vessel Dashboard</h1>
        <p class="subtitle">Overview of vessel statistics and activities</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    title="View Vessels"
                    description="Manage and view all registered vessels"
                    icon="search"
                    to="/vessels/search"
                />
                <DashboardItem
                    title="Create Vessel"
                    description="Register a new vessel into the system"
                    icon="add"
                    to="/vessels/create"
                />
            </div>
        </sl-card>
        <sl-card class="dashboard-statistics">
            <div class="stats-overview" v-if="!loading">
                <p>{{numberOfVessels}}</p>
                <p>Registered Vessels</p>
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