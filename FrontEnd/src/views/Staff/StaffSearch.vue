<script setup lang="ts">
import ListingBox from '@/components/crud/ListingBox.vue';
import StaffPrinter from '@/components/printers/StaffPrinter.vue'
import { StaffService } from '@/service/StaffService'
import type { Staff } from '@/model/Staff'
import type { Filter } from '@/model/Page'
import type { Page } from '@/model/Page'
import { useI18n } from 'vue-i18n';
import type { IStaffService } from '@/service/IService/IStaffService';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';

const staffService = container.get<IStaffService>(TYPES.staffService);

const fetchStaffs = async (filtering?: Filter<Staff>): Promise<Page<Staff>> => {
  return await staffService.getStaffs(filtering);
}

const { t } = useI18n();


const filterDefinition = {
    mechanographicNumber: {
        type: 'text',
        label: t('staff.fields.mechanographicNumber.title'),
    },
    email: {
        type: 'text',
        label: t('staff.fields.email.title'),
    },
    status: {
        type: 'select',
        label: t('staff.fields.status.title'),
        options: [
            { value: '0', text: t('staff.fields.status.options.available') },
            { value: '1', text: t('staff.fields.status.options.unavailable') },
            { value: '2', text: t('staff.fields.status.options.temporarilyReassigned') },
        ],
    },
    phoneNumber: {
        type: 'text',
        label: t('staff.fields.phoneNumber.title'),
    },
};

</script>

<template>
  <div>

    <sl-breadcrumb>
      <sl-breadcrumb-item><RouterLink to="/staff/dashboard" class="breadcrumb-link">{{ t('staff.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
      <sl-breadcrumb-item>{{ t('staff.tabs.search') }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
      <h1 class="title">{{ t('staff.tabs.search') }}</h1>
      <p class="subtitle">{{ t('staff.subtitle.search') }}</p>

      <ListingBox :fetch-function="fetchStaffs" search-filter="name" v-slot="{elements}" :filter-definition="filterDefinition">
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