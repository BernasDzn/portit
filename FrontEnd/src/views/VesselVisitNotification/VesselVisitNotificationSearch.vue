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
import { computed, onMounted, ref, watch } from 'vue';
import CalendarEvents from '@/components/crud/CalendarEvents.vue';

const http = new AxiosHttpService();
const vesselVisitNotificationService = new VesselVisitNotificationService(http as any);
const user = ref(useSession().authenticatedUser!);

const { t } = useI18n();

const vvnList = ref<VesselVisitNotification[]>([]);

const fetchVesselVisitNotifications = async (filtering?: Filter<VesselVisitNotificationFilter>): Promise<Page<VesselVisitNotification>> => {
    // For now admins cant filter the notifications, they just get all of them
    if (user.value.role === 0) {
    
        // Get events
        const tempList = (await vesselVisitNotificationService.getVesselVisitNotifications());
        events.value = tempList.items.map(ev => ({
            title: ev.vessel.name,
            start: ev.expectedArrival.toString(),
            end: ev.expectedDeparture.toString()
        }));

        vvnList.value = tempList.items;
        return await tempList;
    } 

    // Get events
    const tempFiltering: Filter<VesselVisitNotificationFilter> = {
        pageNumber: 1,
        pageSize: 1000,
        filter: filtering?.filter
    };

    const tempList = await vesselVisitNotificationService.getVesselVisitNotificationsByRepresentative(tempFiltering);
    events.value = tempList.items.map(ev => ({
            title: ev.vessel.name,
            start: ev.expectedArrival.toString(),
    }));

    vvnList.value = tempList.items;
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

const selectedDate = ref(new Date())
const events = ref<Array<{ title: string, start: string }>>([]);
    const vvnsOnDate = computed(() => {
    if (!selectedDate.value) return [];
    const selected = selectedDate.value;
    return vvnList.value.filter(vvn => {
        const arrival = new Date(vvn.expectedArrival);
        return arrival.getFullYear() === selected.getFullYear() &&
            arrival.getMonth() === selected.getMonth() &&
            arrival.getDate() === selected.getDate();
    });
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

            <sl-tab-group>
                <sl-tab slot="nav" panel="general"> {{ t('notification.tabs.general') }} </sl-tab>
                <sl-tab slot="nav" panel="custom"> {{ t('notification.tabs.byDate') }} </sl-tab>
              
                <sl-tab-panel name="general">
                    <ListingBox listingStyle="listing-grid" :fetch-function="fetchVesselVisitNotifications"
                    search-filter="notificationNumber" v-slot="{ elements }" :filter-definition="user.role === 2 ? filterDefinition:null">
                    <li v-for="notification in elements" :key="notification.notificationId" class="link">
                        <VesselVisitNotificationPrinter class="listing-box" :notification="notification"
                            :link="`/vessel-visit-notifications/view/${notification.notificationId}`" />
                    </li>
                </ListingBox>
                </sl-tab-panel>
                <sl-tab-panel name="custom">

                    <div class="calendar-events">
                        <CalendarEvents class="calendar" :events="events" v-model="selectedDate" />
                        <div class="mt-4">
                            <h2 class="subtitle">{{ t('notification.eventsOnDate', { date: selectedDate.toDateString() }) }}</h2>
                            <ul v-if="vvnsOnDate.length !== 0">
                                <li v-for="vvn in vvnsOnDate" :key="vvn.notificationId">
                                    <VesselVisitNotificationPrinter class="listing-box" 
                                        :notification="vvn"
                                        :link="`/vessel-visit-notifications/view/${vvn.notificationId}`" 
                                        :short="true"
                                    />
                                </li>
                            </ul>
                            <p v-else>{{ t('notification.noEventsOnDate') }}</p>
                        </div>
                    </div>

                </sl-tab-panel>
            </sl-tab-group>
        </header>
    </div>
</template>

<style scoped>
.link {
    text-decoration: none;
    color: inherit;
}

.calendar-events {
    margin-top: 1rem;
    margin-left: auto;
    margin-right: auto;

    display: flex;
    align-items: center;
    
    justify-content: center;
    gap: 5rem;
}

.calendar {
    min-width: 350px;
}

.calendar-events ul {
    list-style-type: none;
    padding: 0;
    margin: 0;
}

.calendar-events li {
    margin-bottom: 1rem;
}

</style>