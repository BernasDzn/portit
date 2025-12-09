<script setup lang="ts">
import { useRoute } from 'vue-router';
import { useAlerts } from '@/composables/alerts';
import { reactive, ref } from 'vue';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import { useI18n } from 'vue-i18n';
import { container } from '@/inversify.config';
import type { IQualificationService } from '@/service/IService/IQualificationService';
import TYPES from '@/inversify/types';
import type { QualificationDto } from '@/model/dto/QualificationDto';
import { Qualification } from '@/model/Qualifications';
import type { IOperationPlanService } from '@/service/IService/IOperationPlanService';
import type { OperationPlanDto } from '@/model/dto/OperationPlanDto';
import XGantt from "@xpyjs/gantt";

const route = useRoute();
const planId = String(route.params.id || '');

const notifications = useAlerts();

const planService = container.get<IOperationPlanService>(TYPES.operationPlanService);

const plan = ref<OperationPlanDto>({
    id: '',
    relatedVVN: '',
    dock: '',
	operationSchedule: [],
	metadata: {
		createdBy: '',
		createdAt: '',
		algorithmUsed: ''
    }
});

const updatePlan = async () => {
};

const dataList = [
    {
        index: 1,
        startDate: "2020-06-05",
        endDate: "2020-08-20",
        ttt: {
            a: "aaa",
            b: "bbb"
        },
        name: "mydata1",
        children: [] // children is required. If no child, empty array is ok.
    },
    {
        index: 2,
        startDate: "2020-07-07",
        endDate: "2020-09-11",
        ttt: {},
        name: "mydata2",
        children: [
            {
                index: 3,
                startDate: "2020-07-10",
                endDate: "2020-08-15",
                ttt: {
                    a: "aaa"
                },
                name: "child1",
                children: [] // children is required. If no child, empty array is ok.
            }
        ]
    }
];

const { t } = useI18n();

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item><RouterLink to="/scheduling-dashboard" class="breadcrumb-link">{{ t('scheduling.plans.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item><RouterLink to="/scheduling/plans-search" class="breadcrumb-link">{{ t('scheduling.plans.tabs.search') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ t('scheduling.plans.tabs.edit') }}</sl-breadcrumb-item>
        </sl-breadcrumb>
        
        <h1 class="title">{{ t('scheduling.plans.tabs.edit') }}</h1>
        <p class="subtitle">{{ t('scheduling.plans.subtitle.edit') }}</p>
        
        <div style="height: 500px; border: 1px solid red;">
            <XGantt data-id="index" :data="dataList" />
        </div>          

    </div>
</template>