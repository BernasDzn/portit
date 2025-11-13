<script setup lang="ts">
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import CargoManifestReader from '@/components/CargoManifestReader.vue';
import { ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IVesselVisitNotificationService } from '@/service/IService/IVesselVisitNotificationService';
import type { IVesselService } from '@/service/IService/IVesselService';
import type { VesselVisitNotificationDto } from '@/model/dto/VesselVisitNotificationDto';
import { useSession } from '@/composables/session';
import DatePicker from '@/components/DatePicker.vue';
import SafetyOfficerInput from '@/components/SafetyOfficerInput.vue';

const { t } = useI18n();

const vvnService = container.get<IVesselVisitNotificationService>(TYPES.vesselVisitNotificationService);
const vesselService = container.get<IVesselService>(TYPES.vesselService);
const session = useSession();

const vvn = ref<VesselVisitNotificationDto>({
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
  vesselImoNumber: '',
});

const getMyVessels = async () => {
  const res = await vesselService.getVesselByOwner(session.authenticatedUser.email);
  return res;
};

const submitVVN = async (obj: VesselVisitNotificationDto) => {
  await vvnService.createVesselVisitNotification(obj);
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
</script>

<template>
  <div class="vvn-form">
    <sl-breadcrumb>
      <sl-breadcrumb-item>
        <RouterLink to="/vessel-visit-notifications/dashboard" class="breadcrumb-link">
            {{ t('notification.tabs.dashboard') }}
        </RouterLink>
      </sl-breadcrumb-item>
      <sl-breadcrumb-item>{{ t('notification.tabs.create') }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <h1 class="title">{{ t('notification.tabs.create') }}</h1>
    <p class="subtitle">{{ t('notification.subtitle.create') }}</p>

    <div class="steps-indicator">
      <p>Step {{ currentStep }} of {{ totalSteps }}</p>
      <progress :value="currentStep" :max="totalSteps"></progress>
    </div>

    <EntityForm
        :object="vvn"
        :submit-function="submitVVN"
        :hide-buttons="!isLastStep()"
        >
      <!-- STEP 1 -->
      <div v-if="currentStep === 1" class="step">
        <p class="section-title">Visit Information</p>

        <div style="display: flex; gap: 20px;">
            <div>
                <p> Expected Arrival*</p>
                <DatePicker
                    v-model="vvn.expectedArrival"
                />
            </div>

            <div>
                <p> Expected Departure*</p>
                <DatePicker
                    v-model="vvn.expectedDeparture"
                />
            </div>
        </div>
        <br>

        <EntityDropdown
          name="Vessel IMO*"
          v-model="vvn.vesselImoNumber"
          :fetch-function="getMyVessels"
          :fetch-on-mount="true"
          placeholderText="Select vessel"
          valueKey="imoNumber"
          labelKey="imoNumber"
          required
        />
      </div>

      <!-- STEP 2 -->
      <div v-if="currentStep === 2" class="step">
        <p class="section-title">Crew Details</p>

        <FormField required name="Captain Name*" v-model="vvn.crewDetails.captain.value" placeholderText="Enter Captain Name" type="text" />

        <br>

        <FormField required name="Total Crew Members*" v-model="vvn.crewDetails.totalCrewMembers" placeholderText="Enter Total Crew Members" type="text" pattern="^\d+$" />
      </div>

      <!-- STEP 3 -->
      <div v-if="currentStep === 3" class="step">
        <p class="section-title">Cargo Requirements</p>

        <FormField name="Is Cargo Hazardous?" v-model="vvn.isCargoHazardous" type="checkbox" />
        
        <div v-if="vvn.isCargoHazardous">
            <p>Safety officers</p>
            <SafetyOfficerInput v-model="vvn.crewDetails.safetyOfficers" />
        </div>

        <br>

        <FormField name="Special Requirements" v-model="vvn.specialRequirements" placeholderText="Enter any special requirements" type="textarea" />
      </div>

      <!-- STEP 4 -->
      <div v-if="currentStep === 4" class="step">
        <p class="section-title">Cargo Contents</p>

        <div class="cargo-manifests">
          <div>
            <p>Cargo load manifest</p>
            <CargoManifestReader v-model="vvn.loadCargoManifest" />
          </div>

          <div>
            <p>Cargo unload manifest</p>
            <CargoManifestReader v-model="vvn.unloadCargoManifest" />
          </div>

        </div>
        <p class="info" @click="openInfo">
          About cargo manifest files
        </p>
      </div>

      <div class="step-controls">
        <sl-button variant="default" @click="prevStep" :disabled="currentStep === 1">
          Previous
        </sl-button>
    
        <sl-button variant="default" @click="nextStep" :disabled="isLastStep()">
          Next
        </sl-button>
      </div>
    </EntityForm>

    <sl-dialog label="About manifest files" class="dialog-overview"  style="--width: 50vw;">
        
        Cargo manifest files (currently) must be in a european standard CSV format. Each row in the CSV file represents a cargo item with the following columns:
        <ul>
            <li>Bay (number)</li>
            <li>Row (number)</li>
            <li>Tier (number)</li>
            <li>Area code (where to/from)</li>
            <li>Container number (text)</li>
            <li>Container type (number)</li>
            <ol>
                <li>REGRIGERATED_GOODS</li>
                <li>GENERAL_CONSUMER_PRODUCTS</li>
                <li>ELECTRONICS</li>
                <li>HAZMAT </li>
                <li>OVERSIZED_INDUSTRIAL_EQUIPMENT</li>
                <li>OTHER</li>
            </ol>
            <li>Description (text)</li>
        </ul>

        <sl-button @click="downloadSample" slot="footer" variant="default">Download sample</sl-button>
        <sl-button @click="closeInfo" slot="footer" variant="primary">Ok</sl-button>
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
