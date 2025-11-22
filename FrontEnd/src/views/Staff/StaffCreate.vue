<script setup lang="ts">
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import ObjectSelector from '@/components/crud/ObjectSelector.vue';
import OperationalWindowPicker from '@/components/OperationalWindowPicker.vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { StaffDto } from '@/model/dto/StaffDto';
import { Staff } from '@/model/Staff';
import type { IQualificationService } from '@/service/IService/IQualificationService';
import type { IStaffService } from '@/service/IService/IStaffService';
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';

const staffService = container.get<IStaffService>( TYPES.staffService );
const qualificationService = container.get<IQualificationService>( TYPES.qualificationService );

const staff = ref({
    mechanographicNumber: '',
    name: '',
    email: '',
    phoneNumber: '',
    status: 0,
    operationalWindow: { shifts: [] },
    qualifications: [],
});

const { t } = useI18n();

const submitStaff = (obj: any) => 
    staffService.createStaff(new Staff(obj));

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
                <FormField input-id="staff-name" :required="true" class="field" :name="t('staff.fields.name.title') + '*'" v-model="staff.name" :placeholderText="t('staff.fields.name.placeholder')"/>
                <FormField input-id="staff-email" :required="true" class="field" :name="t('staff.fields.email.title') + '*'" v-model="staff.email" :placeholderText="t('staff.fields.email.placeholder')"/>
                <FormField input-id="staff-phone" :required="true" class="field" :name="t('staff.fields.phoneNumber.title') + '*'" v-model="staff.phoneNumber" :placeholderText="t('staff.fields.phoneNumber.placeholder')"/>

                <ObjectSelector
                    class="field-dropdown"
                    :name="t('staff.fields.qualifications.title') + '*'"
                    v-model="staff.qualifications"
                    :fetch-function="() => qualificationService.getQualifications()"
                    :placeholderText="t('staff.fields.qualifications.placeholder')"
                    labelKey="qualificationName"
                    required
                    multiple
                />
                <div style="flex:100%; width: 100%;">
                   <OperationalWindowPicker
                        v-model="staff.operationalWindow"
                   />
                </div>  
            </div>
        </EntityForm>
    </div>
</template>

<style scoped>
.name-imo {
    display: flex;
    gap: .5rem;
    flex-wrap: wrap;
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