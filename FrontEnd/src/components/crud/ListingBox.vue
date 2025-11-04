<script setup lang="ts">
import { onMounted, ref, watch, onBeforeUnmount } from 'vue';
import Loading from '../Loading.vue';
import ErrorHandler from '../ErrorHandler.vue';
import type { Filter, Page } from '@/model/Page';
import Pagination from '../Pagination.vue';
import NoResults from '../NoResults.vue';
import { useI18n } from 'vue-i18n';

const {t} = useI18n();

// The function to fetch data is passed as a prop
const props = defineProps<{
    fetchFunction: (filtering?: Filter<any>) => Promise<Page<any>>,
    searchFilter: string,
    listingStyle?: string,
    filterDefinition?: { [key: string]: any }
}>();

const loading = ref(false);
const error = ref<Error | null>(null);
const searchTerm = ref('');

const pageNumber = ref(1);
const elements = ref<Page<any>>({ items: [], pageNumber: 0, pageSize: 0, pageCount: 0 });
    
const showFiltermenu = ref(false);
const filters = ref<{ [key: string]: any }>({});

onMounted(async () => {
  // Initialize filters before loading data
  if (props.filterDefinition) {
    for (const key in props.filterDefinition)
      filters.value[key] = '';
  }

  await loadElements({
    filter: {},
    pageNumber: pageNumber.value
  });
});

const loadElements = async (filter?: any) => {

    //console.log('Loading elements with filter:', filter);
    
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
        void loadElements({ filter: {
            ...filters.value,
            [props.searchFilter]: ''
        }, pageNumber: pageNumber.value });
        return;
    }

    if (debounceTimer) clearTimeout(debounceTimer);
    debounceTimer = setTimeout(async () => {
        pageNumber.value = 1;
        const filter: Filter<any> = {
            filter: {
                ...filters.value,
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
            ...filters.value,
            [props.searchFilter]: searchTerm.value
        },
        pageNumber: newPageNumber
    };

    await loadElements(filter);
});

watch(filters, async (newFilters) => {
    
    if (debounceTimer) {
        clearTimeout(debounceTimer);
        debounceTimer = null;
    }

    debounceTimer = setTimeout(async () => {
        pageNumber.value = 1;
        const filter: Filter<any> = {
            filter: {
                ...newFilters,
                [props.searchFilter]: searchTerm.value
            },
            pageNumber: pageNumber.value
        };

        await loadElements(filter);
        
    }, DEBOUNCE_MS);
}, { deep: true });

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
                <sl-input class="listing-search" :placeholder="t('buttons.search').concat('...')" size="large" clearable v-model="searchTerm">
                    <span slot="prefix" class="material-icons material-icons--prefix">search</span>
                </sl-input>
                <sl-button v-if="filterDefinition" class="filter-button" variant="default" size="large" @click="() => showFiltermenu = !showFiltermenu">
                    <sl-icon slot="prefix" name="filter"></sl-icon>
                    {{ t('buttons.filter.add') }}
                </sl-button>
            </div>
            <transition name="slide-fade">
                <div v-if="showFiltermenu" class="filter-box" ref="filterMenuRef">
                  <div class="filters-container">
                    <p class="filter-title">{{ t('buttons.filter.title') }}</p>
                    <template class="filter-container" v-for="(def, key) in props.filterDefinition" :key="key">
                        <div class="filter-field">
                            <label class="filter-label">{{ def.label }}</label>
                            <sl-input
                            class="filter-input"
                            v-if="def.type === 'text'"
                            size="medium"
                            clearable
                            v-model="filters[key]"
                            :placeholder="def.label"
                            />
                            <sl-select
                                class="filter-input"
                                v-else-if="def.type === 'select'"
                                size="medium"
                                clearable
                                :placeholder="t('buttons.select').concat(' ').concat(def.label)"
                                :value="filters[key]"
                                @sl-change="(e: any) => filters[key] = e.target.value"
                            >
                                <sl-option
                                    v-for="option in def.options"
                                    :key="option.value"
                                    :value="option.value"
                                >
                                    {{ option.text }}
                                </sl-option>
                            </sl-select>
                        </div>
                    </template>
              
                    <sl-button
                      class="clear-button"
                      variant="neutral"
                      outline
                      size="small"
                      @click="() => Object.keys(filters).forEach(k => filters[k] = '')"
                    >
                      {{ t('buttons.filter.clear') }}
                    </sl-button>
                  </div>
                </div>
            </transition>              
            <!-- Pass the loaded elements to the parent via a slot prop --> 
            <Loading v-if="loading"/>
            <div v-else>
                <main v-if="elements.items && elements.items.length">
                    <ul :class="props.listingStyle || 'listing-doubles'">
                        <slot :elements="elements.items" />
                    </ul>
                    <Pagination :total-pages="elements.pageCount" :current-page="elements.pageNumber" @page-changed="(n: number) => pageNumber = n"  />
                </main>
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

.filter-field {
    width: fit-content;
    margin-bottom: 1rem !important;
}

.filter-title {
    margin-top: 0;
}

.clear-button {
    margin-top: 1rem;
}

.filter-input::part(base) {
    margin: 0.5rem 0 0 0;
}

</style>