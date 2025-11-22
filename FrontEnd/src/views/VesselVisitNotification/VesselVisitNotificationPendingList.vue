<script setup lang="ts">
import VesselVisitNotificationPrinter from '@/components/printers/VesselVisitNotificationPrinter.vue';
import ListingBox from '@/components/crud/ListingBox.vue';
import { useRouter } from 'vue-router';
import type { Filter, Page } from '@/model/Page';
import type { VesselVisitNotification } from '@/model/VesselVisitNotification';
import { useI18n } from 'vue-i18n';
import { useSession } from '@/composables/session';
import { onMounted, ref } from 'vue';
import type { IVesselVisitNotificationService } from '@/service/IService/IVesselVisitNotificationService';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';

const router = useRouter();

const vesselVisitNotificationService = container.get<IVesselVisitNotificationService>(TYPES.vesselVisitNotificationService);
const user = ref(useSession().authenticatedUser!);

const { t } = useI18n();

const fetchVesselVisitNotifications = async (filter: Filter<null>): Promise<Page<VesselVisitNotification>> => {
        return await vesselVisitNotificationService.getVesselVisitNotificationsForReview(filter);
};

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/vessel-visit-notifications/dashboard" class="breadcrumb-link">
                    {{ t('notification.tabs.review') }}
                </RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('notification.tabs.review') }}</sl-breadcrumb-item>
        </sl-breadcrumb>

        <header>
            <h1 class="title">{{ t('notification.tabs.review') }}</h1>
            <p class="subtitle">{{ t('notification.subtitle.review') }}</p>

            <ListingBox class="listing-container" listingStyle="listing-grid" :fetch-function="fetchVesselVisitNotifications"
                search-filter="notificationNumber" v-slot="{ elements }">
                <li v-for="notification in elements" :key="notification.notificationId" class="link">
                    <VesselVisitNotificationPrinter class="listing-box" :notification="notification"
                        :link="`/vessel-visit-notifications/review/${notification.notificationId}`" review/>
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