<script setup lang="ts">
import VesselVisitNotificationPrinter from '@/components/printers/VesselVisitNotificationPrinter.vue';
import ListingBox from '@/components/crud/ListingBox.vue';
import { useRouter } from 'vue-router';
import type { Filter, Page } from '@/model/Page';
import type { VesselVisitNotification, VesselVisitNotificationFilter } from '@/model/VesselVisitNotification';
import AxiosHttpService from '@/service/AxiosHttpService';
import { VesselVisitNotificationService } from '@/service/VesselVisitNotificationService';
import { useI18n } from 'vue-i18n';
import { useSession } from '@/composables/session';
import { RepresentativeService } from '@/service/RepresentativeService';
import { ref } from 'vue';

const http = new AxiosHttpService();
const vesselVisitNotificationService = new VesselVisitNotificationService(http as any);
const representativeService = new RepresentativeService(http as any);
const user = ref(useSession().authenticatedUser!);

const { t } = useI18n();

const fetchVesselVisitNotifications = async (filtering?: Filter<VesselVisitNotificationFilter>): Promise<Page<VesselVisitNotification>> => {
    // For now admins cant filter the notifications, they just get all of them
    if (user.value.role === 0) {
        return await vesselVisitNotificationService.getVesselVisitNotifications();
    } 

    const representative = await representativeService.getByEmail(user.value.email);
    return await vesselVisitNotificationService.getVesselVisitNotificationsByRepresentative(representative.citizenshipId, filtering);
};
</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/vessel-visit-notifications/dashboard" class="breadcrumb-link">
                    {{ t('notification.tabs.dashboard') }}
                </RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('notification.tabs.search') }}</sl-breadcrumb-item>
        </sl-breadcrumb>

        <header>
            <h1 class="title">{{ t('notification.tabs.search') }}</h1>
            <p class="subtitle">{{ t('notification.subtitle.search') }}</p>

            <ListingBox listingStyle="listing-grid" :fetch-function="fetchVesselVisitNotifications"
                search-filter="notificationNumber" v-slot="{ elements }">
                <li v-for="notification in elements" :key="notification.notificationId" class="link">
                    <VesselVisitNotificationPrinter class="listing-box" :notification="notification"
                        :link="`/vessel-visit-notifications/view/${notification.notificationId}`" />
                </li>
            </ListingBox>
        </header>
    </div>
</template>

<style scoped>
.link {
    text-decoration: none;
    color: inherit;
}
</style>