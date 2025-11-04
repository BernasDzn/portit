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

import { useI18n } from 'vue-i18n';

const { t } = useI18n();

const filterDefinition = {
    idCode: {
        type: 'text',
        label: t('qualification.fields.idCode.title'),
    }
};

</script>

<template>
<div>

    <sl-breadcrumb>
        <sl-breadcrumb-item><RouterLink to="/qualifications/dashboard" class="breadcrumb-link">{{ t('qualification.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item>{{ t('qualification.tabs.search') }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
        <h1 class="title">{{ t('qualification.title') }}</h1>
        <p class="subtitle">{{ t('qualification.subtitle.search') }}</p>

        <ListingBox :fetch-function="fetchQualifications" search-filter="qualificationName" v-slot="{elements}" :filter-definition="filterDefinition">
            <li v-for="qualification in elements" :key="qualification.idCode">
                <!-- {{ qualification.idCode }} - {{ qualification.qualificationName }} -->
                <QualificationPrinter class="listing-box" :qualification="qualification" :link="`/qualifications/view/${qualification.idCode}`" />
            </li>
        </ListingBox>
    </header>
</div>
</template>