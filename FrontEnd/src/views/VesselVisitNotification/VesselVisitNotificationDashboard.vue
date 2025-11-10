<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
import { useSession } from '@/composables/session';
import { ref } from 'vue';
import { useI18n } from 'vue-i18n'

const { t } = useI18n();
const role = ref(useSession().authenticatedUser!.role);

</script>

<template>
    <div>
        <h1 class="title">{{ t('notification.tabs.dashboard') }}</h1>
        <p class="subtitle">{{ t('notification.subtitle.dashboard') }}</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items" v-if="role!=2">
                <DashboardItem
                    :title="t('notification.tabs.search')"
                    :description="t('notification.subtitle.search')"
                    icon="search"
                    to="/vessel-visit-notifications/search"
                />
                <DashboardItem
                    :title="t('notification.tabs.review')"
                    :description="t('notification.subtitle.review')"
                    icon="rate_review"
                    to="/vessel-visit-notifications/review"
                />
            </div>
            <div class="dashboard-items" v-else>
                <DashboardItem
                    :title="t('notification.tabs.myNotifications')"
                    :description="t('notification.subtitle.search')"
                    icon="search"
                    to="/vessel-visit-notifications/search"
                />
                <DashboardItem
                    :title="t('notification.tabs.makeDraft')"
                    :description="t('notification.subtitle.create')"
                    icon="add"
                    to="/vessel-visit-notifications/create"
                />
            </div>
        </sl-card>
    </div>
</template>