<script setup lang="ts">
import { useRoute, useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import { useAlerts } from '@/composables/alerts';

import EntityView from '@/components/crud/EntityView.vue';
import ActivityTag from '@/components/ActivityTag.vue';
import QualificationPrinter from '@/components/printers/QualificationPrinter.vue';
import WorkShiftPrinter from '@/components/printers/WorkShiftPrinter.vue';

import TYPES from '@/inversify/types';
import type { IStaffService } from '@/service/IService/IStaffService';
import type { Staff } from '@/model/Staff';


const { t } = useI18n();
const route = useRoute();
const router = useRouter();
const notifications = useAlerts();

const staffService = container.get<IStaffService>(TYPES.staffService);
const mechanographicNumber = decodeURIComponent((route.params.mechanographicNumber ?? '') as string);

const fetchStaff = async (): Promise<Staff | undefined> => {
    return await staffService.getStaffByMechanographicNumber(mechanographicNumber);
};

const openDeactivationModal = () => {
    const dialog = document.querySelector('.dialog-overview') as any;
    dialog.show();
};

const closeModal = () => {
    const dialog = document.querySelector('.dialog-overview') as any;
    dialog.hide();
};

const deactivateStaff = async () => {
    await staffService.deactivateStaff(mechanographicNumber).then(() => {

        notifications.enqueueNotification(
            t('staff.deactivation.successMessage'),
            notifications.notificationTypes.SUCCESS
        );
        
        closeModal();
        router.back();

    }).catch((error) => {
        notifications.enqueueNotification(
            t('staff.deactivation.errorMessage') + ` (${error.message})`,
            notifications.notificationTypes.DANGER
        );
        console.error('Error deactivating resource:', error);
        closeModal();
        router.back();
    });
};

</script>

<template>
  <div>
    <sl-breadcrumb>
        <sl-breadcrumb-item><RouterLink to="/staff/dashboard" class="breadcrumb-link">{{ t('staff.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item><RouterLink to="/staff/search" class="breadcrumb-link">{{ t('staff.tabs.search') }}</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item>{{ mechanographicNumber }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <sl-dialog :label="t('staff.deactivation.title')" class="dialog-overview">
        {{ t('staff.deactivation.message') }}
        <sl-button slot="footer" @click="closeModal" variant="primary" sl-dialog-close>{{ t('buttons.cancel') }}</sl-button>
        <sl-button slot="footer" @click="deactivateStaff" variant="danger">{{ t('buttons.deactivate') }}</sl-button>
    </sl-dialog>

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
                <div>
                    <RouterLink :to="`/staff/edit/${encodeURIComponent(entity.element.mechanographicNumber)}`">
                    <sl-button slot="footer" variant="default" size="large">
                        <sl-icon slot="prefix" name="pencil"></sl-icon>
                        {{ t('staff.tabs.edit') }}
                    </sl-button>
                    </RouterLink>
                    <sl-button slot="footer" variant="danger" size="large" style="margin-left: 0.5rem;" @click="openDeactivationModal">
                        <sl-icon slot="prefix" name="x"></sl-icon>
                        {{ t('buttons.deactivate') }}
                    </sl-button>
                </div>
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