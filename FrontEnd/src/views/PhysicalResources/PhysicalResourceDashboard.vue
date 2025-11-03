<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
</script>

<template>
    <div>
        <h1 class="title">Physical Resource Dashboard</h1>
        <p class="subtitle">Control panel for all physical resource things</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    title="View Physical Resources"
                    description="Manage and view all registered Physical Resources"
                    icon="search"
                    to="/resources/search"
                />
                <DashboardItem
                    title="Create Physical Resource"
                    description="Register a new Physical Resource into the system"
                    icon="add"
                    to="/resources/create"
                />
            </div>
        </sl-card>
    </div>
</template>