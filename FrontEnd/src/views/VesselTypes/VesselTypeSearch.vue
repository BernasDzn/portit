<script setup lang="ts">
import VesselPrinter from '@/components/printers/VesselPrinter.vue';
import ListingBox from '@/components/crud/ListingBox.vue';
import { useRouter } from 'vue-router';
import type { Filter, Page } from '@/model/Page';
import type { VesselType } from '@/model/VesselType';
import AxiosHttpService from '@/service/AxiosHttpService';
import { VesselTypeService } from '@/service/VesselTypeService';
import VesselTypePrinter from '@/components/printers/VesselTypePrinter.vue';

const http = new AxiosHttpService()
const vesselTypeService = new VesselTypeService(http as any)

const fetchVesselTypes = async (filtering?: Filter<VesselType>): Promise<Page<VesselType>> => {
  return await vesselTypeService.getVesselTypes(filtering);
}

</script>

<template>
<div>

    <sl-breadcrumb>
        <sl-breadcrumb-item><RouterLink to="../vessel-types/dashboard" class="breadcrumb-link">Vessel Type Dashboard</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item>Search Vessel Types</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
        <h1 class="title">Vessel Types</h1>
        <p class="subtitle">Search all vessel types</p>

        <ListingBox listing-style="listing-triples" :fetch-function="fetchVesselTypes" search-filter="name" v-slot="{elements}">
            <li v-for="vt in elements" :key="vt.id">
                <VesselTypePrinter class="listing-box" :vtype="vt" />
            </li>
        </ListingBox>
    </header>
</div>
</template>