<script setup lang="ts">
import VesselVisitNotificationPrinter from '@/components/printers/VesselVisitNotificationPrinter.vue';
import ListingBox from '@/components/crud/ListingBox.vue';
import { useRouter } from 'vue-router';
import type { Filter, Page } from '@/model/Page';
import type { VesselVisitNotification } from '@/model/VesselVisitNotification';
import AxiosHttpService from '@/service/AxiosHttpService';
import { VesselVisitNotificationService } from '@/service/VesselVisitNotificationService';
import { useI18n } from 'vue-i18n';

const http = new AxiosHttpService();
const vesselVisitNotificationService = new VesselVisitNotificationService(http as any);
const { t } = useI18n();

const fetchVesselVisitNotifications = async (filtering?: Filter<VesselVisitNotification>): Promise<Page<VesselVisitNotification>> => {
    // The service currently returns an array of notifications. ListingBox expects a Page<T>.
    // Wrap the array into a Page object so the listing component can render it.
    const data = await vesselVisitNotificationService.getVesselVisitNotifications();
    const page: Page<VesselVisitNotification> = {
        items: data || [],
        pageNumber: 1,
        pageSize: data ? data.length : 0,
        pageCount: data && data.length > 0 ? 1 : 0
    };
    return page;
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

            <ListingBox
                listingStyle="listing-grid"
                :fetch-function="fetchVesselVisitNotifications"
                search-filter="notificationNumber"
                v-slot="{ elements }"
            >
                <li v-for="notification in elements" :key="notification.notificationId" class="link">
                    <VesselVisitNotificationPrinter
                        class="listing-box"
                        :notification="notification"
                        :link="`/vessel-visit-notifications/view/${notification.notificationId}`"
                    />
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