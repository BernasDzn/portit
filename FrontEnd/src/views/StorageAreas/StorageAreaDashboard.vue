<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';

import AxiosHttpService from '@/service/AxiosHttpService';
import { ref } from 'vue';
// @ts-ignore: missing type declarations for 'vue-i18n' in this project
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

</script>

<template>
    <div>
        <h1 class="title">{{ t('storageArea.tabs.dashboard') }}</h1>
        <p class="subtitle">{{ t('storageArea.subtitle.dashboard') }}</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    :title="t('storageArea.tabs.view_dashboard')"
                    :description="t('storageArea.subtitle.search')"
                    icon="search"
                    to="/storage-areas/search"
                />
                <DashboardItem
                    :title="t('storageArea.tabs.create')"
                    :description="t('storageArea.subtitle.create')"
                    icon="add"
                    to="/storage-areas/create"
                />
            </div>
        </sl-card>
    </div>
</template>

<style scoped>
.info-card{
    border-radius: 0.5rem;
    height: fit-content;
    flex: fit-content;
}

.info-grid {
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
}
</style>