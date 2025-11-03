<script setup lang="ts">
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import { useAlerts } from '@/composables/alerts';
import type { Qualification } from '@/model/Qualifications';
import AxiosHttpService from '@/service/AxiosHttpService';
import { QualificationService } from '@/service/QualificationService';
import { ref } from 'vue';

const http = new AxiosHttpService();
const qualificationService = new QualificationService(http as any);

const qualification = ref<Qualification>({
    idCode: '',
    qualificationName: ''
});

const submitQualification = (obj: Qualification) => 
    qualificationService.addQualification(obj);

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item><RouterLink to="/qualifications/dashboard" class="breadcrumb-link">Qualifications Dashboard</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>Create Qualification</sl-breadcrumb-item>
        </sl-breadcrumb>
        
        <h1 class="title">Create Qualification</h1>
        <p class="subtitle">Register a new qualification into the system</p>

        <EntityForm
            :object="qualification" 
            :submit-function="submitQualification"
        >
            <FormField :required="true" class="field" name="Qualification Code*" v-model="qualification.idCode" placeholderText="Qualification code" pattern="^[a-zA-Z0-9]+$" />
            <FormField :required="true" class="field" name="Qualification Name*" v-model="qualification.qualificationName" placeholderText="Qualification name"/>
        </EntityForm>

    </div>
</template>