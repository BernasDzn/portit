<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
</script>

<template>
    <div>
        <h1 class="title">{{ t('qualification.tabs.dashboard') }}</h1>
        <p class="subtitle">{{ t('qualification.subtitle.dashboard') }}</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    :title="t('qualification.tabs.view_dashboard')"
                    :description="t('qualification.subtitle.search')"
                    icon="search"
                    to="/qualifications/search"
                />
                <DashboardItem
                    :title="t('qualification.tabs.create')"
                    :description="t('qualification.subtitle.create')"
                    icon="add"
                    to="/qualifications/create"
                />
            </div>
        </sl-card>
    </div>
</template>