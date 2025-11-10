<script setup lang="ts">
import DashboardItem from '@/components/DashboardItem.vue';

import AxiosHttpService from '@/service/AxiosHttpService';
import Loading from '@/components/Loading.vue';
import { ref, onMounted } from 'vue';
import { AdminService } from '@/service/AdminService';
import { useI18n } from 'vue-i18n';
import type { IAdminService } from '@/service/IService/IAdminService';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';

const { t } = useI18n();

const adminService = container.get<IAdminService>(TYPES.adminService)
onMounted(async () => {

});

</script>

<template>
    <div>
        <h1 class="title">{{ t('admin.title') }}</h1>
        <p class="subtitle">{{ t('admin.subtitle') }}</p>
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
            </div>
        </sl-card>
    </div>
</template>