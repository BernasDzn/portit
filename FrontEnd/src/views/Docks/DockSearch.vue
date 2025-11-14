<script setup lang="ts">
import DockPrinter from '@/components/printers/DockPrinter.vue';
import ListingBox from '@/components/crud/ListingBox.vue';
import type { Filter, Page } from '@/model/Page';
import type { Dock } from '@/model/Dock';
import type { DockFilter } from '@/model/dto/DockDto';
import { onMounted, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import type { IDockService } from '@/service/IService/IDockService';
import TYPES from '@/inversify/types';
import type { IVesselTypeService } from '@/service/IService/IVesselTypeService';

const { t, locale } = useI18n();

const dockService = container.get<IDockService>(TYPES.dockService);
const vesselTypes = container.get<IVesselTypeService>(TYPES.vesselTypeService);

const fetchDocks = async (filtering?: Filter<DockFilter>): Promise<Page<Dock>> => {
    return await dockService.getDocks(filtering);
}

const filterDefinition = ref({});

async function buildFilterDefinition() {
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
}

onMounted(async () => buildFilterDefinition());
watch(locale, () => buildFilterDefinition());

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