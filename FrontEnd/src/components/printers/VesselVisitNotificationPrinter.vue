<script setup lang="ts">
import type { VesselVisitNotification } from '@/model/VesselVisitNotification';
import VesselPrinter from './VesselPrinter.vue';
import { RouterLink } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { useSession } from '@/composables/session';

const { t } = useI18n();

const props = defineProps<{ notification: VesselVisitNotification; link?: string; short?: boolean, review?: boolean}>();
</script>

<template>
    <component :is="props.link ? RouterLink : 'div'" :to="props.link">
        <sl-card :class="'listing-item ' + (props.short ? 'short-card' :'')">
            <div class="notification-display">
                <div class="notification-header">
                     <div class="notification-title">
                        <p>{{ props.notification.notificationId }}</p>
                        <sl-tag size="small" variant="danger" v-if="props.notification.isCargoHazardous">
                            {{ t('notification.hazardous') }}
                        </sl-tag>
                        <sl-tag size="small" variant="success" v-else>
                            {{ t('notification.nonHazardous') }}
                        </sl-tag>
                    </div>
                    <div v-if="!props.short" class="date item-description">
                        {{ props.notification.expectedArrival.split('T')[0] }}
                        <span class="material-icons arrow">arrow_forward</span>
                        {{ props.notification.expectedDeparture.split('T')[0] }}
                    </div>
                </div>
                <div class="opposed">
                    <div>
                        <div v-if="props.short">
                            <p class="subtitle short">{{props.notification.vessel.name}}</p>
                        </div>
                        <p v-if="!props.short" class="item-description">
                            {{ t('notification.specialRequirements') }}: {{ props.notification.specialRequirements || t('notification.none') }}<br/>
                            <!-- {{ t('notification.crew') }}: {{ props.notification.crewDetails ? props.notification.crewDetails.totalCrewMembers : t('notification.unknown') }}<br/> -->
                            {{ t('notification.captain') }}: {{ props.notification.crewDetails && props.notification.crewDetails.captain ? props.notification.crewDetails.captain.value : t('notification.unknown') }}
                        </p>
                    </div>
                    <span v-if="!props.short && !props.review" class="material-icons icon" aria-hidden="true">notifications</span>
                    <span v-if="!props.short && props.review" class="material-icons icon" aria-hidden="true">rate_review</span>
                    <!--<button class="review-button" v-if="props.review" @click="props.reviewButtonFunction">
                        <p>{{ t('buttons.review') }}</p> <span class="material-icons icon">rate_review</span>
                    </button>-->
                </div>
                
            </div>
            <div class="details" v-if="props.notification.vessel && !props.short">
                <sl-divider></sl-divider>
                <div class="vessel-section">
                        <p>{{ props.notification.vessel.name }} <sl-badge class="list-badge" variant="neutral">{{ props.notification.vessel.imoNumber }}</sl-badge></p>
                        <p v-if="useSession().authenticatedUser?.role === 0">{{ props.notification.submitter.name }}<br/>
                        <span class="submitter-description">{{ props.notification.vessel.owner.name }}</span>
                        </p>
                </div>                
                
                <slot></slot>
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
.date {
    display: flex;
    align-items: center;
    font-size: small;
    width: fit-content;
    gap: 2px;
}

.arrow {
    font-size: 15px;
}

.notification-display {
    display: flex;
    flex-direction: column;
}

.notification-header {
    display: flex;
    flex-direction: column;
}

.notification-title {
    display: flex;
    flex-direction: row;
    justify-content: space-between;
    align-items: center;
}

.vessel-section {
    display: flex;
    justify-content: space-between;
}

.submitter-description {
    font-size: small;
    color: var(--sl-color-neutral-500);
}

.short {
    margin-top: -10px;
}

.list-badge::part(base) {
    border-radius: var(--sl-border-radius-medium);
    background-color: var(--sl-color-neutral-200);
    color: var(--sl-color-neutral-800);
}

/*
.review-button {
    display: flex;
    flex-direction: row;
    align-items: center;
    gap: 5px;
    background-color: transparent;
    border: 1px solid var(--accent-1);
    border-radius: var(--sl-border-radius-medium);
    cursor: pointer;
    padding: 5px;
    height: fit-content;
}

.review-button:hover {
    background-color: var(--sl-color-neutral-200);
}

.review-button .icon {
    margin: 0;
}
*/

</style>