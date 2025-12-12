<script setup lang="ts">
interface Props {
    warnings: string[];
}

defineProps<Props>();
</script>

<template>
    <div v-if="warnings.length > 0" class="warnings-container">
        <sl-alert 
            v-for="(warning, index) in warnings"
            :key="index" 
            variant="warning" 
            open
        >
            <sl-icon slot="icon" name="exclamation-triangle"></sl-icon>
            <strong>Warning:</strong>
            {{ warning }}
        </sl-alert>
    </div>
</template>

<style scoped>
.warnings-container {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
    gap: 1rem;
    margin-top: 1rem;
}

@media (min-width: 1200px) {
    .warnings-container {
        grid-template-columns: repeat(3, 1fr);
    }
}
</style>