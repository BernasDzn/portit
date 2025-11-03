<script setup lang="ts">

import Loading from '@/components/Loading.vue';

const loading = false;
</script>

<template>
  <div>
    <h1 class="title">Portit dashboard</h1>
    <p class="subtitle">Overview of PortIT system</p>
    <sl-card style="width: 100%;">
      <div class="main-dashboard-statistics">
        <div class="stats-overview" v-if="!loading">
          <div class="opposed">
            <p>42</p>
            <span class="material-icons icon" style="color: var(--accent-1);">directions_boat</span>
          </div>
          <p>Vessels</p>
        </div>
        <div class="stats-overview" v-if="!loading">
          <div class="opposed">
            <p>8</p>
            <span class="material-icons icon" style="color: var(--accent-1);">sailing</span>
          </div>
          <p>Vessel Types</p>
        </div>
        <div class="stats-overview" v-if="!loading">
          <div class="opposed">
            <p>15</p>
            <span class="material-icons icon" style="color: var(--accent-1);">anchor</span>
          </div>
          <p>Docks</p>
        </div>
        <div class="stats-overview" v-if="!loading">
          <div class="opposed">
            <p>25</p>
            <span class="material-icons icon" style="color: var(--accent-1);">school</span>
          </div>
          <p>Qualifications</p>
        </div>
        <div class="stats-overview" v-if="!loading">
          <div class="opposed">
            <p>30</p>
            <span class="material-icons icon" style="color: var(--accent-1);">people</span>
          </div>
          <p>Staff Members</p>
        </div>
        <Loading v-if="loading"/>
      </div>
    </sl-card>
  </div>
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