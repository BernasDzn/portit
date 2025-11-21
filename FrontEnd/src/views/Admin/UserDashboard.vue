<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
import Loading from '@/components/Loading.vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IAdminService } from '@/service/IService/IAdminService';
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

const adminService = container.get<IAdminService>(TYPES.adminService);
const { t } = useI18n()

const numberOfUsers = ref(0);
const loading = ref(true)
onMounted(async () => {
    try {
        // assign to the ref's value so Vue reactivity updates the template
        numberOfUsers.value = await adminService.getAllUsers().then(users => users.length);
        loading.value = false;
    } catch (err) {
        console.error('Failed to load vessels', err);
    }
});
</script>

<template>
    <div>
        <h1 class="title">{{ t('user.tabs.dashboard') }}</h1>
        <p class="subtitle">{{ t('user.subtitle.dashboard') }}</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem :title="t('user.tabs.create')" :description="t('user.subtitle.create')" icon="person_add"
                    to="/admin/users/create" />
                <DashboardItem :title="t('user.tabs.view')" :description="t('user.subtitle.view')" icon="search"
                    to="/admin/users/search" />
            </div>
        </sl-card>
        <sl-card class="dashboard-statistics">
            <div class="stats-overview" v-if="!loading">
                <p>{{ numberOfUsers }}</p>
                <p>{{ t('user.registeredUsers') }}</p>
            </div>
            <Loading v-if="loading" />
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