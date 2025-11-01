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
                    title="View Docks"
                    description="Manage and view all registered docks"
                    icon="anchor"
                    to="/docks/search"
                />
                <DashboardItem
                    title="Create Dock"
                    description="Register a new dock into the system"
                    icon="add"
                    to="/docks/create"
                />
            </div>
        </sl-card>
    </div>
</template>