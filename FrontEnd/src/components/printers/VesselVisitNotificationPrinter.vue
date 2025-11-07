<script setup lang="ts">
import type { VesselVisitNotification } from '@/model/VesselVisitNotification';
import VesselPrinter from './VesselPrinter.vue';
import { RouterLink } from 'vue-router';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

const props = defineProps<{ notification: VesselVisitNotification; link?: string; short?: boolean }>();
</script>

<template>
    <component :is="props.link ? RouterLink : 'div'" :to="props.link">
        <sl-card :class="'listing-item ' + (props.short ? 'short-card' :'')">
            <div class="opposed">
                <div>
                    <p class="notification-title">
                        {{ props.notification.notificationId }}
                        <sl-tag size="small" variant="danger" v-if="props.notification.isCargoHazardous">
                            {{ t('notification.hazardous') }}
                        </sl-tag>
                        <sl-tag size="small" variant="success" v-else>
                            {{ t('notification.nonHazardous') }}
                        </sl-tag>
                    </p>
                    <div v-if="props.short">
                        <p class="subtitle short">{{props.notification.vessel.name}}</p>
                    </div>
                    <p v-if="!props.short" class="item-description">
                        {{ t('notification.arrival') }}: {{ props.notification.expectedArrival.split('T')[0] }}<br/>
                        {{ t('notification.departure') }}: {{ props.notification.expectedDeparture.split('T')[0] }}<br/>
                        {{ t('notification.specialRequirements') }}: {{ props.notification.specialRequirements || t('notification.none') }}<br/>
                        {{ t('notification.crew') }}: {{ props.notification.crewDetails ? props.notification.crewDetails.totalCrewMembers : t('notification.unknown') }}<br/>{{ t('notification.captain') }}: {{ props.notification.crewDetails && props.notification.crewDetails.captain ? props.notification.crewDetails.captain.value : t('notification.unknown') }}
                    </p>
                </div>
                <span v-if="!props.short" class="material-icons icon" aria-hidden="true">notifications</span>
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

.notification-title {
    display: flex;
    align-items: center;
    gap: 15px;
}

.vessel-section {
    margin-top: 10px;
}

.short {
    margin-top: -10px;
}

</style>