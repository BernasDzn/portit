<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

</script>

<template>
    <div>
        <h1 class="title">{{ t('user.tabs.dashboard') }}</h1>
        <p class="subtitle">{{ t('user.subtitle.dashboard') }}</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    :title="t('user.tabs.create')"
                    :description="t('user.subtitle.create')"
                    icon="person_add"
                    to="/admin/users/create"
                />
                <DashboardItem
                    :title="t('user.tabs.view')"
                    :description="t('user.subtitle.view')"
                    icon="search"
                    to="/admin/users/search"
                />
            </div>
        </sl-card>
    </div>
</template>