<script setup lang="ts">
import { computed } from 'vue';
import { RouterLink } from 'vue-router';

const props = defineProps<{
    icon?: string;
    title: string;
    description?: string;
    to?: string;
}>();

const displayIcon = computed(() => props.icon || 'dashboard');
</script>

<template>
    <RouterLink v-if="props.to" :to="props.to" class="dashboard-item">
        <sl-card class="card" clickable>
            <div class="card-content">
                <div class="icon-wrap">
                    <span class="material-icons icon">{{ displayIcon }}</span>
                </div>
                <div class="text-wrap">
                    <p class="title">{{ props.title }}</p>
                    <p class="description">{{ props.description }}</p>
                </div>
            </div>
        </sl-card>
    </RouterLink>
    <div v-else class="dashboard-item">
        <sl-card class="card">
            <div class="card-content">
                <div class="icon-wrap">
                    <span class="material-icons icon">{{ displayIcon }}</span>
                </div>
                <div class="text-wrap">
                    <p class="title">{{ props.title }}</p>
                    <p class="description">{{ props.description }}</p>
                </div>
            </div>
        </sl-card>
    </div>
</template>

<style scoped>
.dashboard-item {
    cursor: pointer;
}
.card {
    width: 100%;
}
.card-content {
    display: flex;
    gap: 1rem;
    align-items: center;
}
.icon {
    font-size: 28px;
    padding-right: 0.5rem;
    color: var(--accent-1);
}
.text-wrap .title {
    margin: 0;
    font-weight: 600;
    color: var(--text-black);
}
.text-wrap .description {
    margin: 0;
    color: var(--sl-color-gray-500);
    font-size: 0.9rem;
}
</style>