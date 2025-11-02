<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
</script>

<template>
    <div>
        <h1 class="title">Vessel Type Dashboard</h1>
        <p class="subtitle">Overview of vessel type statistics and activities</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    title="View Vessel Types"
                    description="Manage and view all registered vessel types"
                    icon="search"
                    to="/vessel-types/search"
                />
                <DashboardItem
                    title="Create Vessel Type"
                    description="Register a new vessel type into the system"
                    icon="add"
                    to="/vessel-types/create"
                />
            </div>
        </sl-card>
    </div>
</template>