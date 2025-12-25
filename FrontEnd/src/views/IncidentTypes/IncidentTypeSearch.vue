<script setup lang="ts">
import IncidentTypePrinter from '@/components/printers/IncidentTypePrinter.vue';
import ListingBox from '@/components/crud/ListingBox.vue';
import type { Filter, Page } from '@/model/Page';
import { useI18n } from 'vue-i18n';
import { ref, onMounted, watch } from 'vue';
import { container } from '@/inversify.config';
import type { IIncidentTypeService } from '@/service/IService/IIncidentTypeService';
import TYPES from '@/inversify/types';
import type { IncidentTypeDto } from '@/model/dto/IncidentTypeDto';


const incidentTypeService = container.get<IIncidentTypeService>(TYPES.incidentTypeService);

const fetchIncidentTypes = async (filtering?: Filter<IncidentTypeDto>): Promise<Page<IncidentTypeDto>> => {
    return await incidentTypeService.getAllIncidentTypes(filtering);
}

const { t, locale } = useI18n();

const filterDefinition = ref({});

function buildFilterDefinition() {
    filterDefinition.value = {
        severity: {
            type: 'select',
            label: t('incidentType.fields.severity.title'),
            options: [
                { value: 'Minor', text: t('incidentType.severity.Minor') },
                { value: 'Major', text: t('incidentType.severity.Major') },
                { value: 'Critical', text: t('incidentType.severity.Critical') }
            ]
        }
    };
}

onMounted(() => buildFilterDefinition());
watch(locale, () => buildFilterDefinition());

</script>

<template>
<div>
    <sl-breadcrumb>
        <sl-breadcrumb-item><RouterLink to="/incident-types/dashboard" class="breadcrumb-link">{{ t('incidentType.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item>{{ t('incidentType.tabs.search') }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
        <h1 class="title">{{ t('incidentType.title') }}</h1>
        <p class="subtitle">{{ t('incidentType.subtitle.search') }}</p>

        <ListingBox :fetch-function="fetchIncidentTypes" search-filter="name" :filter-definition="filterDefinition" v-slot="{elements}">
            <li v-for="incidentType in elements" :key="incidentType.bid">
                <IncidentTypePrinter class="listing-box" :incident-type="incidentType" :link="`/incident-types/view/${incidentType.bid}`"/>
            </li>
        </ListingBox>
    </header>
</div>
</template>
