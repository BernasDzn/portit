<script setup lang="ts">
import ListingBox from '@/components/crud/ListingBox.vue';
import PhysicalResourcePrinter from '@/components/printers/PhysicalResourcePrinter.vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { Filter, Page } from '@/model/Page';
import type { PhysicalResource, PhysicalResourceFilter } from '@/model/PhysicalResource';
import type { IPhysicalResourceService } from '@/service/IService/IPhysicalResourceService';
import { useI18n } from 'vue-i18n';

const resourceService = container.get<IPhysicalResourceService>(TYPES.physicalResourceService);

const fetchResources = async (filtering?: Filter<PhysicalResourceFilter>): Promise<Page<PhysicalResource>> => {
    return await resourceService.getPhysicalResources(filtering);
}
const { t } = useI18n();

const filterDefinition = {
    Code: {
        type: 'text',
        label: t('physicalResource.fields.code.title'),
    },
    Status: {
        type: 'select',
        label: t('physicalResource.fields.status.title'),
        options: [
            { value: '0', text: t('physicalResource.fields.status.options.available') },
            { value: '1', text: t('physicalResource.fields.status.options.maintenance') },
            { value: '2', text: t('physicalResource.fields.status.options.outOfService') },
        ],
    },
    Type: {
        type: 'select',
        label: t('physicalResource.fields.type.title'),
        options: [
            { value: '0', text: t('physicalResource.fields.type.options.stsCrane') },
            { value: '1', text: t('physicalResource.fields.type.options.yardGantry') },
            { value: '2', text: t('physicalResource.fields.type.options.truck') },
        ],
    }
};

</script>

<template>
<div>

    <sl-breadcrumb>
        <sl-breadcrumb-item><RouterLink to="/resources/dashboard" class="breadcrumb-link">{{ t('physicalResource.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item>{{ t('physicalResource.tabs.search') }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
        <h1 class="title">{{ t('physicalResource.title') }}</h1>
        <p class="subtitle">{{ t('physicalResource.subtitle.search') }}</p>

        <ListingBox :fetch-function="fetchResources" search-filter="Description" v-slot="{elements}" :filter-definition="filterDefinition">
            <li v-for="resource in elements" :key="resource.code">
                <!-- {{ resource.code }} - {{ resource.description }} -->
                <!-- <QualificationPrinter class="listing-box" :qualification="qualification" :link="`/qualifications/view/${qualification.idCode}`" /> -->
                <PhysicalResourcePrinter class="listing-box" :resource="resource" :link="`/resources/view/${resource.code}`" />
            </li>
        </ListingBox>
    </header>
</div>
</template>