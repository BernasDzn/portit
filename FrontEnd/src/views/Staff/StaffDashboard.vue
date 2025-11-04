<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
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
    </div>
</template>