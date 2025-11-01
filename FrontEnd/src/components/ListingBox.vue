<script setup lang="ts">
import { onMounted, ref, watch } from 'vue';
import Loading from './Loading.vue';
import ErrorHandler from './ErrorHandler.vue';
import type { Filter, Page } from '@/model/Page';
import Pagination from './Pagination.vue';

// The function to fetch data is passed as a prop
const props = defineProps<{
    fetchFunction: (filtering?: Filter<any>) => Promise<Page<any>>,
    searchFilter: string,
    listingStyle?: string,
}>();

const loading = ref(false);
const error = ref<Error | null>(null);
const searchTerm = ref('');

const pageNumber = ref(1);
const elements = ref<Page<any>>({ items: [], pageNumber: 0, pageSize: 0, pageCount: 0 });

onMounted(async () => {
    await loadElements(
        {
            filter: {},
            pageNumber: pageNumber.value
        }
    );
});

const loadElements = async (filter?: any) => {
    
    loading.value = true;
    error.value = null;

    try {

        elements.value = (await props.fetchFunction(filter));
        if (elements.value.pageNumber > elements.value.pageCount && elements.value.pageCount > 0) {
            
            // If the current page number exceeds the total pages, reset to the last page
            pageNumber.value = elements.value.pageCount;
            elements.value = (await props.fetchFunction({
                ...filter,
                pageNumber: pageNumber.value
            }));
        }

    } catch (e: any) {
        console.error('Failed to load elements', e);
        error.value = e;
    } finally {
        loading.value = false;
    }
};

watch([searchTerm, pageNumber], async ([newTerm, newPageNumber]) => {

    const filter: Filter<any> = {
        filter : {
            [props.searchFilter]: newTerm
        },
        pageNumber: newPageNumber,
    };

    await loadElements(filter);
});

</script>

<template>
    <div>
        <ErrorHandler v-if="error" :error-object="error" />
        <sl-card v-else class="listing-box">
            <sl-input class="listing-search" placeholder="Search..." size="large" clearable v-model="searchTerm">
                <span slot="prefix" class="material-icons material-icons--prefix">search</span>
              </sl-input>
            <!-- Pass the loaded elements to the parent via a slot prop --> 
            <Loading v-if="loading"/>
            <div v-else>
                <ul :class="props.listingStyle || 'listing-doubles'">
                    <slot :elements="elements.items">
                        No elements found.
                    </slot>
                </ul>
                <Pagination :total-pages="elements.pageCount" :current-page="elements.pageNumber" @page-changed="(n: number) => pageNumber = n"  />
            </div>

        </sl-card>
    </div>
</template>

<style scoped>

.listing-search {
    margin-bottom: 1rem;
    color: gray;
}

</style>