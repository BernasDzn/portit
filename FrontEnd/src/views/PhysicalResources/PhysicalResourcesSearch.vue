<script setup lang="ts">
import ListingBox from '@/components/crud/ListingBox.vue';
import PhysicalResourcePrinter from '@/components/printers/PhysicalResourcePrinter.vue';
import type { Filter, Page } from '@/model/Page';
import type { PhysicalResource, PhysicalResourceFilter } from '@/model/PhysicalResource';
import AxiosHttpService from '@/service/AxiosHttpService';
import { PhysicalResourceService } from '@/service/PhysicalResourceService';

const http = new AxiosHttpService()
const resourceService = new PhysicalResourceService(http as any)

const fetchResources = async (filtering?: Filter<PhysicalResourceFilter>): Promise<Page<PhysicalResource>> => {
    return await resourceService.getPhysicalResources(filtering);
}

// const filterDefinition = {
//     idCode: {
//         type: 'text',
//         label: 'Qualification Code',
//     }
// };

</script>

<template>
<div>

    <sl-breadcrumb>
        <sl-breadcrumb-item><RouterLink to="/resources/dashboard" class="breadcrumb-link">Physical Resources Dashboard</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item>Search Physical Resources</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
        <h1 class="title">Physical Resources</h1>
        <p class="subtitle">Manage existing Physical Resources at the port</p>

        <ListingBox :fetch-function="fetchResources" search-filter="description" v-slot="{elements}">
            <li v-for="resource in elements" :key="resource.code">
                <!-- {{ resource.code }} - {{ resource.description }} -->
                <!-- <QualificationPrinter class="listing-box" :qualification="qualification" :link="`/qualifications/view/${qualification.idCode}`" /> -->
                <PhysicalResourcePrinter class="listing-box" :resource="resource" :link="`/resources/view/${resource.code}`" />
            </li>
        </ListingBox>
    </header>
</div>
</template>