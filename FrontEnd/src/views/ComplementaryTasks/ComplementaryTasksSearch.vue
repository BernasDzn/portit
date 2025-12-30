<script setup lang="ts">
import ComplementaryTaskPrinter from '@/components/printers/ComplementaryTaskPrinter.vue';
import ListingBox from '@/components/crud/ListingBox.vue';
import type { Filter, Page } from '@/model/Page';
import { useI18n } from 'vue-i18n';
import { ref, onMounted, watch } from 'vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IVesselVisitExecutionService } from '@/service/IService/IVesselExecutionService';
import type { ComplementaryTaskDto } from '@/model/dto/VesselVisitExecutionDto';

const vveService = container.get<IVesselVisitExecutionService>(TYPES.vesselVisitExecutionService);

const fetchTasks = async (filtering?: Filter<any>): Promise<Page<ComplementaryTaskDto>> => {
    return await vveService.getAllComplementaryTasks(filtering);
}

const { t, locale } = useI18n();

const filterDefinition = ref({});

function buildFilterDefinition() {
    filterDefinition.value = {
        status: {
            type: 'select',
            label: 'Status',
            options: [
                { value: 'Pending', text: 'Pending' },
                { value: 'Started', text: 'Started' },
                { value: 'Delayed', text: 'Delayed' },
                { value: 'Completed', text: 'Completed' }
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
        <sl-breadcrumb-item>
            <RouterLink to="/task-categories/dashboard" class="breadcrumb-link">{{ t('taskCategory.tabs.dashboard') }}</RouterLink>
        </sl-breadcrumb-item>
        <sl-breadcrumb-item>Search</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
        <h1 class="title">Complementary Tasks</h1>
        <p class="subtitle">Browse all complementary tasks from vessel visit executions</p>

        <ListingBox :fetch-function="fetchTasks" search-filter="vveCode" :filter-definition="filterDefinition" v-slot="{elements}">
            <li v-for="task in elements" :key="task.taskId">
                <ComplementaryTaskPrinter class="listing-box" :task="task"/>
            </li>
        </ListingBox>
    </header>
</div>
</template>

<style scoped>
.loading-state, .no-data {
    text-align: center;
    padding: 3rem;
    color: var(--sl-color-neutral-600);
}

.error-state {
    margin: 2rem 0;
}

.listing {
    list-style: none;
    padding: 0;
    margin: 1rem 0;
    display: flex;
    flex-direction: column;
    gap: 1rem;
}
</style>
