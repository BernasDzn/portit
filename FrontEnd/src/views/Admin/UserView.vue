<script setup lang="ts">
import { reactive, ref, onMounted } from 'vue';
import { useRoute, useRouter, RouterLink } from 'vue-router';
import FormField from '@/components/crud/FormField.vue';
import EntityDropdown from '@/components/crud/EntityDropdown.vue';
import Loading from '@/components/Loading.vue';
import { useAlerts } from '@/composables/alerts';
import type { SystemUser } from '@/model/SystemUser';
import { useI18n } from 'vue-i18n';
import type { IAdminService } from '@/service/IService/IAdminService';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';

const { t } = useI18n();

const adminService = container.get<IAdminService>(TYPES.adminService);
const notifications = useAlerts();

const route = useRoute();
const router = useRouter();
const emailParam = route.fullPath.split('/').pop() || '';

const loading = ref(true);
const deleteDialog = ref<HTMLElement | null>(null);

// Local mutable user object so FormField/EntityDropdown can bind with v-model
const user = reactive<SystemUser>({ sub: '', emailAddress: '', isActive: false, role: 0 });

// Role items for EntityDropdown (match backend enum ordering)
const roleItems = [
  { id: 0, name: 'user.fields.role.options.admin' },
  { id: 1, name: 'user.fields.role.options.pao' },
  { id: 2, name: 'user.fields.role.options.saoRep' },
  { id: 3, name: 'user.fields.role.options.logisticsOperator' }
];

onMounted(async () => {
  if (!emailParam) {
    loading.value = false;
    return;
  }

    try {
    loading.value = true;
    const data = await adminService.getByEmail(emailParam);
    // copy into reactive object
    Object.assign(user, data as any);
    // ensure emailAddress is populated whether backend returns `emailAddress` or `email`
    user.emailAddress = (data as any).emailAddress ?? (data as any).email ?? user.emailAddress;
  } catch (err: any) {
    notifications.enqueueNotification(err?.response?.data || 'Failed to load user', notifications.notificationTypes.DANGER);
  } finally {
    loading.value = false;
  }
});

const activate = async () => {
  try {
    await adminService.activateUserAccount(user.emailAddress);
    user.isActive = true;
    notifications.enqueueNotification('User activated', notifications.notificationTypes.SUCCESS);
  } catch (err: any) {
    notifications.enqueueNotification(err?.response?.data || 'Failed to activate user', notifications.notificationTypes.DANGER);
  }
};

const deactivate = async () => {
  try {
    await adminService.deactivateUserAccount(user.emailAddress);
    user.isActive = false;
    notifications.enqueueNotification('User deactivated', notifications.notificationTypes.SUCCESS);
  } catch (err: any) {
    notifications.enqueueNotification(err?.response?.data || 'Failed to deactivate user', notifications.notificationTypes.DANGER);
  }
};

const confirmDelete = () => {
  (deleteDialog.value as any)?.show?.();
};

const doDelete = async () => {
  try {
    await adminService.deleteUser(user.emailAddress);
    notifications.enqueueNotification('User deleted', notifications.notificationTypes.SUCCESS);
    router.back();
  } catch (err: any) {
    notifications.enqueueNotification(err?.response?.data || 'Failed to delete user', notifications.notificationTypes.DANGER);
  } finally {
    (deleteDialog.value as any)?.hide?.();
  }
};

</script>

<template>
  <div>
    <sl-breadcrumb>
      <sl-breadcrumb-item><RouterLink to="/admin" class="breadcrumb-link">{{ t('admin.sidebarTitle') }}</RouterLink></sl-breadcrumb-item>
      <sl-breadcrumb-item><RouterLink to="/admin/users" class="breadcrumb-link">{{ t('user.title') }}</RouterLink></sl-breadcrumb-item>
      <sl-breadcrumb-item><RouterLink to="/admin/users/search" class="breadcrumb-link">{{ t('user.tabs.search') }}</RouterLink></sl-breadcrumb-item>
      <sl-breadcrumb-item>{{user.emailAddress}}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <h1 class="title">{{ t('user.infoTitle') }}</h1>

    <sl-card style="margin-top: 1rem;">
      <div class="view-grid">
        <div class="fields">
          <Loading v-if="loading" />

          <div v-else>
            <FormField :name="t('user.fields.email.title')" v-model="user.emailAddress" :enabled="false" />

            <EntityDropdown
              :name="t('user.fields.role.title')"
              :items="roleItems.map(r => ({ id: r.id, name: t(r.name) }))"
              v-model="user.role"
              valueKey="id"
              labelKey="name"
              :enabled="false"
            />

            <p>{{ t('user.fields.isActive.title') }}: <strong>{{ user.isActive ? t('user.fields.isActive.options.active') : t('user.fields.isActive.options.inactive') }}</strong></p>
          </div>
        </div>

        <div class="actions">
          <sl-button v-if="!user.isActive" variant="primary" @click="activate">{{ t('buttons.activate') }}</sl-button>
          <sl-button v-else variant="warning" @click="deactivate">{{ t('buttons.deactivate') }}</sl-button>
          <sl-button variant="danger" outline @click="confirmDelete">{{ t('buttons.delete') }}</sl-button>
        </div>
      </div>
    </sl-card>

    <sl-dialog ref="deleteDialog" :label="t('user.confirmDelete')">
      <div>{{ t('user.confirmDeleteMessage') }}</div>
      <sl-button slot="footer" variant="text" @click="(deleteDialog as any).hide()">{{ t('buttons.cancel') }}</sl-button>
      <sl-button slot="footer" variant="danger" @click="doDelete">{{ t('buttons.delete') }}</sl-button>
    </sl-dialog>
  </div>
</template>

<style scoped>
.view-grid {
  display: flex;
  justify-content: space-between;
  gap: 1rem;
  align-items: stretch;
}
.fields {
  flex: 1 1 60%;
}
.actions {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  width: 180px;
  justify-content: flex-end;
}

</style>