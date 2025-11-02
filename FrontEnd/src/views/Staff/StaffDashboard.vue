<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
</script>

<template>
    <div>
        <h1 class="title">Staff Dashboard</h1>
        <p class="subtitle">Overview of staff members statistics and activities</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    title="View Staff Members"
                    description="Manage and view all registered staff members"
                    icon="search"
                    to="/staff/search"
                />
                <DashboardItem
                    title="Create Staff Member"
                    description="Register a new staff member into the system"
                    icon="add"
                    to="/staff/create"
                />
            </div>
        </sl-card>
    </div>
</template>