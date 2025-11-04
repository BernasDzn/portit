<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
</script>

<template>
    <div>
        <h1 class="title">{{ t('physicalResource.tabs.dashboard') }}</h1>
        <p class="subtitle">{{ t('physicalResource.subtitle.dashboard') }}</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    :title="t('physicalResource.tabs.view_dashboard')"
                    :description="t('physicalResource.subtitle.search')"
                    icon="search"
                    to="/resources/search"
                />
                <DashboardItem
                    :title="t('physicalResource.tabs.create')"
                    :description="t('physicalResource.subtitle.create')"
                    icon="add"
                    to="/resources/create"
                />
            </div>
        </sl-card>
    </div>
</template>