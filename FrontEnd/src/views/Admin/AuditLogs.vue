<script setup lang="ts">
import DataTable from '@/components/crud/DataTable.vue';
import { onMounted, ref, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import type { Logs } from '@/model/Logs';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IAdminService } from '@/service/IService/IAdminService';

const { t } = useI18n();

const adminService = container.get<IAdminService>(TYPES.adminService);

const logs = ref<Logs[]>([] as Logs[]);

const columns = [
  'timestamp',
  'level',
  'message',
  'requestId',
  'requestType'
];

const fetchLogs = async () => {
  const logList: Logs[] = await adminService.getLogs();
  logs.value = logList;
};

onMounted(() => {
  void fetchLogs();
});

// Map numeric level codes to human-friendly labels
const levelMap: Record<string, string> = {
  '1000': 'Create',
  '1001': 'Update',
  '1002': 'Delete',
  '1003': 'Deactivate',
  '1004': 'Retrieve',
  '1005': 'Filter'
};

const mappedLogs = computed(() =>
  logs.value.map(l => ({
    ...l,
    // Map based on the numeric requestId (the log's Id field), not the textual Level (INF/ERR)
    requestType: levelMap[String(l.requestId)] ?? String(l.requestId)
  }))
);
</script>

<template>
  <div>

    <sl-breadcrumb>
      <sl-breadcrumb-item><RouterLink to="/admin/dashboard" class="breadcrumb-link">{{ t('admin.sidebarTitle') }}</RouterLink></sl-breadcrumb-item>
      <sl-breadcrumb-item>Audit logs</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
      <h1 class="title">Audit logs</h1>
      <p class="subtitle">Peep what's been happening in the app</p>

  <DataTable :columns="columns" :rows="mappedLogs" keyField="requestId" emptyText="No logs" />
    </header>
  </div>
</template>

<style scoped> 
.link {
  text-decoration: none;
  color: inherit;
}
</style>