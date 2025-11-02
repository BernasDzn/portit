<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
</script>

<template>
    <div>
        <h1 class="title">Qualification Dashboard</h1>
        <p class="subtitle">Control panel for all qualification things</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    title="View Qualifications"
                    description="Manage and view all registered qualifications"
                    icon="search"
                    to="/qualifications/search"
                />
                <DashboardItem
                    title="Create Qualification"
                    description="Register a new qualification into the system"
                    icon="add"
                    to="/qualifications/create"
                />
            </div>
        </sl-card>
    </div>
</template>