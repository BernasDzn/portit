<script setup lang="ts">
import type { Vessel } from '@/model/Vessel';
import { RouterLink } from 'vue-router';
import { computed } from 'vue';

const props = defineProps<{ vessel: Vessel; link?: string }>();

const imoDisplay = computed(() => ((props as any).vessel?.imoNumber ?? (props as any).vessel?.imo ?? ''));
const typeDisplay = computed(() => {
    const t = (props as any).vessel?.type;
    return t && typeof t === 'object' ? t.name : (t ?? '');
});
const ownerDisplay = computed(() => {
    const o = (props as any).vessel?.owner;
    return o && typeof o === 'object' ? o.name : (o ?? '');
});

</script>

<template>
    <component :is="props.link ? RouterLink : 'div'" :to="props.link">
        <sl-card class="listing-item">
            <div class="opposed">
                <div>
                    <p>{{ props.vessel.name }}</p>
                    <p class="item-description">
                        {{ imoDisplay }}<br/>
                        {{ typeDisplay }}<br/>
                        {{ ownerDisplay }}
                    </p>
                </div>
                <span class="material-icons icon" aria-hidden="true">directions_boat</span>
            </div>
            <slot></slot>
        </sl-card>
    </component>
</template>

<style scoped>
.icon {
    font-size: 35px;
    color: var(--accent-1);
}

</style>