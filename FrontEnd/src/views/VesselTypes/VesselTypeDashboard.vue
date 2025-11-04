<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
import { useI18n } from 'vue-i18n'

const { t } = useI18n();
</script>

<template>
    <div>
        <h1 class="title">{{ t('vesselType.tabs.dashboard') }}</h1>
        <p class="subtitle">{{ t('vesselType.subtitle.dashboard') }}</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    :title="t('vesselType.tabs.view_dashboard')"
                    :description="t('vesselType.subtitle.search')"
                    icon="search"
                    to="/vessel-types/search"
                />
                <DashboardItem
                    :title="t('vesselType.tabs.create')"
                    :description="t('vesselType.subtitle.create')"
                    icon="add"
                    to="/vessel-types/create"
                />
            </div>
        </sl-card>
    </div>
</template>