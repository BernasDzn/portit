<script setup lang="ts">
import { useRoute } from 'vue-router';
import type { Staff } from '@/model/Staff';
import EntityView from '@/components/crud/EntityView.vue';
import ActivityTag from '@/components/ActivityTag.vue';
import QualificationPrinter from '@/components/printers/QualificationPrinter.vue';
import { useI18n } from 'vue-i18n';
const { t } = useI18n();
import WorkShiftPrinter from '@/components/printers/WorkShiftPrinter.vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IStaffService } from '@/service/IService/IStaffService';
const route = useRoute();

const staffService = container.get<IStaffService>(TYPES.staffService);
const mechanographicNumber = decodeURIComponent((route.params.mechanographicNumber ?? '') as string);

const fetchStaff = async (): Promise<Staff | undefined> => {
    return await staffService.getStaffByMechanographicNumber(mechanographicNumber);
};

</script>

<template>
  <div>
    <sl-breadcrumb>
        <sl-breadcrumb-item><RouterLink to="/staff/dashboard" class="breadcrumb-link">{{ t('staff.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item><RouterLink to="/staff/search" class="breadcrumb-link">{{ t('staff.tabs.search') }}</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item>{{ mechanographicNumber }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <EntityView :fetch-function="fetchStaff" v-slot="entity">
        <div>
            <div class="opposed">
                <div class="view-header">
                    <span class="material-icons icon" aria-hidden="true">person</span>
                    <div>
                        <h2 class="title">{{ entity.element.name }}</h2>
                        <p class="subtitle">{{ entity.element.mechanographicNumber }}</p>
                    </div>
                </div>
                <RouterLink :to="`/staff/edit/${encodeURIComponent(entity.element.mechanographicNumber)}`">
                    <sl-button variant="default" size="large">
                        <sl-icon slot="prefix" name="pencil"></sl-icon>
                        {{ t('staff.tabs.edit') }}
                    </sl-button>
                </RouterLink>
    
            </div>
            <div class="viewing-content">
                <sl-card class="info-card" style="flex: 65%;">
                    <p>{{ t('staff.infoTitle') }}</p>
                    <div class="info-grid">
                        <div class="info-block">
                            <span class="label">{{ t('staff.fields.name.title') }}</span>
                            <p>{{ entity.element.name }}</p>
                        </div>
                        <div class="info-block">
                            <span class="label">{{ t('staff.fields.mechanographicNumber.title') }}</span>
                            <p>{{ entity.element.mechanographicNumber }}</p>
                        </div>
                        <div class="info-block">
                            <span class="label">{{ t('staff.fields.email.title') }}</span>
                            <p>{{ entity.element.email }}</p>
                        </div>
                        <div class="info-block">
                            <span class="label">{{ t('staff.fields.phoneNumber.title') }}</span>
                            <p>{{ entity.element.phoneNumber }}</p>
                        </div>
                    </div>
                </sl-card>
                <sl-card class="info-card">
                    <p>{{ t('common.statistics') }}</p>
                    <div class="view-statistics-overview">
                        <p class="view-statistic-data">{{ entity.element.qualifications.length }}</p>
                        <p>{{ t('staff.fields.qualifications.title') }}</p>
                    </div>
                    <p class="info-row">
                        <span class="label">{{ t('staff.fields.status.title') }}:</span>
                        <span><ActivityTag :status="entity.element.status" /></span>
                    </p>
                </sl-card>
                <sl-card class="info-card" style="flex: 100%;">
                    <p>{{ t('staff.fields.qualifications.title') }}</p>
                    <div class="info-grid">
                        <div v-for="qualification in entity.element.qualifications" :key="qualification.idCode">
                            <QualificationPrinter class="listing-box" :qualification="qualification" :link="`/qualifications/view/${qualification.idCode}`" />
                        </div>
                    </div>
                </sl-card>
                <sl-card class="info-card" style="flex: 100%;">
                    <p>{{ t("operationalWindow.title") }}:</p>
                    <WorkShiftPrinter
                        :op_window="entity.element.operationalWindow"
                    />
                </sl-card>
            </div>
        </div>
    </EntityView>
  </div>
</template>

<style scoped> 

.icon{
    margin: 0;
    margin-right: 1rem;
}

.info-block {
    flex: 1 1 45%;
    min-width: 200px;
}

.viewing-content{
    display: flex;
    flex-wrap: wrap;
}

</style>