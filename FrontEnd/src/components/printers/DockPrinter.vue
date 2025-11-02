<script setup lang="ts">
import type { Dock } from '@/model/Dock';
import { computed } from 'vue';
const props = defineProps<{dock: Dock}>();

const typesDisplay = computed(() => {
    const t = (props as any).dock?.supportedVesselTypes;
    return Array.isArray(t) ? t.map((type: any) => type.name).join(', ') : '';
});
</script>

<template>
    <sl-card class="listing-item">
        <div class="opposed">
            <div>
                <p>{{ dock.name }}</p>
                <p class="item-description">
                    {{ dock.code }}<br/>
                    {{ dock.location }}<br/>
                    Supports: {{ typesDisplay }}
                </p>
            </div>
            <span class="material-icons icon" aria-hidden="true">anchor</span>
        </div>
        <slot></slot>
    </sl-card>
</template>

<style scoped>

.icon {
    font-size: 35px;
    color: var(--accent-1);
}

</style>