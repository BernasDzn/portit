<script setup lang="ts">
import ListingBox from '@/components/crud/ListingBox.vue';
import { useRouter } from 'vue-router';
import type { Filter, Page } from '@/model/Page';
import type { Vessel } from '@/model/Vessel';
import AxiosHttpService from '@/service/AxiosHttpService';
import { AdminService } from '@/service/AdminService';
import type { SystemUser } from '@/model/SystemUser';
import SystemUserPrinter from '@/components/printers/SystemUserPrinter.vue';

const http = new AxiosHttpService()
const adminService = new AdminService(http as any)

const fetchUsers = async (filter?: Filter<SystemUser>): Promise<Page<SystemUser>> => {
  // ListingBox may call fetch with no args; ensure we supply a default filter shape
  const effectiveFilter: Filter<SystemUser> = filter ?? { filter: {}, pageNumber: 1, pageSize: 10 };
  return await adminService.filterSystemUsers(effectiveFilter);
}
</script>

<template>
  <div>

    <sl-breadcrumb>
      <sl-breadcrumb-item><RouterLink to="/admin/dashboard" class="breadcrumb-link">Admin</RouterLink></sl-breadcrumb-item>
      <sl-breadcrumb-item><RouterLink to="/admin/users" class="breadcrumb-link">Users</RouterLink></sl-breadcrumb-item>
      <sl-breadcrumb-item>Search Users</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
      <h1 class="title">Search Users</h1>
      <p class="subtitle">Search registered system users</p>

      <ListingBox listingStyle="listing-grid" :fetch-function="fetchUsers" search-filter="email" v-slot="{elements}">
        <li v-for="systemUser in elements" :key="(systemUser.emailAddress ?? systemUser.email)">
          <SystemUserPrinter class="listing-box" :systemUser="systemUser" :link="`/admin/users/view/${(systemUser.emailAddress ?? systemUser.email)}`" />
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