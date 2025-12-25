<script setup lang="ts">
import ListingBox from '@/components/crud/ListingBox.vue';
import { useI18n } from 'vue-i18n'
import type { Filter, Page } from '@/model/Page';
import type { VesselType } from '@/model/VesselType';
import VesselTypePrinter from '@/components/printers/VesselTypePrinter.vue';
import { container } from '@/inversify.config';
import type { IVesselTypeService } from '@/service/IService/IVesselTypeService';
import TYPES from '@/inversify/types';
import type { IVesselVisitExecutionService } from '@/service/IService/IVesselExecutionService';
import type { VesselVisitExecutionFilter } from '@/model/dto/VesselVisitExecutionDto';
import type { VesselVisitExecution } from '@/model/VesselVisitExecution';
import VesselVisitExecutionPrinter from '@/components/printers/VesselVisitExecutionPrinter.vue';
import VesselVisitExecutionTable from '@/components/printers/VesselVisitExecutionTable.vue';
import { ref, watch, onMounted } from 'vue';

const {t, locale} = useI18n();

const vveService = container.get<IVesselVisitExecutionService>(TYPES.vesselVisitExecutionService);

const fetchVesselExecutions = async (filtering?: Filter<VesselVisitExecutionFilter>): Promise<Page<VesselVisitExecution>> => {
    return await vveService.getAllVesselVisitExecutions(filtering);
}

const filterDefinition = ref({});
function buildFilterDefinition() {
    filterDefinition.value = {
        startDate: {
            type: 'date',
            label: t('execution.filters.startDate') as string
        },
        endDate: {
            type: 'date',
            label: t('execution.filters.endDate') as string
        },
        relatedVVN: {
            type: 'text',
            label: t('execution.filters.relatedVVN') as string
        },
        status: {
            type: 'select',
            label: t('execution.filters.status') as string,
            options: [
                { value: 'Open', text: 'Open' },
                { value: 'Closed', text: 'Closed' }
            ]
        }
    };
}

onMounted(async () => buildFilterDefinition());
watch(locale, () => buildFilterDefinition());

</script>

<template>
<div>

    <sl-breadcrumb>
        <sl-breadcrumb-item>
            <RouterLink to="../vessel-visit-executions/dashboard" class="breadcrumb-link">{{ t('execution.tabs.dashboard') }}</RouterLink>
        </sl-breadcrumb-item>
        <sl-breadcrumb-item>{{ t('execution.tabs.search') }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
        <div style="display:flex; justify-content:space-between; align-items:center;">
            <div>
                <h1 class="title">{{ t('execution.title') }}</h1>
                <p class="subtitle">{{ t('execution.subtitle.search') }}</p>
            </div>
        </div>

        <sl-tab-group>
            <sl-tab slot="nav" panel="table">{{ t('execution.tabs.table') }}</sl-tab>
            <sl-tab slot="nav" panel="cards">{{ t('execution.tabs.cards') }}</sl-tab>

            <sl-tab-panel name="table">
                <ListingBox 
                    listing-style="custom" 
                    :fetch-function="fetchVesselExecutions" 
                    :filter-definition="filterDefinition"
                    v-slot="{elements}">
                    <VesselVisitExecutionTable :executions="elements" />
                </ListingBox>
            </sl-tab-panel>

            <sl-tab-panel name="cards">
                <ListingBox 
                    listing-style="listing-triples" 
                    :fetch-function="fetchVesselExecutions" 
                    :filter-definition="filterDefinition"
                    v-slot="{elements}">
                    <li v-for="vt in elements" :key="vt.id">
                        <VesselVisitExecutionPrinter class="listing-box" :execution="vt" :link="`/vessel-visit-executions/${vt.relatedVVN}`"/>
                    </li>
                </ListingBox>
            </sl-tab-panel>
        </sl-tab-group>
    </header>
</div>
</template>