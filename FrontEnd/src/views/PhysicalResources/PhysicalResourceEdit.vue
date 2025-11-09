<script setup lang="ts">
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import OperationalWindowPicker from '@/components/OperationalWindowPicker.vue';
import { useAlerts } from '@/composables/alerts';
import type { PhysicalResource, STSCrane, Truck, YardCrane } from '@/model/PhysicalResource';
import AxiosHttpService from '@/service/AxiosHttpService';
import { DockService } from '@/service/DockService';
import { PhysicalResourceService } from '@/service/PhysicalResourceService';
import { QualificationService } from '@/service/QualificationService';
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute } from 'vue-router';

const { t } = useI18n();


const route = useRoute();
const resourceCode = String(route.params.code || '');

const notifications = useAlerts();

const http = new AxiosHttpService();
const resourceService = new PhysicalResourceService(http);
const dockService = new DockService(http);
const qualificationService = new QualificationService(http);

const genericResourse = ref<any>({
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
    operationalWindow: {
        shifts: []
    }
});

const type = ref(0);

const statusValues = {
    'Available': 0,
    'Maintenance': 1,
    'Out of Service': 2
};

const updateSTSResource = (obj: any) => {

    if (!resourceCode) {
        notifications.enqueueNotification(
            'Cannot update physical resources at this time.',
            notifications.notificationTypes.DANGER
        );
        return;
    }

    const STSObject: STSCrane = {
        type: "STS Crane",
        code: obj.code,
        description: obj.description,
        status: statusValues[obj.status],
        setupTimeInMinutes: obj.setupTime,
        operationalWindow: obj.operationalWindow,
        qualificationsCodes: obj.qualifications,
        liftingCapacity: obj.liftingCapacity,
        servingDockCode: obj.servingDock,
        containersPerHour: obj.containersPerHour
    };
    return resourceService.updateSTSCrane(resourceCode, STSObject);
};

const updateYardGantryResource = (obj: any) => {

    if (!resourceCode) {
        notifications.enqueueNotification(
            'Cannot update qualifications at this time.',
            notifications.notificationTypes.DANGER
        );
        return;
    }

    const yardGantryObject: YardCrane = {
        type: "Yard Crane",
        code: obj.code,
        description: obj.description,
        status: statusValues[obj.status],
        setupTimeInMinutes: obj.setupTime,
        operationalWindow: obj.operationalWindow,
        qualificationsCodes: obj.qualifications,
        liftingCapacity: obj.liftingCapacity,
        containersPerHour: obj.containersPerHour
    };
    return resourceService.updateYardCrane(resourceCode, yardGantryObject);
};

const updateTruckResource = (obj: any) => {
    
    if (!resourceCode) {
        notifications.enqueueNotification(
            'Cannot update qualifications at this time.',
            notifications.notificationTypes.DANGER
        );
        return;
    }

    const truckObject: Truck = {
        type: "Truck",
        code: obj.code,
        description: obj.description,
        status: statusValues[obj.status],
        setupTimeInMinutes: obj.setupTime,
        operationalWindow: obj.operationalWindow,
        qualificationsCodes: obj.qualifications,
        maxLoadCapacity: obj.maxLoadCapacity,
        averageSpeed: obj.averageSpeed,
        containersPerTrip: obj.containersPerTrip
    };
    return resourceService.updateTruck(resourceCode, truckObject);
};

const update = () => {

    // Map status back to index

    if (type.value == 0) {
        return updateSTSResource(genericResourse.value);
    } else if (type.value == 1) {
        return updateYardGantryResource(genericResourse.value);
    } else {
        return updateTruckResource(genericResourse.value);
    }
}

const getById = async () => {

    const res: any = await resourceService.getPhysicalResourceById(resourceCode);
    if (res.servingDock != undefined) type.value = 0;
    else if (res.averageSpeed != undefined) type.value = 2;
    else type.value = 1;

    return res;
}

onMounted(async () => {
    if (!resourceCode) return;
    try {
        const data = await getById();
        genericResourse.value.code = data.code;
        genericResourse.value.description = data.description;
        const statusKey = Object.keys(statusValues);
        genericResourse.value.status = statusKey[data.status];
        genericResourse.value.setupTime = data.setupTimeInMinutes;
        genericResourse.value.operationalWindow = data.operationalWindow;
        genericResourse.value.qualifications = data.qualifications?.map((q: any) => q.idCode) || [];

        if (type.value == 0) {
            genericResourse.value.liftingCapacity = data.liftingCapacity;
            genericResourse.value.servingDock = data.servingDock.code;
            genericResourse.value.containersPerHour = data.containersPerHour;
        } else if (type.value == 1) {
            genericResourse.value.liftingCapacity = data.liftingCapacity;
            genericResourse.value.containersPerHour = data.containersPerHour;
        } else {
            genericResourse.value.maxLoadCapacity = data.maxLoadCapacity;
            genericResourse.value.averageSpeed = data.averageSpeed;
            genericResourse.value.containersPerTrip = data.containersPerTrip;
        }

    } catch (err) {
        console.error('Failed to load physical resource', err);
    }
});

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item><RouterLink to="/resources/dashboard" class="breadcrumb-link">{{ t('physicalResource.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item><RouterLink to="/resources/search" class="breadcrumb-link">{{ t('physicalResource.tabs.search') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('physicalResource.tabs.edit') }}</sl-breadcrumb-item>
        </sl-breadcrumb>
        
        <h1 class="title">{{ t('physicalResource.tabs.edit') }}</h1>
        <p class="subtitle">{{ t('physicalResource.subtitle.edit') }}</p>

        <EntityForm
            :editing-id="resourceCode"
            :object="genericResourse" 
            :submit-function="update"
            class="group"
        >

        <div class="group">
            <div class="form">

                <FormField 
                    :enabled="false"
                    :required="true" 
                    class="field" 
                    :name="`${t('physicalResource.fields.code.title')}*`" 
                    v-model="genericResourse.code" 
                    :placeholderText="t('physicalResource.fields.code.placeholder')" 
                    pattern="^[a-zA-Z0-9]+$" 
                />
    
                <span class="section-divider"></span>
    
                <FormField 
                    :required="true" 
                    class="field" 
                    :name="`${t('physicalResource.fields.description.title')}*`" 
                    v-model="genericResourse.description" 
                    :placeholderText="t('physicalResource.fields.description.placeholder')" 
                />

                <span class="section-divider"></span>

                <EntityDropdown
                    class="field-dropdown"
                    :name="`${t('physicalResource.fields.status.title')}*`"
                    v-model="genericResourse.status"
                    :items="['Available', 'Maintenance', 'Out of Service']"
                    :placeholderText="t('physicalResource.fields.status.placeholder')"
                    required
                />
            </div>
            <br>
            <div class="form">    
                <!-- <EntityDropdown
                    class="field-dropdown"
                    :name="`${t('physicalResource.fields.qualifications.title')}*`"
                    v-model="genericResourse.qualifications"
                    :fetch-function="() => qualificationService.getQualifications()"
                    :fetch-on-mount="true"
                    :placeholderText="t('physicalResource.fields.qualifications.placeholder')"
                    valueKey="idCode"
                    labelKey="idCode"
                    multiple
                    required
                /> -->
                <EntityDropdown
                    class="field-dropdown"
                    :name="t('staff.fields.qualifications.title') + '*'"
                    v-model="genericResourse.qualifications"
                    :fetch-function="() => qualificationService.getQualifications().then(page => (page.items || []).map(t => t.idCode))"
                    :fetch-on-mount="true"
                    :placeholderText="t('staff.fields.qualifications.placeholder')"
                    :required="false"
                    :multiple="true"
                    valueKey="idCode"
                    labelKey="name"
                />

                <span class="section-divider"></span>

                <FormField 
                    :required="true" 
                    class="field" 
                    :name="`${t('physicalResource.fields.setupTime.title')}*`"
                    v-model="genericResourse.setupTime"
                    :placeholderText="t('physicalResource.fields.setupTime.placeholder')"
                    pattern="^[0-9]+$"
                />
            </div>

            <div style="flex:100%; width: 100%;">
                <OperationalWindowPicker
                    v-if="genericResourse.operationalWindow && genericResourse.code"
                    v-model="genericResourse.operationalWindow"
                />
            </div>
        </div>
            
        <p class="section-title">{{ t('physicalResource.specificFields') }}</p>

        <div name="general" v-if="type == 0">

            <EntityDropdown
                class="field-dropdown"
                :name="`${t('physicalResource.fields.servingDocks.title')}*`"
                v-model="genericResourse.servingDock"
                :fetch-function="() => dockService.getDocks()"
                :fetch-on-mount="true"
                :placeholderText="t('physicalResource.fields.servingDocks.placeholder')"
                valueKey="code"
                labelKey="name"
                required
            />

            <br>

            <FormField 
                :required="true" 
                class="field" 
                :name="`${t('physicalResource.fields.liftingCapacity.title')}*`" 
                v-model="genericResourse.liftingCapacity" 
                :placeholderText="t('physicalResource.fields.liftingCapacity.placeholder')"
                pattern="^[0-9]+$"
            />

            <FormField 
                :required="true" 
                class="field" 
                :name="`${t('physicalResource.fields.containersPerHour.title')}*`" 
                v-model="genericResourse.containersPerHour" 
                :placeholderText="t('physicalResource.fields.containersPerHour.placeholder')"
                pattern="^[0-9]+$"
            />
                
        </div>
        <div name="custom" v-if="type == 1">
        
            <FormField 
                :required="true" 
                class="field" 
                :name="`${t('physicalResource.fields.liftingCapacity.title')}*`" 
                v-model="genericResourse.liftingCapacity" 
                :placeholderText="t('physicalResource.fields.liftingCapacity.placeholder')"
                pattern="^[0-9]+$"
            />

            <FormField 
                :required="true" 
                class="field" 
                :name="`${t('physicalResource.fields.containersPerHour.title')}*`" 
                v-model="genericResourse.containersPerHour" 
                :placeholderText="t('physicalResource.fields.containersPerHour.placeholder')"
                pattern="^[0-9]+$"
            />
                
        </div>
        <div name="advanced" v-if="type == 2">
            
            <FormField 
                :required="true" 
                class="field" 
                :name="`${t('physicalResource.fields.maxLoadCapacity.title')}*`" 
                v-model="genericResourse.maxLoadCapacity" 
                :placeholderText="t('physicalResource.fields.maxLoadCapacity.placeholder')"
                pattern="^[0-9]+$"
            />

            <FormField 
                :required="true" 
                class="field" 
                :name="`${t('physicalResource.fields.averageSpeed.title')}*`" 
                v-model="genericResourse.averageSpeed" 
                :placeholderText="t('physicalResource.fields.averageSpeed.placeholder')"
                pattern="^[0-9]+$"
            />

            <FormField 
                :required="true" 
                class="field" 
                :name="`${t('physicalResource.fields.containersPerTrip.title')}*`" 
                v-model="genericResourse.containersPerTrip" 
                :placeholderText="t('physicalResource.fields.containersPerTrip.placeholder')"
                pattern="^[0-9]+$"
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

.form{
    display: flex;
    flex-direction: row;
}

.group {
    margin: 5px;
}

</style>