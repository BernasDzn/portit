<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute } from 'vue-router';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';

import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import OperationalWindowPicker from '@/components/OperationalWindowPicker.vue';

import { useAlerts } from '@/composables/alerts';
import type { IPhysicalResourceService } from '@/service/IService/IPhysicalResourceService';
import type { IDockService } from '@/service/IService/IDockService';
import type { IQualificationService } from '@/service/IService/IQualificationService';
import { STSCrane, Truck, YardCrane } from '@/model/PhysicalResource';
import ObjectSelector from '@/components/crud/ObjectSelector.vue';

const { t } = useI18n();
const route = useRoute();
const notifications = useAlerts();

const resourceService = container.get<IPhysicalResourceService>(TYPES.physicalResourceService);
const dockService = container.get<IDockService>(TYPES.dockService);
const qualificationService = container.get<IQualificationService>(TYPES.qualificationService);

const resourceCode = String(route.params.code || '');
const type = ref(0);

const genericResource = ref({
  code: '',
  description: '',
  status: '',
  setupTime: 0,
  qualifications: [],
  liftingCapacity: 0,
  servingDock: null,
  containersPerHour: 0,
  maxLoadCapacity: 0,
  averageSpeed: 0,
  containersPerTrip: 0,
  operationalWindow: { shifts: [] },
});

const statusValues = {
  Available: 0,
  Maintenance: 1,
  'Out of Service': 2,
};

const notifyError = (msg: string) => {
  notifications.enqueueNotification(msg, notifications.notificationTypes.DANGER);
};

const update = () => {
  if (!resourceCode) return notifyError('Cannot update resource at this time.');

  const obj = genericResource.value;
  const base = {
    code: obj.code,
    description: obj.description,
    status: statusValues[obj.status],
    setupTimeInMinutes: obj.setupTime,
    operationalWindow: obj.operationalWindow,
    qualifications: obj.qualifications,
  };

  switch (type.value) {
    case 0:
      return resourceService.updateSTSCrane(new STSCrane({
        ...base,
        liftingCapacity: obj.liftingCapacity,
        servingDock: obj.servingDock,
        containersPerHour: obj.containersPerHour,
      }));
    case 1:
      return resourceService.updateYardCrane(new YardCrane({
        ...base,
        liftingCapacity: obj.liftingCapacity,
        containersPerHour: obj.containersPerHour,
      }));
    case 2:
      return resourceService.updateTruck(new Truck({
        ...base,
        maxLoadCapacity: obj.maxLoadCapacity,
        averageSpeed: obj.averageSpeed,
        containersPerTrip: obj.containersPerTrip,
      }));
  }
};

const getById = async () => {
  const res: any = await resourceService.getPhysicalResourceById(resourceCode);

  if (res.servingDock) type.value = 0;
  else if (res.averageSpeed !== undefined) type.value = 2;
  else type.value = 1;

  const statusKey = Object.keys(statusValues);
  genericResource.value = {
    ...genericResource.value,
    ...res,
    status: statusKey[res.status],
  };

  console.log('Loaded resource:', genericResource.value);
};

onMounted(async () => {
  if (!resourceCode) return;
  try {
    await getById();
  } catch (err) {
    console.error('Failed to load physical resource', err);
    notifyError('Failed to load resource data.');
  }
});
</script>

<template>
  <div>
    <sl-breadcrumb>
      <sl-breadcrumb-item>
        <RouterLink to="/resources/dashboard" class="breadcrumb-link">{{ t('physicalResource.tabs.dashboard') }}</RouterLink>
      </sl-breadcrumb-item>
      <sl-breadcrumb-item>
        <RouterLink to="/resources/search" class="breadcrumb-link">{{ t('physicalResource.tabs.search') }}</RouterLink>
      </sl-breadcrumb-item>
      <sl-breadcrumb-item>
        <RouterLink
          :to="resourceCode ? `/resources/view/${resourceCode}` : '/resources/search'"
          class="breadcrumb-link"
        >{{ resourceCode }}</RouterLink>
      </sl-breadcrumb-item>
      <sl-breadcrumb-item>{{ t('physicalResource.tabs.edit') }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <h1 class="title">{{ t('physicalResource.tabs.edit') }}</h1>
    <p class="subtitle">{{ t('physicalResource.subtitle.edit') }}</p>

    <EntityForm
      :object="genericResource"
      :submit-function="update"
      class="group"
    >
      <!-- General Fields -->
      <p class="section-title">{{ t('physicalResource.generalFields') }}</p>
      <div class="group">
        <div class="form">
            <FormField
                :enabled="false"
                required
                class="field"
                :name="`${t('physicalResource.fields.code.title')}*`"
                v-model="genericResource.code"
                :placeholderText="t('physicalResource.fields.code.placeholder')"
                pattern="^[a-zA-Z0-9]+$"
                input-id="pr-code"
            />

            <span class="section-divider"></span>

            <FormField
                required
                class="field"
                :name="`${t('physicalResource.fields.description.title')}*`"
                v-model="genericResource.description"
                :placeholderText="t('physicalResource.fields.description.placeholder')"
                input-id="pr-description"
            />

            <span class="section-divider"></span>

            <EntityDropdown
                class="field-dropdown"
                :name="`${t('physicalResource.fields.status.title')}*`"
                v-model="genericResource.status"
                :items="['Available', 'Maintenance', 'Out of Service']"
                :placeholderText="t('physicalResource.fields.status.placeholder')"
                required
                input-id="pr-status"
            />

            <span class="section-divider"></span>

            <ObjectSelector
                class="field-dropdown"
                :name="`${t('physicalResource.fields.qualifications.title')}*`"
                v-model="genericResource.qualifications"
                :fetch-function="() => qualificationService.getQualifications()"
                fetch-on-mount
                labelKey="qualificationName"
                required
                multiple
            />

            <span class="section-divider"></span>

            <FormField
                required
                class="field"
                :name="`${t('physicalResource.fields.setupTime.title')}*`"
                v-model="genericResource.setupTime"
                :placeholderText="t('physicalResource.fields.setupTime.placeholder')"
                pattern="^[0-9]+$"
                input-id="pr-setupTime"
            />
        </div>

        <div style="flex: 100%; width: 100%;">
          <OperationalWindowPicker
            v-if="genericResource.operationalWindow && genericResource.code"
            v-model="genericResource.operationalWindow"
          />
        </div>
      </div>

      <!-- Specific Fields -->
      <p class="section-title">{{ t('physicalResource.specificFields') }}</p>

      <!-- STS Crane -->
      <div v-if="type === 0">
        <ObjectSelector
            class="field-dropdown"
            :name="`${t('physicalResource.fields.servingDocks.title')}*`"
            v-model="genericResource.servingDock"
            :fetch-function="() => dockService.getDocks()"
            :fetch-on-mount="true"
            :placeholderText="t('physicalResource.fields.servingDocks.placeholder')"
            labelKey="name"
            required
        />
        <br />
        <FormField
          required
          class="field"
          :name="`${t('physicalResource.fields.liftingCapacity.title')}*`"
          v-model="genericResource.liftingCapacity"
          pattern="^[0-9]+$"
          input-id="pr-liftingCapacity"
        />
        <FormField
          required
          class="field"
          :name="`${t('physicalResource.fields.containersPerHour.title')}*`"
          v-model="genericResource.containersPerHour"
          pattern="^[0-9]+$"
          input-id="pr-containersPerHour"
        />
      </div>

      <!-- Yard Crane -->
      <div v-else-if="type === 1">
        <FormField
          required
          class="field"
          :name="`${t('physicalResource.fields.liftingCapacity.title')}*`"
          v-model="genericResource.liftingCapacity"
          pattern="^[0-9]+$"
          input-id="pr-liftingCapacity"
        />
        <FormField
          required
          class="field"
          :name="`${t('physicalResource.fields.containersPerHour.title')}*`"
          v-model="genericResource.containersPerHour"
          pattern="^[0-9]+$"
          input-id="pr-containersPerHour"
        />
      </div>

      <!-- Truck -->
      <div v-else>
        <FormField
          required
          class="field"
          :name="`${t('physicalResource.fields.maxLoadCapacity.title')}*`"
          v-model="genericResource.maxLoadCapacity"
          pattern="^[0-9]+$"
          input-id="pr-maxLoadCapacity"
        />
        <FormField
          required
          class="field"
          :name="`${t('physicalResource.fields.averageSpeed.title')}*`"
          v-model="genericResource.averageSpeed"
          pattern="^[0-9]+$"
          input-id="pr-averageSpeed"
        />
        <FormField
          required
          class="field"
          :name="`${t('physicalResource.fields.containersPerTrip.title')}*`"
          v-model="genericResource.containersPerTrip"
          pattern="^[0-9]+$"
          input-id="pr-containersPerTrip"
        />
      </div>
    </EntityForm>
  </div>
</template>

<style scoped>
.section-divider {
  width: 1px;
  margin: 0 1rem;
  background-color: var(--sl-color-neutral-200);
}
.section-title {
  font-size: 0.8rem;
  margin-bottom: 1rem;
  color: var(--sl-color-neutral-400);
}
.form {
  display: flex;
  flex-direction: row;
}
.group {
  margin: 5px;
}
</style>
