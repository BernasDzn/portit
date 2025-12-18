<script setup lang="ts">
import { RouterLink } from 'vue-router';
import type { TaskCategory } from '@/model/TaskCategory';

const props = withDefaults(defineProps<{
    taskCategory: TaskCategory;
    link?: string;
    showDetails?: boolean;
}>(), {
    showDetails: true
});

</script>

<template>
    <component :is="props.link ? RouterLink : 'div'" :to="props.link">
        <sl-card class="listing-item">
            <div class="opposed">
                <div>
                    <p>{{ taskCategory.category }}</p>
                    <p>{{ taskCategory.name }}</p>
                    <p class="item-description">
                        {{ taskCategory.description }}
                    </p>
                </div>
                <span class="material-icons icon" aria-hidden="true">category</span>
            </div>
        </sl-card>
    </component>
</template>

<style scoped>
.listing-item {
    cursor: pointer;
    transition: transform 0.2s;
}

.listing-item:hover {
    transform: translateY(-2px);
}

.opposed {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    gap: 1rem;
}

.opposed > div {
    flex: 1;
}

.icon {
    font-size: 2.5rem;
    color: var(--sl-color-primary-600);
}

.item-description {
    color: var(--sl-color-neutral-600);
    font-size: 0.9rem;
    margin-top: 0.5rem;
}

.details {
    margin-top: 1rem;
    color: var(--sl-color-neutral-700);
    font-size: 0.9rem;
}
</style>
