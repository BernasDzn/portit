<script setup lang="ts">
import VesselPrinter from '@/components/printers/VesselPrinter.vue';
import ListingBox from '@/components/ListingBox.vue';
import { useRouter } from 'vue-router';
import type { Filter, Page } from '@/model/Page';
import type { Vessel } from '@/model/Vessel';
import AxiosHttpService from '@/service/AxiosHttpService';
import { VesselService } from '@/service/VesselService';

const http = new AxiosHttpService()
const vesselService = new VesselService(http as any)

const fetchVessels = async (filtering?: Filter<Vessel>): Promise<Page<Vessel>> => {
  return await vesselService.getVessels(filtering);
}

const router = useRouter();

function goToVessel(imo: string) {
  if (!imo) return;
  // use named route to avoid hard-coded path issues and ensure correct params
  router.push({ name: 'viewVessel', params: { imo } });
}

</script>

<template>
  <div>

    <sl-breadcrumb>
      <sl-breadcrumb-item><RouterLink to="/vessels/dashboard" class="breadcrumb-link">Vessel Dashboard</RouterLink></sl-breadcrumb-item>
      <sl-breadcrumb-item>Search Vessels</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
      <h1 class="title">Search Vessels</h1>
      <p class="subtitle">Search registered vessels</p>

      <ListingBox :fetch-function="fetchVessels" search-filter="name" v-slot="{elements}">
        <li v-for="vessel in elements" :key="vessel.imo">
          <VesselPrinter class="listing-box" :vessel="vessel" :link="`/vessels/view/${vessel.imoNumber}`" />
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