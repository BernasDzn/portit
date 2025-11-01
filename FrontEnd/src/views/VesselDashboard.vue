<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
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
                    icon="directions_boat"
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
    </div>
</template>