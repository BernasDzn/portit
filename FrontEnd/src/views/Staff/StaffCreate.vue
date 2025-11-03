<script setup lang="ts">
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import type { StaffCreate } from '@/model/Staff';
import type { OperationalWindow } from '@/model/OperationalWindow';
import AxiosHttpService from '@/service/AxiosHttpService';
import { QualificationService } from '@/service/QualificationService';
import { StaffService } from '@/service/StaffService';
import { ref } from 'vue';
import { FullWeek } from '@/model/OperationalWindow';

const http = new AxiosHttpService();
const staffService = new StaffService(http);
const qualificationService = new QualificationService(http);

const staff = ref<StaffCreate>({
    mechanographicNumber: '',
    name: '',
    email: '',
    phoneNumber: '',
    status: 0,
    operationalWindow: FullWeek(),
    qualificationsCodes: [],
});

const submitStaff = (obj: any) => 
    staffService.createStaff(obj);

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item><RouterLink to="/staff/dashboard" class="breadcrumb-link">Staff Dashboard</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>Create Staff</sl-breadcrumb-item>
        </sl-breadcrumb>

        <h1 class="title">Create Staff</h1>
        <p class="subtitle">Register a new staff member into the system</p>
        <EntityForm :object="staff" :submit-function="submitStaff">
            <div class="name-imo">
                <FormField :required="true" class="field" name="Staff Name*" v-model="staff.name" placeholderText="Staff name"/>
                <FormField :required="true" class="field" name="Email*" v-model="staff.email" placeholderText="Email"/>
                <FormField :required="true" class="field" name="Phone Number*" v-model="staff.phoneNumber" placeholderText="Phone number"/>
                <EntityDropdown
                    class="field-dropdown"
                    name="Qualifications*"
                    v-model="staff.qualificationsCodes"
                    :fetch-function="() => qualificationService.getQualifications().then(page => (page.items || []).map(t => t.idCode))"
                    :fetch-on-mount="true"
                    placeholderText="Select qualifications"
                    :required="true"
                    :multiple="true"
                    valueKey="name"
                    labelKey="name"
                />
            </div>
        </EntityForm>
    </div>
</template>

<style scoped>
.name-imo {
    display: flex;
    gap: .5rem;
}

.measurements {
    display: flex;
    gap: .5rem;
}

.create-vessel-form {
    width: 100%;
}

.field {
    margin-bottom: 1rem;
    max-width: 30rem;
    padding: .5rem
}

.field-dropdown {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    margin-bottom: 1rem;
    margin-top: 0.5rem;
    margin-left: 0.5rem;
    max-width: 30rem;
}

.buttons {
    display: flex;
    justify-content: flex-end;
    gap: 1rem;
}

.form-button {
    min-width: 100px;
}

.form-messages {
    margin: 0.5rem 0 1rem 0;
    bottom: 1rem;
}

.form-tip {
    font-size: 0.9rem;
    color: #666666;
    margin-bottom: 1rem;
    display: flex;
    justify-content: flex-end;
}

</style>