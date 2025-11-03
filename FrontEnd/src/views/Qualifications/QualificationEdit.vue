<script setup lang="ts">
import { useRoute } from 'vue-router';
import QualificationCreate from './QualificationCreate.vue';
import { useAlerts } from '@/composables/alerts';
import AxiosHttpService from '@/service/AxiosHttpService';
import { QualificationService } from '@/service/QualificationService';
import { ref } from 'vue';
import type { Qualification } from '@/model/Qualifications';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';

const route = useRoute();
const qualificationId = String(route.params.id || '');

const notifications = useAlerts();

const http = new AxiosHttpService();
const qualificationService = new QualificationService(http as any);

const qualification = ref<Qualification>({
    idCode: qualificationId,
    qualificationName: ''
});

const updateQualification = async (obj: Qualification) => {
    if (!qualificationId) {
        notifications.enqueueNotification(
            'Cannot update qualifications at this time.',
            notifications.notificationTypes.DANGER
        );
        return;
    }

    qualificationService.updateQualification(qualificationId, obj);
};

const getById = async (id: string) => 
    qualificationService.getQualificationById(id);

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item><RouterLink to="/qualifications/dashboard" class="breadcrumb-link">Qualifications Dashboard</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item><RouterLink to="/qualifications/search" class="breadcrumb-link">Search Qualifications</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>Edit Qualification</sl-breadcrumb-item>
        </sl-breadcrumb>
        
        <h1 class="title">Edit Qualification</h1>
        <p class="subtitle">Update a qualification from the system</p>

        <EntityForm
            :editing-id="qualificationId"
            :object="qualification" 
            :submit-function="updateQualification"
            :fetching-function="getById"
        >
            <FormField :enabled="false" :required="true" class="field" name="Qualification Code*" v-model="qualification.idCode" placeholderText="Qualification code" />
            <FormField :required="true" class="field" name="Qualification Name*" v-model="qualification.qualificationName" placeholderText="Qualification name"/>
        </EntityForm>

    </div>
</template>