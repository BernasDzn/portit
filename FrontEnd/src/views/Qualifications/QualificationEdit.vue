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
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import type { IQualificationService } from '@/service/IService/IQualificationService';
import TYPES from '@/inversify/types';

const route = useRoute();
const qualificationId = String(route.params.id || '');

const notifications = useAlerts();

const qualificationService = container.get<IQualificationService>(TYPES.qualificationService);

const qualification = ref<QualificationDto>({
    idCode: qualificationId,
    qualificationName: ''
});

const updateQualification = async (obj: QualificationDto) => {
    if (!qualificationId) {
        notifications.enqueueNotification(
            'Cannot update qualifications at this time.',
            notifications.notificationTypes.DANGER
        );
        return;
    }

    qualificationService.updateQualification(obj);
};

const getById = async (id: string) => 
    qualificationService.getQualificationById(id);

const { t } = useI18n();

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item><RouterLink to="/qualifications/dashboard" class="breadcrumb-link">{{ t('qualification.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item><RouterLink to="/qualifications/search" class="breadcrumb-link">{{ t('qualification.tabs.search') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('qualification.tabs.edit') }}</sl-breadcrumb-item>
        </sl-breadcrumb>
        
        <h1 class="title">{{ t('qualification.tabs.edit') }}</h1>
        <p class="subtitle">{{ t('qualification.subtitle.edit') }}</p>

        <EntityForm
            :editing-id="qualificationId"
            :object="qualification" 
            :submit-function="updateQualification"
            :fetching-function="getById"
        >
            <FormField :enabled="false" :required="true" class="field" :name="t('qualification.fields.idCode.title') + '*'" v-model="qualification.idCode" :placeholderText="t('qualification.fields.idCode.placeholder')" />
            <FormField :required="true" class="field" :name="t('qualification.fields.qualificationName.title') + '*'" v-model="qualification.qualificationName" :placeholderText="t('qualification.fields.qualificationName.placeholder')"/>
        </EntityForm>

    </div>
</template>