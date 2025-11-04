<script setup lang="ts">
import { useRoute } from 'vue-router';
import { AxiosHttpService } from '@/service/AxiosHttpService';
import { StaffService } from '@/service/StaffService';

import type { Staff } from '@/model/Staff';

import EntityView from '@/components/crud/EntityView.vue';
import ActivityTag from '@/components/ActivityTag.vue';
import QualificationPrinter from '@/components/printers/QualificationPrinter.vue';
import WorkShiftPrinter from '@/components/printers/WorkShiftPrinter.vue';

const route = useRoute();

const http = new AxiosHttpService();
const staffService = new StaffService(http);
const mechanographicNumber = decodeURIComponent((route.params.mechanographicNumber ?? '') as string);

const fetchStaff = async (): Promise<Staff | undefined> => {
    return await staffService.getStaffByMechanographicNumber(mechanographicNumber);
};

</script>

<template>
  <div>
    <sl-breadcrumb>
        <sl-breadcrumb-item><RouterLink to="/staff/dashboard" class="breadcrumb-link">Staff Dashboard</RouterLink></sl-breadcrumb-item>
        <sl-breadcrumb-item><RouterLink to="/staff/search" class="breadcrumb-link">Search Staff</RouterLink></sl-breadcrumb-item>
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
                        Edit Staff
                    </sl-button>
                </RouterLink>
    
            </div>
            <div class="viewing-content">
                <sl-card class="info-card" style="flex: 65%;">
                    <p>Staff information</p>
                    <div class="info-grid">
                        <div class="info-block">
                            <span class="label">Name</span>
                            <p>{{ entity.element.name }}</p>
                        </div>
                        <div class="info-block">
                            <span class="label">Mechanographic Number</span>
                            <p>{{ entity.element.mechanographicNumber }}</p>
                        </div>
                        <div class="info-block">
                            <span class="label">Email address</span>
                            <p>{{ entity.element.email }}</p>
                        </div>
                        <div class="info-block">
                            <span class="label">Phone number</span>
                            <p>{{ entity.element.phoneNumber }}</p>
                        </div>
                    </div>
                </sl-card>
                <sl-card class="info-card">
                    <p>Statistics</p>
                    <div class="view-statistics-overview">
                        <p class="view-statistic-data">{{ entity.element.qualifications.length }}</p>
                        <p>Qualifications</p>
                    </div>
                    <p class="info-row">
                        <span class="label">Status:</span> 
                        <span><ActivityTag :status="entity.element.status" /></span>
                    </p>
                </sl-card>
                <sl-card class="info-card" style="flex: 100%;">
                    <p>Qualifications</p>
                    <div class="info-grid">
                        <div v-for="qualification in entity.element.qualifications" :key="qualification.idCode">
                            <QualificationPrinter class="listing-box" :qualification="qualification" :link="`/qualifications/view/${qualification.idCode}`" />
                        </div>
                    </div>
                </sl-card>
                <sl-card class="info-card" style="flex: 100%;">
                    <p>Operational Window:</p>
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