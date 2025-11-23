<script setup lang="ts">
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import OperationalWindowPicker from '@/components/OperationalWindowPicker.vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';

import type { IPhysicalResourceService } from '@/service/IService/IPhysicalResourceService';
import type { IDockService } from '@/service/IService/IDockService';
import type { IQualificationService } from '@/service/IService/IQualificationService';

import GeneralFields from './GeneralFields.vue';
import { STSCrane, Truck, YardCrane } from '@/model/PhysicalResource';
import ObjectSelector from '@/components/crud/ObjectSelector.vue';

const { t } = useI18n();

// Services
const resourceService = container.get<IPhysicalResourceService>(TYPES.physicalResourceService);
const dockService = container.get<IDockService>(TYPES.dockService);
const qualificationService = container.get<IQualificationService>(TYPES.qualificationService);

// Shared Data
const statuses = [
    t('physicalResource.fields.status.options.available'),
    t('physicalResource.fields.status.options.maintenance'),
    t('physicalResource.fields.status.options.outOfService')
];

const genericResource = ref({
    code: '',
    description: '',
    status: 0,
    setupTime: 0,
    qualifications: [],
    liftingCapacity: 0,
    servingDock: null,
    containersPerHour: 0,
    maxLoadCapacity: 0,
    averageSpeed: 0,
    containersPerTrip: 0,
    operationalWindow: { shifts: [] }
});

// Submit Handlers
function submitResource(obj: any, type: 'STS' | 'YardCrane' | 'Truck') {
    const base = {
        code: obj.code,
        description: obj.description,
        status: statuses.indexOf(obj.status),
        setupTimeInMinutes: obj.setupTime,
        operationalWindow: obj.operationalWindow,
        qualificationsCodes: obj.qualifications
    };

    switch (type) {
    case 'STS':
        return resourceService.addSTSCrane(new STSCrane({
            ...base,
            liftingCapacity: obj.liftingCapacity,
            servingDock: obj.servingDock,
            containersPerHour: obj.containersPerHour
        }));
    case 'YardCrane':
        return resourceService.addYardCrane(new YardCrane({
            ...base,
            liftingCapacity: obj.liftingCapacity,
            containersPerHour: obj.containersPerHour
        }));
    case 'Truck':
        return resourceService.addTruck(new Truck({
            ...base,
            maxLoadCapacity: obj.maxLoadCapacity,
            averageSpeed: obj.averageSpeed,
            containersPerTrip: obj.containersPerTrip
        }));
    }
}
</script>

<template>
  <div>
    <sl-breadcrumb>
      <sl-breadcrumb-item>
        <RouterLink to="/resources/dashboard" class="breadcrumb-link">
          {{ t('physicalResource.tabs.dashboard') }}
        </RouterLink>
      </sl-breadcrumb-item>
      <sl-breadcrumb-item>{{ t('physicalResource.tabs.create') }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <h1 class="title">{{ t('physicalResource.tabs.create') }}</h1>
    <p class="subtitle">{{ t('physicalResource.subtitle.create') }}</p>
    
    <sl-tab-group>
        <sl-tab slot="nav" panel="sts">{{ t('physicalResource.fields.type.options.stsCrane') }}</sl-tab>
        <sl-tab slot="nav" panel="yard">{{ t('physicalResource.fields.type.options.yardGantry') }}</sl-tab>
        <sl-tab slot="nav" panel="truck">{{ t('physicalResource.fields.type.options.truck') }}</sl-tab>
    
        <GeneralFields :t="t" :genericResource="genericResource" :statuses="statuses" :qualificationService="qualificationService" />
        <OperationalWindowPicker v-model="genericResource.operationalWindow" />

      <!-- ========== STS Crane ========== -->
      <sl-tab-panel name="sts">

        <EntityForm :object="genericResource" :submit-function="(obj) => submitResource(obj, 'STS')" class="group">

          <p class="section-title">{{ t('physicalResource.specificFields') }}</p>
          <!-- <EntityDropdown
            class="field-dropdown"
            :name="`${t('physicalResource.fields.servingDocks.title')}*`"
            v-model="genericResource.servingDock"
            :fetch-function="() => dockService.getDocks()"
            :fetch-on-mount="true"
            :placeholderText="t('physicalResource.fields.servingDocks.placeholder')"
            valueKey="code"
            labelKey="name"
            required
            input-id="pr-servingDock-sts"
          /> -->
          
          <ObjectSelector
                class="field-dropdown"
                :name="`${t('physicalResource.fields.servingDocks.title')}*`"
                v-model="genericResource.servingDock"
                :fetch-function="() => dockService.getDocks()"
                :placeholderText="t('physicalResource.fields.servingDocks.placeholder')"
                labelKey="name"
                required
                input-id="pr-servingDock-sts"
            />

          <FormField :required="true" class="field" :name="t('physicalResource.fields.liftingCapacity.title')" v-model="genericResource.liftingCapacity" pattern="^[0-9]+$" input-id="pr-liftingCapacity-sts" />
          <FormField :required="true" class="field" :name="t('physicalResource.fields.containersPerHour.title')" v-model="genericResource.containersPerHour" pattern="^[0-9]+$" input-id="pr-containersPerHour-sts" />
        </EntityForm>
      </sl-tab-panel>

      <!-- ========== Yard Crane ========== -->
      <sl-tab-panel name="yard">
        <EntityForm :object="genericResource" :submit-function="(obj) => submitResource(obj, 'YardCrane')" class="group">
          <p class="section-title">{{ t('physicalResource.specificFields') }}</p>
          <FormField :required="true" class="field" :name="t('physicalResource.fields.liftingCapacity.title')" v-model="genericResource.liftingCapacity" pattern="^[0-9]+$" input-id="pr-liftingCapacity-yard" />
          <FormField :required="true" class="field" :name="t('physicalResource.fields.containersPerHour.title')" v-model="genericResource.containersPerHour" pattern="^[0-9]+$" input-id="pr-containersPerHour-yard" />
        </EntityForm>
      </sl-tab-panel>

      <!-- ========== Truck ========== -->
      <sl-tab-panel name="truck">
        <EntityForm :object="genericResource" :submit-function="(obj) => submitResource(obj, 'Truck')" class="group">
          <p class="section-title">{{ t('physicalResource.specificFields') }}</p>
          <FormField :required="true" class="field" :name="t('physicalResource.fields.maxLoadCapacity.title')" v-model="genericResource.maxLoadCapacity" pattern="^[0-9]+$" input-id="pr-maxLoadCapacity-truck" />
          <FormField :required="true" class="field" :name="t('physicalResource.fields.averageSpeed.title')" v-model="genericResource.averageSpeed" pattern="^[0-9]+$" input-id="pr-averageSpeed-truck" />
          <FormField :required="true" class="field" :name="t('physicalResource.fields.containersPerTrip.title')" v-model="genericResource.containersPerTrip" pattern="^[0-9]+$" input-id="pr-containersPerTrip-truck" />
        </EntityForm>
      </sl-tab-panel>
    </sl-tab-group>
  </div>
</template>

<style scoped>

.section-title {
    font-size: 0.8rem;
    margin-bottom: 1rem;
    color: var(--sl-color-neutral-400);
}

.form{
    display: flex;
    flex-direction: row;
}

</style>