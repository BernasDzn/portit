<script setup lang="ts">
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import type { PhysicalResource, STSCrane } from '@/model/PhysicalResource';
import AxiosHttpService from '@/service/AxiosHttpService';
import { PhysicalResourceService } from '@/service/PhysicalResourceService';
import { ref } from 'vue';

const http = new AxiosHttpService();
const resourceService = new PhysicalResourceService(http);

const craneResourse = ref<STSCrane>({
    code: '',
    description: '',
    status: 0,
    setupTime: 0,
    operationalWindow: null,
    qualifications: [],
    liftingCapacity: 0,
    servingDock: null,
    containersPerHour: 0
});

const submitResource = (obj: PhysicalResource) => 
    resourceService.addPhysicalResource(obj);

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item><RouterLink to="/resources/dashboard" class="breadcrumb-link">Physical Resources Dashboard</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>Create Physical Resource</sl-breadcrumb-item>
        </sl-breadcrumb>
        
        <h1 class="title">Create Create Physical Resource</h1>
        <p class="subtitle">Register a new physical resource into the system</p>

        <EntityForm
            :object="craneResourse" 
            :submit-function="submitResource"
        >
        </EntityForm>

    </div>
</template>