<script setup lang="ts">
import IncidentTypePrinter from '@/components/printers/IncidentTypePrinter.vue';
import ListingBox from '@/components/crud/ListingBox.vue';
import type { Filter, Page } from '@/model/Page';
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import type { IIncidentTypeService } from '@/service/IService/IIncidentTypeService';
import TYPES from '@/inversify/types';
import type { IncidentTypeDto } from '@/model/dto/IncidentTypeDto';

const { t } = useI18n();

const incidentTypeService = container.get<IIncidentTypeService>(TYPES.incidentTypeService);

const fetchIncidentTypes = async (filtering?: Filter<IncidentTypeDto>): Promise<Page<IncidentTypeDto>> => {
    return await incidentTypeService.getAllIncidentTypes(filtering);
}

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

        <ListingBox :fetch-function="fetchIncidentTypes" v-slot="{elements}">
            <li v-for="incidentType in elements" :key="incidentType.bid">
                <IncidentTypePrinter class="listing-box" :incident-type="incidentType" :link="`/incident-types/view/${incidentType.bid}`"/>
            </li>
        </ListingBox>
    </header>
</div>
</template>
