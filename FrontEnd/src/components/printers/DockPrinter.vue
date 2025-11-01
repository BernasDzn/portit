<script setup lang="ts">
import type { Dock } from '@/model/Dock';
const props = defineProps<{dock: Dock}>();
</script>

<template>
    <sl-card class="listing-item">
        <div class="opposed">
            <div>
                <p>{{ dock.name }}</p>
                <p class="item-description">
                    {{ dock.code }}<br/>
                    {{ dock.location }}<br/>
                    Supports: {{ dock.supportedVesselTypes.map(type => type.name).join(', ') }}
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