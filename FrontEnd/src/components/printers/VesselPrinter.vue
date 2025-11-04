<script setup lang="ts">
import type { Vessel } from '@/model/Vessel';
import { RouterLink } from 'vue-router';
import { computed } from 'vue';
import {useI18n} from 'vue-i18n';

const { t } = useI18n();

const props = defineProps<{ vessel: Vessel; link?: string }>();

</script>

<template>
    <component :is="props.link ? RouterLink : 'div'" :to="props.link">
        <sl-card class="listing-item">
            <div class="opposed">
                <div>
                    <p class="vessel-identification">
                        {{ props.vessel.name }} 
                        <sl-tag size="small" variant="neutral">
                            {{ props.vessel.type.name }}
                        </sl-tag>
                    </p> 
                    <p class="item-description">
                        {{ props.vessel.imoNumber }}<br/>
                        {{ t('vessel.ownedBy') }} {{ props.vessel.owner.name }}
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

.vessel-identification {
    display: flex;
    align-items: center;
    gap: 15px;
}

</style>