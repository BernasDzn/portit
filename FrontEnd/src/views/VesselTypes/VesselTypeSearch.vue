<script setup lang="ts">
import ListingBox from '@/components/crud/ListingBox.vue';
import { useI18n } from 'vue-i18n'
import type { Filter, Page } from '@/model/Page';
import type { VesselType } from '@/model/VesselType';
import VesselTypePrinter from '@/components/printers/VesselTypePrinter.vue';
import { container } from '@/inversify.config';
import type { IVesselTypeService } from '@/service/IService/IVesselTypeService';
import TYPES from '@/inversify/types';
import { ref, onMounted, watch } from 'vue';

const {t, locale} = useI18n();

const vesselTypeService = container.get<IVesselTypeService>(TYPES.vesselTypeService);

const fetchVesselTypes = async (filtering?: Filter<VesselType>): Promise<Page<VesselType>> => {
  return await vesselTypeService.getVesselTypes(filtering);
}

const filterDefinition = ref({});

function buildFilterDefinition() {
    filterDefinition.value = {
        description: {
            type: 'text',
            label: t('vesselType.fields.description.title') as string,
        }
    };
}

onMounted(() => buildFilterDefinition());
watch(locale, () => buildFilterDefinition());

</script>

<template>
<div>

    <sl-breadcrumb>
        <sl-breadcrumb-item>
            <RouterLink to="../vessels/dashboard" class="breadcrumb-link">{{ t('vessel.tabs.dashboard') }}</RouterLink>
        </sl-breadcrumb-item>
        <sl-breadcrumb-item>{{ t('vesselType.tabs.search') }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
        <div style="display:flex; justify-content:space-between; align-items:center;">
            <div>
                <h1 class="title">{{ t('vesselType.title') }}</h1>
                <p class="subtitle">{{ t('vesselType.subtitle.search') }}</p>
            </div>
        </div>

        <ListingBox listing-style="listing-triples" :fetch-function="fetchVesselTypes" search-filter="name" v-slot="{elements}" :filter-definition="filterDefinition">
            <li v-for="vt in elements" :key="vt.id">
                <VesselTypePrinter class="listing-box" :vtype="vt" :link="`/vessel-types/view/${vt.name}`" />
            </li>
        </ListingBox>
    </header>
</div>
</template>