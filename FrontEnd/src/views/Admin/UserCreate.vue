<script setup lang="ts">
import { reactive } from 'vue';
import { useRouter, RouterLink } from 'vue-router';
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import AxiosHttpService from '@/service/AxiosHttpService';
import { AdminService } from '@/service/AdminService';
import type { SystemUser } from '@/model/SystemUser';

const http = new AxiosHttpService();
const adminService = new AdminService(http as any);

// Role items must match backend enum ordering: Administrator=0, PortAuthorityOfficer=1, SAORepresentative=2, LogisticsOperator=3
const roleItems = [
  { id: 0, name: 'Administrator' },
  { id: 1, name: 'Port Authority Officer' },
  { id: 2, name: 'SAO Representative' },
  { id: 3, name: 'Logistics Operator' }
];

const model = reactive<SystemUser>({
  sub: '',
  emailAddress: '',
  isActive: false,
  role: 0
});

const submitFunction = async (obj: SystemUser) => {
  const roleNum = typeof obj.role === 'number' ? obj.role : Number(obj.role);
  return await adminService.inviteUser(obj.emailAddress, roleNum);
}

</script>

<template>
  <div>
    <sl-breadcrumb>
      <sl-breadcrumb-item><RouterLink to="/admin" class="breadcrumb-link">Admin</RouterLink></sl-breadcrumb-item>
      <sl-breadcrumb-item><RouterLink to="/admin/users" class="breadcrumb-link">Users</RouterLink></sl-breadcrumb-item>
      <sl-breadcrumb-item>Create User</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
      <h1 class="title">Create User</h1>
      <p class="subtitle">Register a new system user</p>

      <EntityForm :object="model" :submitFunction="submitFunction" successMessage="User created successfully">
        <FormField name="Email" v-model="model.emailAddress" placeholderText="example@mail.com" required />

        <EntityDropdown
          name="Role"
          :items="roleItems"
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
