<script setup lang="ts">

import Loading from '@/components/Loading.vue';
import Port3DHero from '@/components/Port3DHero.vue';
import { useI18n } from 'vue-i18n'
import { onMounted, ref } from 'vue';
import AxiosHttpService from '@/service/AxiosHttpService';
import { VesselService } from '@/service/VesselService';
import { DockService } from '@/service/DockService';

const http = new AxiosHttpService();
const vesselService = new VesselService(http);
const dockService = new DockService(http);

const numberOfVessels = ref(0);
const numberOfDocks = ref(0);

onMounted(async () => {
    try {
        numberOfVessels.value = await vesselService.getNumberOfVessels();
        numberOfDocks.value = await dockService.getNumberOfDocks();
    } catch (err) {
        console.error('Failed to load vessels', err);
    }
});

const { t } = useI18n();
const loading = ref(false);

</script>

<template>
  <div>
    <h1 class="title">{{ t('dashboard.title') }}</h1>
    <p class="subtitle">{{ t('dashboard.subtitle') }}</p>
      <sl-card style="width: 100%;">
        <div class="main-dashboard-statistics">
        <div class="stats-overview" v-if="!loading">
          <div class="opposed">
            <p>{{ numberOfVessels }}</p>
            <span class="material-icons icon" style="color: var(--accent-1);">directions_boat</span>
          </div>
          <p>{{ t('vessel.title') }}</p>
        </div>
        <div class="stats-overview" v-if="!loading">
          <div class="opposed">
            <p>8</p>
            <span class="material-icons icon" style="color: var(--accent-1);">sailing</span>
          </div>
          <p>{{ t('vesselType.title') }}</p>
        </div>
        <div class="stats-overview" v-if="!loading">
          <div class="opposed">
            <p>{{ numberOfDocks }}</p>
            <span class="material-icons icon" style="color: var(--accent-1);">anchor</span>
          </div>
          <p>{{ t('dock.title') }}</p>
        </div>
        <div class="stats-overview" v-if="!loading">
          <div class="opposed">
            <p>25</p>
            <span class="material-icons icon" style="color: var(--accent-1);">school</span>
          </div>
          <p>{{ t('qualification.title') }}</p>
        </div>
        <div class="stats-overview" v-if="!loading">
          <div class="opposed">
            <p>30</p>
            <span class="material-icons icon" style="color: var(--accent-1);">people</span>
          </div>
          <p>{{ t('staff.title') }}</p>
        </div>
        <Loading v-if="loading"/>
        </div>
      </sl-card>
  </div>
  <Port3DHero :route="'visualization'" />
</template>

<style scoped>

.main-dashboard-statistics {
  display: grid;
  gap: 1rem;
  width: 100%;
  margin: 0;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr) );
}

.stats-overview {
    text-align: left;
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