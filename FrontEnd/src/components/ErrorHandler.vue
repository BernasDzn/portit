<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';

const props = defineProps<{
    errorObject: Error
}>();

const errorMessage = computed(() => {
    return props.errorObject.message || 'An unknown error occurred.';
});


</script>

<template>
    <sl-card class="error">
        <h2> Something went wrong!</h2>
        <p>{{ errorMessage }}</p>
        
        <pre v-if="props.errorObject.stack">{{ props.errorObject.stack }}</pre>
        <pre v-else>No additional stack trace available.</pre>
    </sl-card>
</template>