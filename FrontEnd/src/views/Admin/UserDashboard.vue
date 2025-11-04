<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
</script>

<template>
    <div>
        <h1 class="title">User Dashboard</h1>
        <p class="subtitle">User overview and management</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    title="Create User"
                    description="Register a new user into the system"
                    icon="person_add"
                    to="/admin/users/create"
                />
                <DashboardItem
                    title="View Users"
                    description="View and manage existing users"
                    icon="search"
                    to="/admin/users/search"
                />
            </div>
        </sl-card>
    </div>
</template>