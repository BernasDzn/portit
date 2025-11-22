<script setup lang="ts">
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import CargoManifestReader from '@/components/CargoManifestReader.vue';
import { ref, watch, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IVesselVisitNotificationService } from '@/service/IService/IVesselVisitNotificationService';
import type { IVesselService } from '@/service/IService/IVesselService';
import type { VesselVisitNotificationDto } from '@/model/dto/VesselVisitNotificationDto';
import { VesselVisitNotification } from '@/model/VesselVisitNotification';
import { useSession } from '@/composables/session';
import DatePicker from '@/components/DatePicker.vue';
import SafetyOfficerInput from '@/components/SafetyOfficerInput.vue';
import { useRoute, RouterLink } from 'vue-router';
import ObjectSelector from '@/components/crud/ObjectSelector.vue';
import Loading from '@/components/Loading.vue';

const { t } = useI18n();
const route = useRoute();

const vvnService = container.get<IVesselVisitNotificationService>(TYPES.vesselVisitNotificationService);
const vesselService = container.get<IVesselService>(TYPES.vesselService);
const session = useSession();

const notificationId = String(route.params.id || '');
const loading = ref(false);

const vvn = ref({
  notificationId: '',
  expectedArrival: '',
  expectedDeparture: '',
  isCargoHazardous: false,
  specialRequirements: '',
  crewDetails: {
    captain: { value: '' },
    totalCrewMembers: 0,
    safetyOfficers: [],
  },
  loadCargoManifest: [],
  unloadCargoManifest: [],
  vessel: null,
});

const getMyVessels = async () => {
    
    const res = await vesselService.getVesselByOwner(session.authenticatedUser.email);
    return res;
};

const submitVVN = async (obj: any) => {
    
    obj.expectedArrival = new Date(obj.expectedArrival);
    obj.expectedDeparture = new Date(obj.expectedDeparture);

    obj.loadCargoManifest = obj.loadCargoManifest.map((item: any) => ({
        position: item.position,
        container: item.container,
        storageAreaCode: item.area.nameCode,
    }));

    obj.unloadCargoManifest = obj.unloadCargoManifest.map((item: any) => ({
        position: item.position,
        container: item.container,
        storageAreaCode: item.area.nameCode,
    }));

    await vvnService.updateVesselVisitNotification(new VesselVisitNotification(obj));
};

const openInfo = () => {
  const dialog = document.querySelector('.dialog-overview') as any;
  dialog.show();
};

const closeInfo = () => {
  const dialog = document.querySelector('.dialog-overview') as any;
  dialog.hide();
};

watch(vvn.value, (newVal) => {
  console.log('VVN updated:', newVal);
}, { deep: true });

const downloadSample = () => {
    const sampleCsv = `Bay;Row;Tier;Area code;Container number;Container type;Description`
        + `\n1;1;1;A1;ABC1234567;0;Electronics`;

    const blob = new Blob([sampleCsv], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    link.setAttribute('href', url);
    link.setAttribute('download', 'cargo_manifest_sample.csv');
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

const currentStep = ref(1);
const totalSteps = 4;

const nextStep = () => {
  if (currentStep.value < totalSteps) currentStep.value++;
};

const prevStep = () => {
  if (currentStep.value > 1) currentStep.value--;
};

const isLastStep = () => currentStep.value === totalSteps;

// Load existing VVN data on mount
onMounted(async () => {
  try {

    loading.value = true;

    const data: VesselVisitNotification = await vvnService.getVesselVisitNotificationById(notificationId);
    if (!data) return;
    
    console.log('Loaded VVN data:', data);

    vvn.value.notificationId = data.notificationId || '';
    vvn.value.expectedArrival = data.expectedArrival instanceof Date 
      ? data.expectedArrival.toISOString() 
      : data.expectedArrival || '';
    vvn.value.expectedDeparture = data.expectedDeparture instanceof Date 
      ? data.expectedDeparture.toISOString() 
      : data.expectedDeparture || '';
    vvn.value.isCargoHazardous = data.isCargoHazardous || false;
    vvn.value.specialRequirements = data.specialRequirements || '';
    vvn.value.vessel = data.vessel || '';
    
    vvn.value.crewDetails = {
      captain: { value: data.crewDetails?.captain?.value || '' },
      totalCrewMembers: data.crewDetails?.totalCrewMembers || 0,
      safetyOfficers: data.crewDetails?.safetyOfficers || [],
    };
    
    vvn.value.loadCargoManifest = data.loadCargoManifest || [];
    vvn.value.unloadCargoManifest = data.unloadCargoManifest || [];
    
  } catch (error) {
    console.error('Error loading VVN:', error);
  } finally {
    loading.value = false;
  }
});
</script>

<template>
  <div class="vvn-form">
    <sl-breadcrumb>
      <sl-breadcrumb-item>
        <RouterLink to="/vessel-visit-notifications/dashboard" class="breadcrumb-link">
            {{ t('notification.tabs.dashboard') }}
        </RouterLink>
      </sl-breadcrumb-item>
      <sl-breadcrumb-item>
        <RouterLink to="/vessel-visit-notifications/search" class="breadcrumb-link">
            {{ t('notification.tabs.search') }}
        </RouterLink>
      </sl-breadcrumb-item>
      <sl-breadcrumb-item>
        <RouterLink :to="`/vessel-visit-notifications/view/${notificationId}`" class="breadcrumb-link">
            {{ notificationId }}
        </RouterLink>
    </sl-breadcrumb-item>
      <sl-breadcrumb-item>{{ t('notification.tabs.update') }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <h1 class="title">{{ t('notification.tabs.update') }}</h1>
    <p class="subtitle">{{ t('notification.subtitle.update') }}</p>

    <div class="steps-indicator">
      <p>{{ t("notification.step") }} {{ currentStep }} {{ t("notification.of") }} {{ totalSteps }}</p>
      <progress :value="currentStep" :max="totalSteps"></progress>
    </div>

    <Loading v-if="loading" />
    <EntityForm
        v-else
        :object="vvn"
        :submit-function="submitVVN"
        :editing-id="notificationId"
        :hide-buttons="!isLastStep()"
        >
      <!-- STEP 1 -->
      <div v-if="currentStep === 1" class="step">
        <p class="section-title">{{ t('notification.sections.vesselDetails') }}</p>

        <div style="display: flex; gap: 20px;">
            <div>
                <p>{{ t('notification.fields.expectedArrival') }}*</p>
                <DatePicker
                    v-model="vvn.expectedArrival"
                    input-id="vvn-expectedArrival"
                />
            </div>

            <div>
                <p>{{ t('notification.fields.expectedDeparture') }}*</p>
                <DatePicker
                    v-model="vvn.expectedDeparture"
                    input-id="vvn-expectedDeparture"
                />
            </div>
        </div>
        <br>

        <ObjectSelector
            class="field-dropdown"
            :name="t('vessel.fields.imoNumber.title') + '*'"
            :fetch-function="getMyVessels"
            :placeholderText="t('physicalResource.fields.servingDocks.placeholder')"
            labelKey="imoNumber"
            required
            v-model="vvn.vessel"
        />
      </div>

      <!-- STEP 2 -->
      <div v-if="currentStep === 2" class="step">
        <p class="section-title">{{ t('notification.sections.crewDetails') }}</p>

        <FormField required :name="t('notification.fields.captain')" v-model="vvn.crewDetails.captain.value" placeholderText="Enter Captain Name" type="text" input-id="vvn-captainName" />

        <br>

        <FormField required :name="t('notification.fields.totalCrewMembers')" v-model="vvn.crewDetails.totalCrewMembers" placeholderText="Enter Total Crew Members" type="text" pattern="^\d+$" input-id="vvn-totalCrewMembers" />
      </div>

      <!-- STEP 3 -->
      <div v-if="currentStep === 3" class="step">
        <p class="section-title">{{ t('notification.sections.cargoRequirements') }}</p>

        <FormField :name="t('notification.fields.isCargoHazardousQ')" v-model="vvn.isCargoHazardous" type="checkbox" input-id="vvn-isCargoHazardous" />
        
        <div v-if="vvn.isCargoHazardous">
            <p>{{ t('notification.fields.safetyOfficers') }}</p>
            <SafetyOfficerInput v-model="vvn.crewDetails.safetyOfficers" />
        </div>

        <br>

        <FormField :name="t('notification.fields.specialRequirements')" v-model="vvn.specialRequirements" placeholderText="Enter any special requirements" type="textarea" input-id="vvn-specialRequirements" />
      </div>

      <!-- STEP 4 -->
      <div v-if="currentStep === 4" class="step">
        <p class="section-title">{{ t('notification.sections.cargoContent') }}</p>

        <div class="cargo-manifests">
          <div>
            <p>{{ t('notification.fields.loadCargoManifest') }}</p>
            <CargoManifestReader v-model="vvn.loadCargoManifest" />
          </div>

          <div>
            <p>{{ t('notification.fields.unloadCargoManifest') }}</p>
            <CargoManifestReader v-model="vvn.unloadCargoManifest" />
          </div>

        </div>
        <p class="info" @click="openInfo">
            {{ t('notification.fields.cargoManifestInfo') }}
        </p>
      </div>

      <div class="step-controls">
        <sl-button variant="default" @click="prevStep" :disabled="currentStep === 1">
          {{ t('buttons.pagination.previous') }}
        </sl-button>
    
        <sl-button variant="default" @click="nextStep" :disabled="isLastStep()">
          {{ t('buttons.pagination.next') }}
        </sl-button>
      </div>
    </EntityForm>

    <sl-dialog label="About manifest files" class="dialog-overview"  style="--width: 50vw;">
        {{ t('notification.about.text') }}
        <ul>
            <li>{{ t('notification.about.bay') }}</li>
            <li>{{ t('notification.about.row') }}</li>
            <li>{{ t('notification.about.tier') }}</li>
            <li>{{ t('notification.about.area') }}</li>
            <li>{{ t('notification.about.containerNumber') }}</li>
            <li>{{ t('notification.about.containerType') }}
                <ol>
                    <li>{{ t('notification.about.containerTypes.refrigeratedGoods') }}</li>
                    <li>{{ t('notification.about.containerTypes.generalConsumerProducts') }}</li>
                    <li>{{ t('notification.about.containerTypes.electronics') }}</li>
                    <li>{{ t('notification.about.containerTypes.hazmat') }}</li>
                    <li>{{ t('notification.about.containerTypes.oversizedIndustrialEquipment') }}</li>
                    <li>{{ t('notification.about.containerTypes.other') }}</li>
                </ol>
            </li>
        </ul>

        <sl-button @click="downloadSample" slot="footer" variant="default">
          {{ t('buttons.downloadSample') }}
        </sl-button>
        <sl-button @click="closeInfo" slot="footer" variant="primary">
          {{ t('buttons.ok') }}
        </sl-button>
    </sl-dialog>
  </div>
</template>

<style scoped>
.steps-indicator {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin-bottom: 1.5rem;
}

.step {
  margin-bottom: 2rem;
}

.step-controls {
  display: flex;
  justify-content: space-between;
}

.cargo-manifests {
  display: flex;
  flex-direction: row;
  gap: 20px;
}

.info {
  font-size: 0.9rem;
  color: var(--sl-color-primary-600);
  margin-top: 0.5rem;
  margin-bottom: 0;
}

.info:hover {
  cursor: pointer;
}
</style>
