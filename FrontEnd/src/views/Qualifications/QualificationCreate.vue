<script setup lang="ts">
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import { useI18n } from 'vue-i18n';
import type { Qualification } from '@/model/Qualifications';
import { ref } from 'vue';
import { container } from '@/inversify.config';
import type { IQualificationService } from '@/service/IService/IQualificationService';
import TYPES from '@/inversify/types';

const qualificationService = container.get<IQualificationService>(TYPES.qualificationService);

const qualification = ref<QualificationDto>({
    idCode: '',
    qualificationName: ''
});

const { t } = useI18n();

const submitQualification = (obj: QualificationDto) => 
    qualificationService.addQualification(obj);

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
            :object="qualification" 
            :submit-function="submitQualification"
        >
            <FormField :required="true" class="field" :name="t('qualification.fields.idCode.title') + '*'" v-model="qualification.idCode" :placeholderText="t('qualification.fields.idCode.placeholder')" pattern="^[a-zA-Z0-9]+$" />
            <FormField :required="true" class="field" :name="t('qualification.fields.qualificationName.title') + '*'" v-model="qualification.qualificationName" :placeholderText="t('qualification.fields.qualificationName.placeholder')"/>
        </EntityForm>

    </div>
</template>