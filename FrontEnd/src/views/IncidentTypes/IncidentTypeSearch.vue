<script setup lang="ts">
import IncidentTypePrinter from '@/components/printers/IncidentTypePrinter.vue';
import ListingBox from '@/components/crud/ListingBox.vue';
import type { Filter, Page } from '@/model/Page';
import type { IncidentType } from '@/model/IncidentType';
import { ref, watch, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import type { IIncidentTypeService } from '@/service/IService/IIncidentTypeService';
import TYPES from '@/inversify/types';

const { t, locale } = useI18n();

const incidentTypeService = container.get<IIncidentTypeService>(TYPES.incidentTypeService);

const fetchIncidentTypes = async (filtering?: Filter<any>): Promise<Page<IncidentType>> => {
    const incidentTypes = await incidentTypeService.getAllIncidentTypes();
    return {
        items: incidentTypes,
        pageNumber: 1,
        pageSize: incidentTypes.length,
        pageCount: 1,
    };
}

const filterDefinition = ref({});

async function buildFilterDefinition() {
    filterDefinition.value = {
        severity: {
            type: 'select',
            label: 'Severity',
            options: [
                { value: 'Minor', text: 'Minor' },
                { value: 'Major', text: 'Major' },
                { value: 'Critical', text: 'Critical' }
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
        <sl-breadcrumb-item><RouterLink to="/incident-types/dashboard" class="breadcrumb-link">Incident Types Dashboard</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item>Search Incident Types</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
        <h1 class="title">Incident Types</h1>
        <p class="subtitle">Manage and view all registered incident types</p>

        <ListingBox :fetch-function="fetchIncidentTypes" search-filter="name" v-slot="{elements}" :filter-definition="filterDefinition">
            <li v-for="incidentType in elements" :key="incidentType.id">
                <IncidentTypePrinter class="listing-box" :incident-type="incidentType" :link="`/incident-types/edit/${incidentType.id}`"/>
            </li>
        </ListingBox>
    </header>
</div>
</template>
