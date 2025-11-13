<script setup lang="ts">
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import OperationalWindowPicker from '@/components/OperationalWindowPicker.vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { StaffDto } from '@/model/dto/StaffDto';
import type { IQualificationService } from '@/service/IService/IQualificationService';
import type { IStaffService } from '@/service/IService/IStaffService';
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute, RouterLink } from 'vue-router';

const staffService = container.get<IStaffService>(TYPES.staffService);
const qualificationService = container.get<IQualificationService>(TYPES.qualificationService);

const route = useRoute();
const staffMecNumber = String(route.params.id || '');


const staff = ref<StaffDto>({
    mechanographicNumber: '',
    name: '',
    email: '',
    phoneNumber: '',
    status: 0,
    operationalWindow: { shifts: [] },
    qualificationsCodes: [],
});

onMounted(async () => {
    if (!staffMecNumber) return;
    try {
        const data = await staffService.getStaffByMechanographicNumber(staffMecNumber);
        if (!data) return;

        staff.value.mechanographicNumber = data.mechanographicNumber;
        staff.value.name = data.name;
        staff.value.email = data.email;
        staff.value.phoneNumber = data.phoneNumber;
        staff.value.status = data.status;
        staff.value.operationalWindow = data.operationalWindow;
        staff.value.qualificationsCodes = data.qualifications?.map(q => q.idCode) || [];
    } catch (err) {
        console.error('Failed to load staff', err);
    }
});

const { t } = useI18n();

const editStaff = (obj: any) => 
    staffService.updateStaff(obj);

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item><RouterLink to="/staff/dashboard" class="breadcrumb-link">{{ t('staff.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item><RouterLink to="/staff/search" class="breadcrumb-link">{{ t('staff.tabs.search') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink 
                    :to="staffMecNumber ? `/staff/view/${staffMecNumber}` : '/staff/search'" 
                    class="breadcrumb-link"
                >{{ staffMecNumber }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('staff.tabs.edit') }}</sl-breadcrumb-item>
        </sl-breadcrumb>

        <h1 class="title">{{ t('staff.tabs.edit') }}</h1>
        <p class="subtitle">{{ t('staff.subtitle.edit') }}</p>
        <EntityForm editingId="true" :object="staff" :submit-function="editStaff">
            <div class="name-imo">

                <FormField :required="true" class="field" :name="t('staff.fields.mechanographicNumber.title') + '*'" v-model="staff.mechanographicNumber" :placeholderText="t('staff.fields.mechanographicNumber.placeholder')" :enabled="false"/>
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
                    :required="false"
                    :multiple="true"
                    valueKey="name"
                    labelKey="name"
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