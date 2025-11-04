<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
import { useI18n } from 'vue-i18n'

const { t } = useI18n();
</script>

<template>
    <div>
        <h1 class="title">{{ t('dock.tabs.dashboard') }}</h1>
        <p class="subtitle">{{ t('dock.subtitle.dashboard') }}</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    :title="t('dock.tabs.view_dashboard')"
                    :description="t('dock.subtitle.search')"
                    icon="search"
                    to="/docks/search"
                />
                <DashboardItem
                    :title="t('dock.tabs.create')"
                    :description="t('dock.subtitle.create')"
                    icon="add"
                    to="/docks/create"
                />
            </div>
        </sl-card>
    </div>
</template>