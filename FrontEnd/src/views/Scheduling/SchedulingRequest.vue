<script setup lang="ts">
import DatePicker from '@/components/DatePicker.vue';
import { useI18n } from 'vue-i18n';
import { computed, ref, watch } from 'vue'
import AxiosHttpService from '@/service/AxiosHttpService';
import { VesselVisitNotificationService } from '@/service/VesselVisitNotificationService';
import { SchedulingService } from '@/service/SchedulingService';
import type { Schedule } from '@/model/Schedule';

const http = new AxiosHttpService();
const vvnService = new VesselVisitNotificationService(http);

const scheduleHttp = new AxiosHttpService("http://localhost:8080");
const scheduleService = new SchedulingService(scheduleHttp);

const { t } = useI18n();

const selectedDate = ref<Date | null>(null);
const numberOfVVNonDay = ref<number>(0);
const loading = ref<boolean>(false);

watch(selectedDate, async (newDate) => {
  if (!newDate) {
    numberOfVVNonDay.value = 0;
    return;
  }

  const startOfDay = new Date(newDate);
  startOfDay.setHours(0, 0, 0, 0);

  
  try {
        
        loading.value = true;
        const res = await vvnService.getVesselVisitNotifcationsByDay(startOfDay);
        numberOfVVNonDay.value = res.length;
        console.log('Vessel Visit Notifications for', startOfDay, ':', res);
        loading.value = false;

  } catch (err) {
    console.error('Error fetching VVN:', err);
    numberOfVVNonDay.value = 0;
    loading.value = false;
  }
});

const generateTasksForDate = async () => {
    // Placeholder function to generate tasks for the selected date
    const results: Schedule = await scheduleService.scheduleForDay(selectedDate.value!);
    console.log('Generated Schedule:', results);
};

</script>

<template>
    <div class="full-height">
        <h1 class="title">{{ t("scheduling.title") }}</h1>
        <p class="subtitle">{{ t("scheduling.subtitle") }}</p>

        <div class="date-selector">
            <DatePicker v-model="selectedDate" />
            <p> {{ t("scheduling.visitsOnSelectedDay") }}
                <span v-if="loading">...</span>
                <span v-else>{{ numberOfVVNonDay }}</span>
            </p>
        </div>

        <sl-divider></sl-divider>

        <div v-if="numberOfVVNonDay > 0">
            <sl-button @click="generateTasksForDate" variant="primary" :disabled="loading">
                {{ t("scheduling.viewVisits") }}
            </sl-button>
        </div>
        <div v-else>
            <p>{{ t("scheduling.noVisits") }}</p>
        </div>
    </div>
</template>


<style scoped>

.full-height {
    height: 100vh
}

.date-selector {
    margin-top: 20px;
    display: flex;

    flex-direction: row;
    align-items: center;
    gap: 20px;
}

</style>