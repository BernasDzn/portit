<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

</script>

<template>
    <div>
        <h1 class="title">{{ t('scheduling.tabs.dashboard') }}</h1>
        <p class="subtitle">{{ t('scheduling.subtitles.dashboard') }}</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem :title="t('scheduling.tabs.schedule')"
                    :description="t('scheduling.subtitles.schedule')" 
                    icon="calendar_month" to="/scheduling/schedule" />
                <DashboardItem :title="t('scheduling.tabs.search')" 
                    :description="t('scheduling.subtitles.search')"
                    icon="search" to="/scheduling/plans-search" />
            </div>
        </sl-card>
    </div>
</template>
