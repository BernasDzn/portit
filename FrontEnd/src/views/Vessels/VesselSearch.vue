<script setup lang="ts">
import VesselPrinter from '@/components/printers/VesselPrinter.vue';
import ListingBox from '@/components/crud/ListingBox.vue';
import type { Filter, Page } from '@/model/Page';
import type { Vessel } from '@/model/Vessel';
import { useI18n } from 'vue-i18n';
import { ref, onMounted, watch } from 'vue';
import { container } from '@/inversify.config';
import type { IVesselService } from '@/service/IService/IVesselService';
import TYPES from '@/inversify/types';

const vesselService = container.get<IVesselService>(TYPES.vesselService);
const { t, locale } = useI18n();

const fetchVessels = async (filtering?: Filter<Vessel>): Promise<Page<Vessel>> => {
  return await vesselService.getVessels(filtering);
}

const filterDefinition = ref({});

function buildFilterDefinition() {
  filterDefinition.value = {
    imoNumber: {
      type: 'text',
      label: t('vessel.fields.imoNumber.title'),
    },
    taxNumber: {
      type: 'text',
      label: t('vessel.fields.taxNumber.title'),
    }
  };
}

onMounted(() => buildFilterDefinition());
watch(locale, () => buildFilterDefinition());

</script>

<template>
  <div>

    <sl-breadcrumb>
      <sl-breadcrumb-item><RouterLink to="/vessels/dashboard" class="breadcrumb-link">{{ t('vessel.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
      <sl-breadcrumb-item>{{ t('vessel.tabs.search') }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
      <h1 class="title">{{ t('vessel.tabs.search') }}</h1>
      <p class="subtitle">{{ t('vessel.subtitle.search') }}</p>

      <ListingBox listingStyle="listing-grid" :fetch-function="fetchVessels" search-filter="name" v-slot="{elements}" :filter-definition="filterDefinition">
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