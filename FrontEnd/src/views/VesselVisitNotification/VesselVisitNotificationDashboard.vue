<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
import { useI18n } from 'vue-i18n'

const { t } = useI18n();
</script>

<template>
    <div>
        <h1 class="title">{{ t('notification.tabs.dashboard') }}</h1>
        <p class="subtitle">{{ t('notification.subtitle.dashboard') }}</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    :title="t('notification.tabs.search')"
                    :description="t('notification.subtitle.search')"
                    icon="search"
                    to="/vessel-visit-notifications/search"
                />
                <DashboardItem
                    :title="t('notification.tabs.create')"
                    :description="t('notification.subtitle.create')"
                    icon="add"
                    to="/vessel-visit-notifications/create"
                />
            </div>
        </sl-card>
    </div>
</template>