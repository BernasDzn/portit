<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
import { useI18n } from 'vue-i18n'

const { t } = useI18n();

</script>

<template>
    <div>
        <h1 class="title">{{ t('taskCategory.tabs.dashboard') }}</h1>
        <p class="subtitle">{{ t('taskCategory.subtitle.dashboard') }}</p>
        
        <sl-card class="dashboard-overview">
            <h3 style="margin: 0 0 1rem 0;">Task Categories</h3>
            <div class="dashboard-items">
                <DashboardItem
                    :title="t('taskCategory.tabs.view_dashboard')"
                    :description="t('taskCategory.subtitle.search')"
                    icon="search"
                    to="/task-categories/search"
                />
                <DashboardItem
                    :title="t('taskCategory.tabs.create')"
                    :description="t('taskCategory.subtitle.create')"
                    icon="add"
                    to="/task-categories/create"
                />
            </div>
        </sl-card>

        <sl-card class="dashboard-overview" style="margin-top: 1rem;">
            <h3 style="margin: 0 0 1rem 0;">Complementary Tasks</h3>
            <div class="dashboard-items">
                <DashboardItem
                    title="View Complementary Tasks"
                    description="Browse all complementary tasks from vessel visit executions"
                    icon="library_add"
                    to="/complementary-tasks/search"
                />
            </div>
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
