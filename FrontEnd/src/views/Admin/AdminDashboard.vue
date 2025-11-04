<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';

import AxiosHttpService from '@/service/AxiosHttpService';
import Loading from '@/components/Loading.vue';
import { ref, onMounted } from 'vue';
import { AdminService } from '@/service/AdminService';

const http = new AxiosHttpService();
const adminService = new AdminService(http);
onMounted(async () => {

});
</script>

<template>
    <div>
        <h1 class="title">Admin Dashboard</h1>
        <p class="subtitle">Administrative overview and management</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    title="User Management"
                    description="Manage system users, roles, and permissions."
                    icon="manage_accounts"
                    to="/admin/users"
                />
                <DashboardItem
                    title="Audit Logs"
                    description="View and monitor system activity logs."
                    icon="history"
                    to="/admin/logs"
                />
            </div>
        </sl-card>
    </div>
</template>