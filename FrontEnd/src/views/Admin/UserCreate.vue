<script setup lang="ts">
import { reactive, computed } from 'vue';
import { useRouter, RouterLink } from 'vue-router';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import type { SystemUser } from '@/model/SystemUser';
import { useI18n } from 'vue-i18n';
import { useAlerts } from '@/composables/alerts';
import { container } from '@/inversify.config';
import type { IAdminService } from '@/service/IService/IAdminService';
import TYPES from '@/inversify/types';
import type { IRepresentativeService } from '@/service/IService/IRepresentativeService';

const { t } = useI18n();
const alerts = useAlerts();

const adminService = container.get<IAdminService>(TYPES.adminService);
const representativeService = container.get<IRepresentativeService>(TYPES.representativeService);

// Role items must match backend enum ordering: Administrator=0, PortAuthorityOfficer=1, SAORepresentative=2, LogisticsOperator=3
const roleItems = [
  { id: 0, name: 'user.fields.role.options.admin' },
  { id: 1, name: 'user.fields.role.options.pao' },
  { id: 2, name: 'user.fields.role.options.saoRep' },
  { id: 3, name: 'user.fields.role.options.logisticsOperator' }
];

const model = reactive<SystemUser>({
  sub: '',
  emailAddress: '',
  isActive: false,
  role: 0
});

const showRepresentativeDropdown = computed(() => {
  const roleNum = typeof model.role === 'number' ? model.role : Number(model.role);
  return roleNum === 2;
});

const submitFunction = async (obj: SystemUser) => {
  const roleNum = typeof obj.role === 'number' ? obj.role : Number(obj.role);

  if (roleNum == 2 && !obj.emailAddress) {
    throw new Error('Please select a representative.');
  }

  return await adminService.inviteUser(obj.emailAddress, roleNum);
}

</script>

<template>
  <div>
    <sl-breadcrumb>
      <sl-breadcrumb-item><RouterLink to="/admin/dashboard" class="breadcrumb-link">{{ t('admin.sidebarTitle') }}</RouterLink></sl-breadcrumb-item>
      <sl-breadcrumb-item><RouterLink to="/admin/users" class="breadcrumb-link">{{ t('user.title') }}</RouterLink></sl-breadcrumb-item>
      <sl-breadcrumb-item>{{ t('user.tabs.create') }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
      <h1 class="title">{{ t('user.tabs.create') }}</h1>
      <p class="subtitle">{{ t('user.subtitle.create') }}</p>

      <EntityForm :object="model" :submitFunction="submitFunction" successMessage="User created successfully">
        
        <EntityDropdown
          :name="t('user.fields.role.title')"
          :items="roleItems.map(r => ({ id: r.id, name: t(r.name) }))"
          v-model="model.role"
          valueKey="id"
          labelKey="name"
          required
        />

        <EntityDropdown
          v-if="showRepresentativeDropdown"
          :name="t('representative.title')"
          v-model="model.emailAddress"
          :fetchFunction="() => representativeService.getAll()"
          :fetchOnMount="true"
          valueKey="emailAddress"
          labelKey="emailAddress"
          :placeholderText="t('representative.select')"
          required
        />

        <FormField 
          v-else
          :name="t('user.fields.email.title')" 
          v-model="model.emailAddress" 
          :placeholderText="t('user.fields.email.placeholder')" 
          required 
        />

      </EntityForm>
    </header>
  </div>
</template>

<style scoped>
.link { text-decoration: none; color: inherit; }
</style>
