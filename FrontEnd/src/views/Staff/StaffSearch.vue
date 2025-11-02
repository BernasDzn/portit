<script setup lang="ts">
import AxiosHttpService from '@/service/AxiosHttpService'

import ListingBox from '@/components/ListingBox.vue';
import StaffPrinter from '@/components/printers/StaffPrinter.vue'

import { StaffService } from '@/service/StaffService'
import type { Staff } from '@/model/Staff'
import type { Filter } from '@/model/Page'
import type { Page } from '@/model/Page'

const http = new AxiosHttpService()
const staffService = new StaffService(http as any)

const fetchStaffs = async (filtering?: Filter<Staff>): Promise<Page<Staff>> => {
	return await staffService.getStaffs(filtering);
}

</script>

<template>
  <div>

    <sl-breadcrumb>
      <sl-breadcrumb-item><RouterLink to="/staff/dashboard" class="breadcrumb-link">Staff Dashboard</RouterLink></sl-breadcrumb-item>
      <sl-breadcrumb-item>Search Staff</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
      <h1 class="title">Search Staff</h1>
      <p class="subtitle">Search registered staff</p>

      <ListingBox :fetch-function="fetchStaffs" search-filter="name" v-slot="{elements}">
        <li v-for="staff in elements" :key="staff.mechanographicNumber">
          <StaffPrinter class="listing-box" :staff="staff" :link="`/staff/view/${staff.mechanographicNumber}`" />
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