<script setup lang="ts">
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import type { PhysicalResource, STSCrane, Truck, YardCrane } from '@/model/PhysicalResource';
import AxiosHttpService from '@/service/AxiosHttpService';
import { DockService } from '@/service/DockService';
import { PhysicalResourceService } from '@/service/PhysicalResourceService';
import { QualificationService } from '@/service/QualificationService';
import { ref } from 'vue';

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

const statuses = ["Available","In maintenance","Out of service"];

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
            <sl-breadcrumb-item><RouterLink to="/resources/dashboard" class="breadcrumb-link">Physical Resources Dashboard</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>Create Physical Resource</sl-breadcrumb-item>
        </sl-breadcrumb>
        
        <h1 class="title">Create Create Physical Resource</h1>
        <p class="subtitle">Register a new physical resource into the system</p>

        <sl-tab-group>
            <sl-tab slot="nav" panel="general">S-T-S Crane</sl-tab>
            <sl-tab slot="nav" panel="custom">Yard Gantry Crane</sl-tab>
            <sl-tab slot="nav" panel="advanced">Trucks and terminal tractors</sl-tab>
            
            <sl-tab-panel name="general">
                
                <EntityForm
                :object="genericResourse" 
                :submit-function="submitSTSResource"
                class="group"
                >
                    <p class="section-title">General fields</p>

                    <div class="group">
                        <div class="form">
        
            
                            <FormField 
                                :required="true" 
                                class="field" 
                                name="Resource code*" 
                                v-model="genericResourse.code" 
                                placeholderText="Resource code" 
                                pattern="^[a-zA-Z0-9]+$" 
                            />
                
                            <span class="section-divider"></span>
                
                            <FormField 
                                :required="true" 
                                class="field" 
                                name="Resource description*" 
                                v-model="genericResourse.description" 
                                placeholderText="Resource description"
                            />
            
                            <span class="section-divider"></span>
            
                            <EntityDropdown
                                class="field-dropdown"
                                name="Resource Status*"
                                v-model="genericResourse.status"
                                :items="statuses"
                                placeholderText="Select resource status"
                                required
                            />
                        </div>
                        <br>
                        <div class="form">    
                            <EntityDropdown
                                class="field-dropdown"
                                name="Required qualifications*"
                                v-model="genericResourse.qualifications"
                                :fetch-function="() => qualificationService.getQualifications()"
                                :fetch-on-mount="true"
                                placeholderText="The qualifications required to operate this resource"
                                valueKey="idCode"
                                labelKey="idCode"
                                multiple
                                required
                            />
        
                            <span class="section-divider"></span>
        
                            <FormField 
                                :required="true" 
                                class="field" 
                                name="Setup time (minutes)*"
                                v-model="genericResourse.setupTime"
                                placeholderText="Setup time"
                                pattern="^[0-9]+$"
                            />
                        </div>
        
                    </div>
                
                    <p class="section-title">Specific fields</p>

                    <EntityDropdown
                        class="field-dropdown"
                        name="Serving Dock*"
                        v-model="genericResourse.servingDock"
                        :fetch-function="() => dockService.getDocks()"
                        :fetch-on-mount="true"
                        placeholderText="Select serving dock"
                        valueKey="code"
                        labelKey="name"
                        required
                    />

                    <FormField 
                        :required="true" 
                        class="field" 
                        name="Lifting capacity (kgs)*" 
                        v-model="genericResourse.liftingCapacity" 
                        placeholderText="Lifting capacity"
                        pattern="^[0-9]+$"
                    />

                    <FormField 
                        :required="true" 
                        class="field" 
                        name="Containers per hour*" 
                        v-model="genericResourse.containersPerHour" 
                        placeholderText="Containers per hour"
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

                <p class="section-title">General fields</p>

                <div class="group">
                    <div class="form">
    
        
                        <FormField 
                            :required="true" 
                            class="field" 
                            name="Resource code*" 
                            v-model="genericResourse.code" 
                            placeholderText="Resource code" 
                            pattern="^[a-zA-Z0-9]+$" 
                        />
            
                        <span class="section-divider"></span>
            
                        <FormField 
                            :required="true" 
                            class="field" 
                            name="Resource description*" 
                            v-model="genericResourse.description" 
                            placeholderText="Resource description"
                        />
        
                        <span class="section-divider"></span>
        
                        <EntityDropdown
                            class="field-dropdown"
                            name="Resource Status*"
                            v-model="genericResourse.status"
                            :items="statuses"
                            placeholderText="Select resource status"
                            required
                        />
                    </div>
                    <br>
                    <div class="form">    
                        <EntityDropdown
                            class="field-dropdown"
                            name="Required qualifications*"
                            v-model="genericResourse.qualifications"
                            :fetch-function="() => qualificationService.getQualifications()"
                            :fetch-on-mount="true"
                            placeholderText="The qualifications required to operate this resource"
                            valueKey="idCode"
                            labelKey="idCode"
                            multiple
                            required
                        />
    
                        <span class="section-divider"></span>
    
                        <FormField 
                            :required="true" 
                            class="field" 
                            name="Setup time (minutes)*"
                            v-model="genericResourse.setupTime"
                            placeholderText="Setup time"
                            pattern="^[0-9]+$"
                        />
                    </div>
    
                </div>
            
                <p class="section-title">Specific fields</p>

                    <FormField 
                        :required="true" 
                        class="field" 
                        name="Lifting capacity (kg)*" 
                        v-model="genericResourse.liftingCapacity" 
                        placeholderText="Lifting capacity"
                        pattern="^[0-9]+$"
                    />

                    <FormField 
                        :required="true" 
                        class="field" 
                        name="Containers per hour*" 
                        v-model="genericResourse.containersPerHour" 
                        placeholderText="Containers per hour"
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
                <p class="section-title">General fields</p>

                <div class="group">
                    <div class="form">
    
        
                        <FormField 
                            :required="true" 
                            class="field" 
                            name="Resource code*" 
                            v-model="genericResourse.code" 
                            placeholderText="Resource code" 
                            pattern="^[a-zA-Z0-9]+$" 
                        />
            
                        <span class="section-divider"></span>
            
                        <FormField 
                            :required="true" 
                            class="field" 
                            name="Resource description*" 
                            v-model="genericResourse.description" 
                            placeholderText="Resource description"
                        />
        
                        <span class="section-divider"></span>
        
                        <EntityDropdown
                            class="field-dropdown"
                            name="Resource Status*"
                            v-model="genericResourse.status"
                            :items="statuses"
                            placeholderText="Select resource status"
                            required
                        />
                    </div>
                    <br>
                    <div class="form">    
                        <EntityDropdown
                            class="field-dropdown"
                            name="Required qualifications*"
                            v-model="genericResourse.qualifications"
                            :fetch-function="() => qualificationService.getQualifications()"
                            :fetch-on-mount="true"
                            placeholderText="The qualifications required to operate this resource"
                            valueKey="idCode"
                            labelKey="idCode"
                            multiple
                            required
                        />
    
                        <span class="section-divider"></span>
    
                        <FormField 
                            :required="true" 
                            class="field" 
                            name="Setup time (minutes)*"
                            v-model="genericResourse.setupTime"
                            placeholderText="Setup time"
                            pattern="^[0-9]+$"
                        />
                    </div>
    
                </div>
            
                <p class="section-title">Specific fields</p>

                    <FormField 
                        :required="true" 
                        class="field" 
                        name="Maximum load capacity (kg)*" 
                        v-model="genericResourse.maxLoadCapacity" 
                        placeholderText="Max load capacity"
                        pattern="^[0-9]+$"
                    />

                    <FormField 
                        :required="true" 
                        class="field" 
                        name="Average speed (km/h)*" 
                        v-model="genericResourse.averageSpeed" 
                        placeholderText="Average speed"
                        pattern="^[0-9]+$"
                    />

                    <FormField 
                        :required="true" 
                        class="field" 
                        name="Containers per trip*" 
                        v-model="genericResourse.containersPerTrip" 
                        placeholderText="Containers per trip"
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