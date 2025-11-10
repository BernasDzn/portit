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
import { onMounted, ref } from 'vue';

const http = new AxiosHttpService();
const vesselVisitNotificationService = new VesselVisitNotificationService(http as any);
const user = ref(useSession().authenticatedUser!);

const { t } = useI18n();

const fetchVesselVisitNotifications = async (filtering?: Filter<VesselVisitNotificationFilter>): Promise<Page<VesselVisitNotification>> => {
    // For now admins cant filter the notifications, they just get all of them
    if (user.value.role === 0) {
        return await vesselVisitNotificationService.getVesselVisitNotifications();
    } 

    return await vesselVisitNotificationService.getVesselVisitNotificationsByRepresentative(filtering);
};

const filterDefinition = ref({});
onMounted(async () => {
    filterDefinition.value = {
        Status: {
            type: 'select',
            label: t('notification.fields.status') as string,
            options: [
                { value: 0, text: t('notification.timeline.inProgress') as string },
                { value: 1, text: t('notification.timeline.pending') as string },
                { value: 2, text: t('notification.timeline.accepted') as string },
                { value: 3, text: t('notification.timeline.rejected') as string }
            ]
        },
        WithReason: {
            type: 'checkbox',
            label: t('notification.filters.withReason') as string
        },
        WithDockAssigned: {
            type: 'checkbox',
            label: t('notification.filters.withDockAssigned') as string
        },
        Vessel: {
            type: 'text',
            label: t('vessel.fields.imoNumber.title') as string
        },
        ExpectedArrivalFrom: {
            type: 'date',
            label: t('notification.filters.expectedArrivalFrom') as string
        },
        ExpectedArrivalTo: {
            type: 'date',
            label: t('notification.filters.expectedArrivalTo') as string
        }

    };
});

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
                search-filter="notificationNumber" v-slot="{ elements }" :filter-definition="user.role === 2 ? filterDefinition:null">
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