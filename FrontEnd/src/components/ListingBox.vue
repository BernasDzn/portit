<script setup lang="ts">
import { onMounted, ref } from 'vue';
import Loading from './Loading.vue';
import ErrorHandler from './ErrorHandler.vue';

// The function to fetch data is passed as a prop
const props = defineProps<{
    fetchFunction: () => Promise<any[]>,
    listingStyle?: string
}>();

const loading = ref(false);
const error = ref<Error | null>(null);
const elements = ref<any[]>([]);

onMounted(async () => {

    loading.value = true;
    error.value = null;

    try {

        elements.value = await props.fetchFunction();
        console.log('Loaded elements:', elements.value);

    } catch (e: any) {
        console.error('Failed to load elements', e);
        error.value = e;
    } finally {
        loading.value = false;
    }
});

</script>

<template>
    <div>
        <Loading v-if="loading"/>
        <ErrorHandler v-else-if="error" :error-object="error" />
        <sl-card v-else class="listing-box">
            <sl-input class="listing-search" placeholder="Search..." size="large" clearable>
                <span class="material-icons material-icons--prefix">search</span>
              </sl-input>
            <!-- Pass the loaded elements to the parent via a slot prop -->
            <ul :class="props.listingStyle || 'listing-doubles'">
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