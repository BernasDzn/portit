<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
import Loading from '@/components/Loading.vue';
import { useI18n } from 'vue-i18n';
import { ref, onMounted } from 'vue';
import type { IStaffService } from '@/service/IService/IStaffService';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';

const staffService = container.get<IStaffService>(TYPES.staffService);

const { t } = useI18n();

const numberOfStaff = ref(0);
const loading = ref(true);
onMounted(async () => {
    try {
        numberOfStaff.value = await staffService.getNumberOfStaffs();
        loading.value = false;
    } catch (err) {
        console.error('Failed to load staff members', err);
    }
});
</script>

<template>
    <div>
        <h1 class="title">{{ t('staff.tabs.dashboard') }}</h1>
        <p class="subtitle">{{ t('staff.subtitle.dashboard') }}</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    :title="t('staff.tabs.view_dashboard')"
                    :description="t('staff.subtitle.search')"
                    icon="search"
                    to="/staff/search"
                />
                <DashboardItem
                    :title="t('staff.tabs.create')"
                    :description="t('staff.subtitle.create')"
                    icon="add"
                    to="/staff/create"
                />
            </div>
        </sl-card>
        <sl-card class="dashboard-statistics">
            <div class="stats-overview" v-if="!loading">
                <p>{{numberOfStaff}}</p>
                <p>{{ t('staff.registeredStaffMembers') }}</p>
            </div>
            <Loading v-if="loading"/>
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