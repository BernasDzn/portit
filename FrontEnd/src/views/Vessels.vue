<script setup lang="ts">
import VesselPrinter from '@/components/printers/VesselPrinter.vue';
import ListingBox from '@/components/ListingBox.vue';
import type { Filter, Page } from '@/model/Page';
import type { Vessel } from '@/model/Vessels';
import AxiosHttpService from '@/service/AxiosHttpService';
import { VesselService } from '@/service/VesselService';

const http = new AxiosHttpService()
const vesselService = new VesselService(http as any)

const fetchVessels = async (filtering?: Filter<Vessel>): Promise<Page<Vessel>> => {
    return await vesselService.getVessels(filtering);
}

</script>

<template>
  <div>

    <sl-breadcrumb>
      <sl-breadcrumb-item>Vessels</sl-breadcrumb-item>
      <sl-breadcrumb-item>Listings</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
      <h1 class="title">Vessels</h1>
      <p class="subtitle">Manage registered vessels</p>

      <ListingBox :fetch-function="fetchVessels" search-filter="name" v-slot="{elements}">
      <li v-for="vessel in elements" :key="vessel.imo">
        <VesselPrinter class="listing-item" :vessel="vessel">
          <p class="item-description">{{ vessel.imoNumber }}</p>
          <p class="item-description">{{ vessel.type.name }}</p>
          <p class ="item-description">{{ vessel.owner.name }}</p>
        </VesselPrinter>
      </li>
    </ListingBox>
    </header>
  </div>
</template>