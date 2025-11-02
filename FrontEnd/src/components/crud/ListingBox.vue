<script setup lang="ts">
import { onMounted, ref, watch, onBeforeUnmount } from 'vue';
import Loading from '../Loading.vue';
import ErrorHandler from '../ErrorHandler.vue';
import type { Filter, Page } from '@/model/Page';
import Pagination from '../Pagination.vue';
import NoResults from '../NoResults.vue';

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
    
const showFiltermenu = ref(false);
const filters = ref<{ [key: string]: any }>({});

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

let debounceTimer: ReturnType<typeof setTimeout> | null = null;
const DEBOUNCE_MS = 500;

watch(searchTerm, (newTerm) => {
    const normalized = (newTerm || '').toString();
    if (normalized.trim().length === 0) {
        if (debounceTimer) {
            clearTimeout(debounceTimer);
            debounceTimer = null;
        }
        pageNumber.value = 1;
        void loadElements({ filter: {}, pageNumber: pageNumber.value });
        return;
    }

    if (debounceTimer) clearTimeout(debounceTimer);
    debounceTimer = setTimeout(async () => {
        pageNumber.value = 1;
        const filter: Filter<any> = {
            filter: {
                [props.searchFilter]: newTerm
            },
            pageNumber: pageNumber.value
        };
        await loadElements(filter);
    }, DEBOUNCE_MS);
});

watch(pageNumber, async (newPageNumber) => {
    if (debounceTimer) {
        clearTimeout(debounceTimer);
        debounceTimer = null;
    }

    const filter: Filter<any> = {
        filter: {
            [props.searchFilter]: searchTerm.value
        },
        pageNumber: newPageNumber
    };

    await loadElements(filter);
});

onBeforeUnmount(() => {
    if (debounceTimer) {
        clearTimeout(debounceTimer);
        debounceTimer = null;
    }
});

</script>

<template>
    <div>
        <ErrorHandler v-if="error" :error-object="error" />
        <sl-card v-else class="listing-box">
            <div class="listing-filters">
                <sl-input class="listing-search" placeholder="Search..." size="large" clearable v-model="searchTerm">
                    <span slot="prefix" class="material-icons material-icons--prefix">search</span>
                </sl-input>
                <sl-button class="filter-button" variant="default" size="large" @click="() => showFiltermenu = !showFiltermenu">
                    <sl-icon slot="prefix" name="filter"></sl-icon>
                    Filter
                </sl-button>
            </div>
            <div v-if="showFiltermenu">
                <!-- Future filter menu implementation -->
                <p>Filter menu coming soon...</p>
            </div>
            <!-- Pass the loaded elements to the parent via a slot prop --> 
            <Loading v-if="loading"/>
            <div v-else>
                <template v-if="elements.items && elements.items.length">
                    <ul :class="props.listingStyle || 'listing-doubles'">
                        <slot :elements="elements.items" />
                    </ul>
                    <Pagination :total-pages="elements.pageCount" :current-page="elements.pageNumber" @page-changed="(n: number) => pageNumber = n"  />
                </template>
                <template v-else>
                    <div class="no-results-container">
                        <NoResults noResultsMessage="No results found."/>
                    </div>
                </template>
            </div>

        </sl-card>
    </div>
</template>

<style scoped>
.listing-filters {
    display: flex;
    align-items: center;
    gap: 0.5rem;
}
  
.listing-search {
    flex: 1;
}

.filter-button::part(base) {
    margin-bottom: 1rem;
    flex-shrink: 0;
}
  
</style>