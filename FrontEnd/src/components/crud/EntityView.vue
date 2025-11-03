<script setup lang="ts">
import { onMounted, ref, watch, onBeforeUnmount } from 'vue';
import Loading from '../Loading.vue';
import ErrorHandler from '../ErrorHandler.vue';
import NoResults from '../NoResults.vue';
import { onBeforeRouteUpdate, useRoute } from 'vue-router';

// The function to fetch data is passed as a prop
const props = defineProps<{
    fetchFunction: () => Promise<any | null>,
}>();

const loading = ref(false);
const error = ref<Error | null>(null);

const element = ref<any>(null);

const loadElement = async () => {
    loading.value = true;
    error.value = null;

    try {

        element.value = await props.fetchFunction();

    } catch (e: any) {
        console.error('Failed to load elements', e);
        error.value = e;
    } finally {
        loading.value = false;
    }
};

onMounted(() => {
    loadElement();
});

// Reload element when route changes (from update page)
const route = useRoute();
watch(() => route.fullPath, () => loadElement())

</script>

<template>
    <div>
        <ErrorHandler v-if="error" :error-object="error" />
        <div v-else class="viewing-box">
            <Loading v-if="loading"/>
            <NoResults v-else-if="element==null" noResultsMessage="No vessel of IMO" />
            <div v-else>
                <slot :element="element" />
            </div>
        </div>
    </div>
</template>