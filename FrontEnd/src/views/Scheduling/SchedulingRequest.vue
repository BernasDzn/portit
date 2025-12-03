<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import { computed, onMounted, ref, watch } from 'vue'
import type { Schedule } from '@/model/values/Schedule';
import type { VesselVisitNotification } from '@/model/VesselVisitNotification';
import type { IVesselVisitNotificationService } from '@/service/IService/IVesselVisitNotificationService';
import type { ISchedulingService } from '@/service/IService/ISchedulingService';
import CalendarEvents from '@/components/crud/CalendarEvents.vue';
import VesselVisitNotificationPrinter from '@/components/printers/VesselVisitNotificationPrinter.vue';
import Loading from '@/components/Loading.vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import type { IDockService } from '@/service/IService/IDockService';
import { useAlerts } from '@/composables/alerts';
import { useRouter } from 'vue-router';

const vvnService = container.get<IVesselVisitNotificationService>(TYPES.vesselVisitNotificationService);
const scheduleService = container.get<ISchedulingService>(TYPES.schedulingService);

const notifications = useAlerts();
const router = useRouter();

const { t } = useI18n();

const algorithmList = [
    { label: "Auto (Recommended) - Selects best algorithm based on problem size", value: "auto" },
    { label: "Optimal Scheduling (Exhaustive)", value: "optimal" },
    { label: "Greedy Scheduling (Fast - EDD)", value: "greedy" },
    { label: "Genetic Scheduling (Generational)", value: "genetic" }
]

const selectedDate = ref<Date | null>(new Date());
const selectedAlgorithm = ref<string | null>("auto");
const vvnList = ref<VesselVisitNotification[]>([]);
const daysAhead = ref<number>(1);
const loading = ref(false);
const generating = ref(false);

const fetchVVNs = async () => {

    loading.value = true;

    const response = await vvnService.getVesselVisitNotifications();
    vvnList.value = response.items;

    events.value = vvnList.value.map(vvn => ({
        title: vvn.vessel.name,
        start: vvn.expectedArrival,
    }));

    loading.value = false;
};

onMounted(async () => {
    await fetchVVNs();
});

const generateTasksForDate = async () => {
    
    if (!selectedDate.value || !selectedAlgorithm.value) return;
    generating.value = true;

    try {
        
        const results = await scheduleService.scheduleForDay(selectedDate.value, selectedAlgorithm.value, daysAhead.value);
        console.log('Generated Schedule:', results);

        notifications.enqueueNotification(`${results.message}`, notifications.notificationTypes.SUCCESS);
        generating.value = false;
        closeModal();

        // Goto queue
        router.push({
            name: 'Scheduling Queue'
        });

    } catch (error) {
        
        notifications.enqueueNotification(`An error occurred while generating the schedule. ${error}`, notifications.notificationTypes.DANGER);
    }

    // if (!Array.isArray(results.data)){
    //     notifications.enqueueNotification("An error occurred while generating the schedule. " + results.data, notifications.notificationTypes.DANGER);
    //     generating.value = false;
    //     return;
    // }

    // generating.value = false;

    // if (results.data.length > 0) {
        
    //     const algorithmLabel = algorithmList.find(a => a.value === selectedAlgorithm.value)?.label || selectedAlgorithm.value;
    //     let message = `Successfully generated ${results.data.length} tasks using ${algorithmLabel}.`;
        
    //     // Add metrics to notification if available
    //     if (results.metrics) {
    //         message += ` Total delay: ${results.metrics.totalDelay}h, Computation time: ${(results.metrics.computationTime * 1000).toFixed(2)}ms`;
    //     }
        
    //     notifications.enqueueNotification(message, notifications.notificationTypes.SUCCESS);
    //     closeModal();

    //     // Generate and open schedule pdf
    //     // const pdfResponse = await scheduleService.generateSchedulePDF(results, selectedDate.value);
    //     // const pdfBlob = new Blob([pdfResponse], { type: 'application/pdf' });
    //     // const pdfUrl = URL.createObjectURL(pdfBlob);
    //     // window.open(pdfUrl, '_blank');
    //     router.push({
    //         name: 'ScheduleResults',
    //         query: {
    //             schedule: JSON.stringify(results),
    //             date: selectedDate.value.toISOString()
    //         }
    //     });

    // } else {
        
    //     notifications.enqueueNotification("The requested schedule came back empty, nothing to do on that dock at this time.", notifications.notificationTypes.NEUTRAL);
    // }
};

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

const openModal = () => {
    const dialog = document.getElementById('generate-tasks-modal') as any;
    dialog.show();
};

const closeModal = () => {
    const dialog = document.getElementById('generate-tasks-modal') as any;
    dialog.hide();
};

const openAboutModal = () => {

    // Download PDF
    const path = "/IARTI Complexity Report 3DJ G001.pdf";
    const link = document.createElement('a');
    link.href = path;
    link.download = 'IARTI Complexity Report 3DJ G001.pdf';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    // const dialog = document.getElementById('about') as any;
    // dialog.show();
};

const closeAboutModal = () => {
    const dialog = document.getElementById('about') as any;
    dialog.hide();
};

</script>

<template>
    <div class="full-height">

        <sl-breadcrumb>
            <sl-breadcrumb-item><RouterLink to="/scheduling-dashboard" class="breadcrumb-link">{{ t('scheduling.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('scheduling.tabs.schedule') }}</sl-breadcrumb-item>
          </sl-breadcrumb>

        <h1 class="title">{{ t("scheduling.title") }}</h1>
        <p class="subtitle">{{ t("scheduling.subtitle") }}</p>

        <Loading v-if="loading" />
        <div v-else>

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
        </div>

        <br>

        <sl-button v-if="vvnsOnDate.length != 0" variant="default" style="margin-right: 20px;" @click="openAboutModal">
            {{ t('scheduling.aboutAlgorithmButton') }}
        </sl-button>

        <sl-button v-if="vvnsOnDate.length != 0" variant="primary" @click="openModal">
            {{ t('scheduling.generateTasksButton') }}
        </sl-button>

        <!-- Dock select modal -->

        <sl-dialog :label="t('scheduling.generateTasksModal.title')" class="dialog-overview" id="generate-tasks-modal">

            <div>
                <!-- Select algorithm -->
                <EntityDropdown
                    class="field-dropdown"
                    :name="t('scheduling.fields.algorithm.title')"
                    v-model="selectedAlgorithm"
                    :items="algorithmList"
                    valueKey="value"
                    labelKey="label"
                    :placeholderText="t('scheduling.fields.algorithm.placeholder')"
                    required
                />

                <br>

                <!-- Days ahead input -->
                <EntityDropdown
                    class="field-dropdown"
                    name="Days Ahead"
                    v-model="daysAhead"
                    :items="[
                        {
                            label: 'Today',
                            value: 1
                        },
                        {
                            label: 'Next 3 Days',
                            value: 3
                        },
                        {
                            label: 'A week from now',
                            value: 7
                        },
                        {
                            label: 'A month from now',
                            value: 30
                        }
                    ]"
                    :placeholderText="t('scheduling.fields.algorithm.placeholder')"
                    valueKey="value"
                    labelKey="label"
                    required
                />
            </div>

            <sl-button slot="footer" variant="danger" @click="closeModal">
                {{ t('buttons.cancel') }}
            </sl-button>
            <sl-button 
                :disabled="!selectedAlgorithm || !selectedDate || generating"
                :loading="generating"
                slot="footer" 
                variant="primary" 
                @click="generateTasksForDate"
            >
                {{ t('buttons.generate') }}
            </sl-button>

        </sl-dialog>

        <sl-dialog :label="t('scheduling.aboutAlgorithmModal.title')" class="dialog-overview" id="about">
            {{ t('scheduling.aboutAlgorithmModal.content') }}

            <sl-button slot="footer" variant="primary" @click="closeAboutModal">
                {{ t('buttons.close') }}
            </sl-button>
        </sl-dialog>
    </div>
</template>


<style scoped>

.full-height {
  height: auto;
}

.date-selector {
    margin-top: 20px;
    display: flex;

    flex-direction: row;
    align-items: center;
    gap: 20px;
}


.calendar-events {
    margin-top: 1rem;
    margin-left: auto;
    margin-right: auto;
    display: flex;
    align-items: flex-start;
    justify-content: center;
    gap: 5rem;
}

.calendar {
    min-width: 350px;
    height: 600px;
}

.mt-4 {
    flex: 1;
    height: 600px;
    overflow-y: auto;
    scrollbar-width: none;
    -ms-overflow-style: none;

    padding: 0 30px;
}

.mt-4::-webkit-scrollbar {
    display: none;
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