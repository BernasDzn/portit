<script setup lang="ts">
import QualificationPrinter from '@/components/printers/QualificationPrinter.vue';
import ListingBox from '@/components/crud/ListingBox.vue';
import type { Filter, Page } from '@/model/Page';
import type { Qualification } from '@/model/Qualifications';
import AxiosHttpService from '@/service/AxiosHttpService';
import { QualificationService } from '@/service/QualificationService';

const http = new AxiosHttpService()
const qualificationService = new QualificationService(http as any)

const fetchQualifications = async (filtering?: Filter<Qualification>): Promise<Page<Qualification>> => {
    return await qualificationService.getQualifications(filtering);
}

const filterDefinition = {
    idCode: {
        type: 'text',
        label: 'Qualification Code',
    }
};

</script>

<template>
<div>

    <sl-breadcrumb>
        <sl-breadcrumb-item><RouterLink to="/qualifications/dashboard" class="breadcrumb-link">Qualification Dashboard</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item>Search Qualifications</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
        <h1 class="title">Qualifications</h1>
        <p class="subtitle">Manage required certifications and qualifications</p>

        <ListingBox :fetch-function="fetchQualifications" search-filter="qualificationName" v-slot="{elements}" :filter-definition="filterDefinition">
            <li v-for="qualification in elements" :key="qualification.idCode">
                <!-- {{ qualification.idCode }} - {{ qualification.qualificationName }} -->
                <QualificationPrinter class="listing-box" :qualification="qualification" :link="`/qualifications/view/${qualification.idCode}`" />
            </li>
        </ListingBox>
    </header>
</div>
</template>