<script setup lang="ts">
import { reactive } from 'vue';
import { useRouter, RouterLink } from 'vue-router';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import AxiosHttpService from '@/service/AxiosHttpService';
import { AdminService } from '@/service/AdminService';
import { RepresentativeService } from '@/service/RepresentativeService';
import type { SystemUser } from '@/model/SystemUser';
import { useI18n } from 'vue-i18n';
import { useAlerts } from '@/composables/alerts';

const { t } = useI18n();
const alerts = useAlerts();

const http = new AxiosHttpService();
const adminService = new AdminService(http as any);

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

const submitFunction = async (obj: SystemUser) => {
  const roleNum = typeof obj.role === 'number' ? obj.role : Number(obj.role);

  if (roleNum == 2) {
    const representativeService = new RepresentativeService(http as any);

    try {
      await representativeService.getByEmail(obj.emailAddress);
    } catch {
      throw new Error('Representative not found for the provided email address.');
    }
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
        <FormField :name="t('user.fields.email.title')" v-model="model.emailAddress" :placeholderText="t('user.fields.email.placeholder')" required />

        <EntityDropdown
          :name="t('user.fields.role.title')"
          :items="roleItems.map(r => ({ id: r.id, name: t(r.name) }))"
          v-model="model.role"
          valueKey="id"
          labelKey="name"
          required
        />

      </EntityForm>
    </header>
  </div>
</template>

<style scoped>
.link { text-decoration: none; color: inherit; }
</style>
