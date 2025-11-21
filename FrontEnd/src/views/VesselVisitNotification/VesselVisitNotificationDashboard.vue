<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
import Loading from '@/components/Loading.vue';
import { useSession } from '@/composables/session';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { VesselVisitDistributionDto } from '@/model/dto/VesselVisitNotificationDto';
import type { IVesselVisitNotificationService } from '@/service/IService/IVesselVisitNotificationService';
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n'

const role = ref(useSession().authenticatedUser!.role);
const vvnService = container.get<IVesselVisitNotificationService>(TYPES.vesselVisitNotificationService);
const { t } = useI18n()

const distribution = ref<VesselVisitDistributionDto>({
    pending: 0,
    accepted: 0,
    rejected: 0
});
const numberOfVesselVisitNotifications = ref(0);
const loading = ref(true)
onMounted(async () => {
    try {
        distribution.value = await vvnService.count();
        loading.value = false;
        numberOfVesselVisitNotifications.value = distribution.value.accepted + distribution.value.pending + distribution.value.rejected;
    } catch (err) {
        console.error('Failed to load vessel visit notifications', err);
    }
});
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
                    to="/vessel-visit-notifications/pending"
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
        <sl-card class="dashboard-statistics">
            <div class="stats-overview" v-if="!loading">
                <p>{{numberOfVesselVisitNotifications}}</p>
                <p>{{ t('notification.registeredVesselVisitNotifications') }}</p>
            </div>
            <Loading v-if="loading"/>
        </sl-card>
    </div>
</template>

<style scoped>

.stats-overview {
    text-align: center;
    margin-bottom: 1rem;
    border-radius: 0.5rem;
    padding: 1rem;
    background-color: #f5f6fa;
}

.stats-overview p {
    margin: 0;
    padding: 0.5rem;
    color: #485ea9;
}

.stats-overview p:nth-child(1) {
    font-size: 2rem;
}
</style>