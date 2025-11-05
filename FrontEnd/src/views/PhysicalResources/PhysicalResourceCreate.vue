<script setup lang="ts">
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import OperationalWindowPicker from '@/components/OperationalWindowPicker.vue';
import type { PhysicalResource, STSCrane, Truck, YardCrane } from '@/model/PhysicalResource';
import AxiosHttpService from '@/service/AxiosHttpService';
import { DockService } from '@/service/DockService';
import { PhysicalResourceService } from '@/service/PhysicalResourceService';
import { QualificationService } from '@/service/QualificationService';
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

const http = new AxiosHttpService();
const resourceService = new PhysicalResourceService(http);
const dockService = new DockService(http);
const qualificationService = new QualificationService(http);

const genericResourse = ref<any>({
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
    operationalWindow: {
        shifts: []
    }
});

const statuses = [t('physicalResource.fields.status.options.available'),t('physicalResource.fields.status.options.maintenance'),t('physicalResource.fields.status.options.outOfService')];

const submitSTSResource = (obj: any) => {
    const STSObject: STSCrane = {
        type: "STS Crane",
        code: obj.code,
        description: obj.description,
        status: statuses.indexOf(obj.status),
        setupTimeInMinutes: obj.setupTime,
        operationalWindow: obj.operationalWindow,
        qualificationsCodes: obj.qualifications,
        liftingCapacity: obj.liftingCapacity,
        servingDockCode: obj.servingDock,
        containersPerHour: obj.containersPerHour
    };
    return resourceService.addSTSCrane(STSObject);
};

const submitYardGantryResource = (obj: any) => {
    const yardGantryObject: YardCrane = {
        type: "Yard Crane",
        code: obj.code,
        description: obj.description,
        status: statuses.indexOf(obj.status),
        setupTimeInMinutes: obj.setupTime,
        operationalWindow: obj.operationalWindow,
        qualificationsCodes: obj.qualifications,
        liftingCapacity: obj.liftingCapacity,
        containersPerHour: obj.containersPerHour
    };
    return resourceService.addYardCrane(yardGantryObject);
};

const submitTruckResource = (obj: any) => {
    const truckObject: Truck = {
        type: "Truck",
        code: obj.code,
        description: obj.description,
        status: statuses.indexOf(obj.status),
        setupTimeInMinutes: obj.setupTime,
        operationalWindow: obj.operationalWindow,
        qualificationsCodes: obj.qualifications,
        maxLoadCapacity: obj.maxLoadCapacity,
        averageSpeed: obj.averageSpeed,
        containersPerTrip: obj.containersPerTrip
    };
    return resourceService.addTruck(truckObject);
};

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item><RouterLink to="/resources/dashboard" class="breadcrumb-link">{{ t('physicalResource.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('physicalResource.tabs.create') }}</sl-breadcrumb-item>
        </sl-breadcrumb>
        
        <h1 class="title">{{ t('physicalResource.tabs.create') }}</h1>
        <p class="subtitle">{{ t('physicalResource.subtitle.create') }}</p>

        <sl-tab-group>
            <sl-tab slot="nav" panel="general">{{ t('physicalResource.fields.type.options.stsCrane') }}</sl-tab>
            <sl-tab slot="nav" panel="custom">{{ t('physicalResource.fields.type.options.yardGantry') }}</sl-tab>
            <sl-tab slot="nav" panel="advanced">{{ t('physicalResource.fields.type.options.truck') }}</sl-tab>

            <sl-tab-panel name="general">
                
                <EntityForm
                    :object="genericResourse" 
                    :submit-function="submitSTSResource"
                    class="group"
                >
                    <p class="section-title">{{ t('physicalResource.generalFields') }}</p>

                    <div class="group">
                        <div class="form">
        
            
                            <FormField 
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
                                :items="statuses"
                                :placeholderText="t('physicalResource.fields.status.placeholder')"
                                required
                            />
                        </div>
                        <br>
                        <div class="form">    
                            <EntityDropdown
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
        
                    </div>

                    <div style="flex:100%; width: 100%;">
                        <OperationalWindowPicker
                             v-model="genericResourse.operationalWindow"
                        />
                     </div>
                
                    <p class="section-title">{{ t('physicalResource.specificFields') }}</p>

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
                    
                </EntityForm>
            </sl-tab-panel>
            <sl-tab-panel name="custom">
                <EntityForm
                    :object="genericResourse" 
                    :submit-function="submitYardGantryResource"
                    class="group"
                >

                <p class="section-title">{{ t('physicalResource.generalFields') }}</p>

                <div class="group">
                    <div class="form">
    
        
                        <FormField 
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
                            :items="statuses"
                            :placeholderText="t('physicalResource.fields.status.placeholder')"
                            required
                        />
                    </div>
                    <br>
                    <div class="form">    
                        <EntityDropdown
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
    
                </div>
            
                <p class="section-title">Specific fields</p>

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
                    
                </EntityForm>
            </sl-tab-panel>
            <sl-tab-panel name="advanced">
                
                <EntityForm
                    :object="genericResourse" 
                    :submit-function="submitTruckResource"
                    class="group"
                >
                <p class="section-title">{{ t('physicalResource.generalFields') }}</p>

                <div class="group">
                    <div class="form">
    
        
                        <FormField 
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
                            :items="statuses"
                            :placeholderText="t('physicalResource.fields.status.placeholder')"
                            required
                        />
                    </div>
                    <br>
                    <div class="form">    
                        <EntityDropdown
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
    
                </div>
            
                <p class="section-title">{{ t('physicalResource.specificFields') }}</p>

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
                    
                </EntityForm>
                
            </sl-tab-panel>
          </sl-tab-group>

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