<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';

import AxiosHttpService from '@/service/AxiosHttpService';
import Loading from '@/components/Loading.vue';
import { ref, onMounted } from 'vue';
import { AdminService } from '@/service/AdminService';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

</script>

<template>
    <div>
        <h1 class="title">{{ t('admin.title') }}</h1>
        <h class="subtitle">{{ t('admin.subtitle') }}</h>
        <sl-card class="dashboard-overview">
            <div class="dashboard-items">
                <DashboardItem
                    :title="t('admin.userManagement.title')"
                    :description="t('admin.userManagement.subtitle')"
                    icon="manage_accounts"
                    to="/admin/users"
                />
                <DashboardItem
                    :title="t('admin.auditLogs.title')"
                    :description="t('admin.auditLogs.subtitle')"
                    icon="history"
                    to="/admin/audit-logs"
                />
                <DashboardItem
                    :title="t('admin.pp.title')"
                    :description="t('admin.pp.subtitle')"
                    icon="privacy_tip"
                    to="/admin/privacy-policy"
                />
            </div>
        </sl-card>
    </div>
</template>