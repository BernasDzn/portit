<script setup lang="ts">
const props = defineProps<{
    noResultsMessage?: string;
}>();
</script>

<template>
    <div class="no-results-container">
        <img
            src="/noresults.svg"
            alt="No results."
            class="no-results-image"
        />
        <div class="no-results-text">
            {{ props.noResultsMessage }}
        </div>
    </div>
</template>