<script setup lang="ts">
import ListingBox from '@/components/crud/ListingBox.vue';
import { useRouter } from 'vue-router';
import type { Filter, Page } from '@/model/Page';
import type { Vessel } from '@/model/Vessel';
import AxiosHttpService from '@/service/AxiosHttpService';
import { AdminService } from '@/service/AdminService';
import type { SystemUser } from '@/model/SystemUser';
import SystemUserPrinter from '@/components/printers/SystemUserPrinter.vue';
import { useI18n } from 'vue-i18n';
import type { Logs } from '@/model/Logs';
import LogPrinter from '@/components/printers/LogPrinter.vue';

const { t } = useI18n();

const http = new AxiosHttpService()
const adminService = new AdminService(http)

const fetchLogs = async (filter?: Filter<any>): Promise<Page<Logs>> => {
    const logList: Logs[] = await adminService.getLogs();
    return {
        items: logList,
        pageNumber: 1,
        pageSize: logList.length,
        pageCount: 1
    };
}
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

      <ListingBox listingStyle="listing-grid" :fetch-function="fetchLogs" v-slot="{elements}">
        <li v-for="log in elements" :key="log.requestId">
          <LogPrinter :log="log" />
        </li>
      </ListingBox>
    </header>
  </div>
</template>

<style scoped> 
.link {
  text-decoration: none;
  color: inherit;
}
</style>