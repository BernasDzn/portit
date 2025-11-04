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
import { useI18n } from 'vue-i18n';
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

const { t } = useI18n();

const submitStaff = (obj: any) => 
    staffService.createStaff(obj);

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item><RouterLink to="/staff/dashboard" class="breadcrumb-link">{{ t('staff.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('staff.tabs.create') }}</sl-breadcrumb-item>
        </sl-breadcrumb>

        <h1 class="title">{{ t('staff.tabs.create') }}</h1>
        <p class="subtitle">{{ t('staff.subtitle.create') }}</p>
        <EntityForm :object="staff" :submit-function="submitStaff">
            <div class="name-imo">
                <FormField :required="true" class="field" :name="t('staff.fields.name.title') + '*'" v-model="staff.name" :placeholderText="t('staff.fields.name.placeholder')"/>
                <FormField :required="true" class="field" :name="t('staff.fields.email.title') + '*'" v-model="staff.email" :placeholderText="t('staff.fields.email.placeholder')"/>
                <FormField :required="true" class="field" :name="t('staff.fields.phoneNumber.title') + '*'" v-model="staff.phoneNumber" :placeholderText="t('staff.fields.phoneNumber.placeholder')"/>
                <EntityDropdown
                    class="field-dropdown"
                    :name="t('staff.fields.qualifications.title') + '*'"
                    v-model="staff.qualificationsCodes"
                    :fetch-function="() => qualificationService.getQualifications().then(page => (page.items || []).map(t => t.idCode))"
                    :fetch-on-mount="true"
                    :placeholderText="t('staff.fields.qualifications.placeholder')"
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