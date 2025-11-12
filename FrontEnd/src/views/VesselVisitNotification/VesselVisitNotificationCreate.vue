<script setup lang="ts">
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import { useI18n } from 'vue-i18n';
import type { Qualification } from '@/model/Qualifications';
import { ref } from 'vue';
import { container } from '@/inversify.config';
import { type VesselVisitNotification, type NotificationDecision, type VesselVisitNotificationFilter, VesselVisitNotificationStatus } from "@/model/VesselVisitNotification";
import TYPES from '@/inversify/types';
import type { IVesselVisitNotificationService } from '@/service/IService/IVesselVisitNotificationService';

const vvnService = container.get<IVesselVisitNotificationService>(TYPES.vesselVisitNotificationService);

const vvn = ref<VesselVisitNotification>({
    notificationId: '',
    status: VesselVisitNotificationStatus.InProgress,
    expectedArrival: '',
    expectedDeparture: '',
    isCargoHazardous: false,
    specialRequirements: '',
    crewDetails: {
        captain: {
            value: '',
        },
        totalCrewMembers: 0,
        safetyOfficers: '',
    },
    loadCargoManifest: [],
    unloadCargoManifest: [],
    vessel: null,
    submitter: null,
    notificationDecisions: [],
});

const { t } = useI18n();

const submitVVN = (obj: VesselVisitNotification) => 
    vvnService.createVesselVisitNotification(obj);

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item><RouterLink to="/qualifications/dashboard" class="breadcrumb-link">{{ t('qualification.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('qualification.tabs.create') }}</sl-breadcrumb-item>
        </sl-breadcrumb>
        
        <h1 class="title">{{ t('qualification.tabs.create') }}</h1>
        <p class="subtitle">{{ t('qualification.subtitle.create') }}</p>

        <EntityForm
            :object="vvn" 
            :submit-function="submitVVN"
        >
            <!-- <FormField :required="true" class="field" :name="t('qualification.fields.idCode.title') + '*'" v-model="qualification.idCode" :placeholderText="t('qualification.fields.idCode.placeholder')" pattern="^[a-zA-Z0-9]+$" />
            <FormField :required="true" class="field" :name="t('qualification.fields.qualificationName.title') + '*'" v-model="qualification.qualificationName" :placeholderText="t('qualification.fields.qualificationName.placeholder')"/> -->

            <FormField :required="true" class="field" name="Expected Arrival*" v-model="vvn.expectedArrival" placeholderText="Enter Expected Arrival Date and Time" type="datetime-local" />
            <FormField :required="true" class="field" name="Expected Departure*" v-model="vvn.expectedDeparture" placeholderText="Enter Expected Departure Date and Time" type="datetime-local" />

        </EntityForm>

    </div>
</template>