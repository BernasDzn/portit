<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';
import Loading from '@/components/Loading.vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IQualificationService } from '@/service/IService/IQualificationService';
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

const qualificationService = container.get<IQualificationService>(TYPES.qualificationService);

const { t } = useI18n();

const numberOfQualifications = ref(0);
const loading = ref(true)
onMounted(async () => {
    try {
        numberOfQualifications.value = await qualificationService.getNumberOfQualifications();
        loading.value = false;
    } catch (err) {
        console.error('Failed to load qualifications', err);
    }
});
</script>

<template>
    <div>
        <h1 class="title">{{ t('qualification.tabs.dashboard') }}</h1>
        <p class="subtitle">{{ t('qualification.subtitle.dashboard') }}</p>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem :title="t('qualification.tabs.view_dashboard')"
                    :description="t('qualification.subtitle.search')" icon="search" to="/qualifications/search" />
                <DashboardItem :title="t('qualification.tabs.create')" :description="t('qualification.subtitle.create')"
                    icon="add" to="/qualifications/create" />
            </div>
        </sl-card>
        <sl-card class="dashboard-statistics">
            <div class="stats-overview" v-if="!loading">
                <p>{{ numberOfQualifications }}</p>
                <p>{{ t('qualification.registeredQualifications') }}</p>
            </div>
            <Loading v-if="loading" />
        </sl-card>
    </div>
</template>

<style scoped>
.stats-overview {
    text-align: center;
    margin-bottom: 1rem;
    border-radius: 0.5rem;
    padding: 1rem;
    background-color: #f5f6fa;
}

.stats-overview p {
    margin: 0;
    padding: 0.5rem;
    color: #485ea9;
}

.stats-overview p:nth-child(1) {
    font-size: 2rem;
}
</style>