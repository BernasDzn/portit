<script setup lang="ts">
import DockPrinter from '@/components/printers/DockPrinter.vue';
import ListingBox from '@/components/crud/ListingBox.vue';
import type { Filter, Page } from '@/model/Page';
import type { Dock, DockFilter } from '@/model/Dock';
import AxiosHttpService from '@/service/AxiosHttpService';
import { DockService } from '@/service/DockService';
import { VesselTypeService } from '@/service/VesselTypeService';
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

const http = new AxiosHttpService()
const dockService = new DockService(http)
const vesselTypes = new VesselTypeService(http);

const fetchDocks = async (filtering?: Filter<DockFilter>): Promise<Page<Dock>> => {
    return await dockService.getDocks(filtering);
}

const filterDefinition = ref({});

onMounted(async () => {
    const vtPage = await vesselTypes.getVesselTypes();
    const optionTypes = vtPage.items.map(vt => ({ value: encodeURIComponent(vt.name), text: vt.name }));

    filterDefinition.value = {
        location: {
            type: 'text',
            label: t('dock.fields.location.title') as string,
        },
        vesselTypeName: {
            type: 'select',
            label: t('dock.fields.supportedVesselTypes.vesselType') as string,
            options: optionTypes,
        }
    };
});

</script>

<template>
<div>

    <sl-breadcrumb>
        <sl-breadcrumb-item><RouterLink to="../docks/dashboard" class="breadcrumb-link">{{ t('dock.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item>{{ t('dock.tabs.search') }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
        <h1 class="title">{{ t('dock.title') }}</h1>
        <p class="subtitle">{{ t('dock.subtitle.search') }}</p>

        <ListingBox :fetch-function="fetchDocks" search-filter="dockName" v-slot="{elements}" :filter-definition="filterDefinition">
            <li v-for="dock in elements" :key="dock.code">
                <DockPrinter class="listing-box" :dock="dock" :link="`/docks/view/${dock.code}`"/>
            </li>
        </ListingBox>
    </header>
</div>
</template>