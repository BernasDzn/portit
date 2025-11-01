<script setup lang="ts">
import { onMounted, ref, watch } from 'vue';
import Loading from './Loading.vue';
import ErrorHandler from './ErrorHandler.vue';
import type { Page } from '@/model/Page';

// The function to fetch data is passed as a prop
const props = defineProps<{
    fetchFunction: (filtering?: any) => Promise<Page<any>>,
    searchFilter: string,
    listingStyle?: string,
}>();

const loading = ref(false);
const error = ref<Error | null>(null);
const searchTerm = ref('');

const elements = ref<any[]>([]);

onMounted(async () => {
    await loadElements();
});

const loadElements = async (filter?: any) => {
    
    loading.value = true;
    error.value = null;

    try {

        elements.value = (await props.fetchFunction(filter)).items;

    } catch (e: any) {
        console.error('Failed to load elements', e);
        error.value = e;
    } finally {
        loading.value = false;
    }
};

watch(searchTerm, async (newTerm) => {
    const filter = props.searchFilter ? { [props.searchFilter]: newTerm } : undefined;
    //console.log('Search term changed:', newTerm, 'Applying filter:', filter);
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
            <ul v-else :class="props.listingStyle || 'listing-doubles'">
                <slot :elements="elements">
                    No elements found.
                </slot>
            </ul>
        </sl-card>
    </div>
</template>

<style scoped>

.listing-search {
    margin-bottom: 1rem;
    color: gray;
}

</style>