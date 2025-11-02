<script setup lang="ts">
import DockPrinter from '@/components/printers/DockPrinter.vue';
import ListingBox from '@/components/ListingBox.vue';
import type { Filter, Page } from '@/model/Page';
import type { Dock, DockFilter } from '@/model/Dock';
import AxiosHttpService from '@/service/AxiosHttpService';
import { DockService } from '@/service/DockService';

const http = new AxiosHttpService()
const dockService = new DockService(http as any)

const fetchDocks = async (filtering?: Filter<DockFilter>): Promise<Page<Dock>> => {
    return await dockService.getDocks(filtering);
}

</script>

<template>
<div>

    <sl-breadcrumb>
        <sl-breadcrumb-item><RouterLink to="../docks/dashboard" class="breadcrumb-link">Dock Dashboard</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item>Search Docks</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
        <h1 class="title">Docks</h1>
        <p class="subtitle">Search all docks</p>

        <ListingBox :fetch-function="fetchDocks" search-filter="dockName" v-slot="{elements}">
            <li v-for="dock in elements" :key="dock.code">
                <DockPrinter class="listing-box" :dock="dock" />
            </li>
        </ListingBox>
    </header>
</div>
</template>