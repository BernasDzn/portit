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

const {t} = useI18n();

const vveService = container.get<IVesselVisitExecutionService>(TYPES.vesselVisitExecutionService);

const fetchVesselExecutions = async (filtering?: Filter<VesselVisitExecutionFilter>): Promise<Page<VesselVisitExecution>> => {
    return await vveService.getAllVesselVisitExecutions(filtering);
}

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

        <ListingBox listing-style="listing-triples" :fetch-function="fetchVesselExecutions" v-slot="{elements}">
            <li v-for="vt in elements" :key="vt.id">
                <VesselVisitExecutionPrinter :vessel-visit-execution="vt" />
            </li>
        </ListingBox>
    </header>
</div>
</template>