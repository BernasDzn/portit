<script setup lang="ts">
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';

import { ref } from 'vue';
import { useI18n } from 'vue-i18n';

import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';

import type { ISystemNotificationService } from '@/service/IService/ISystemNotificationService';
import type { SystemNotificationBroadcastDto, SystemNotificationDto } from '@/model/dto/SystemNotificationDto';
import type { IAdminService } from '@/service/IService/IAdminService';
import { useSession } from '@/composables/session';
import EntityDropdown from '@/components/crud/EntityDropdown.vue';

const { t } = useI18n();

const notificationService = container.get<ISystemNotificationService>(TYPES.systemNotificationService);
const adminService = container.get<IAdminService>(TYPES.adminService);
const user = useSession().authenticatedUser;

const broadcast = ref(false);

const toggleBroadcast = (event: any) => {
    broadcast.value = event.target.checked;
};

async function fetchUsers() {
    try {
        const allUsers = await adminService.getAllUsers();
        return allUsers.filter(u => u.email !== user?.email).map(u => u.email);
        
    } catch (err) {
        console.error('Failed to fetch users for notification dropdown', err);
        return [];
    }
};

const toggleSendEmail = (event: any) => {
    form.value.shouldSendEmail = event.target.checked;
};

const form = ref<SystemNotificationDto>({
    urgency: 0,
    shouldSendEmail: false,
    title: "",
    message: "",
    targetUserEmail: ""
});

const submitFn = async (obj: any) => {

    console.log("Submitting notification:", obj, "Broadcast:", broadcast.value);
    if (broadcast.value) {
        const payload: SystemNotificationBroadcastDto = {
            urgency: obj.urgency,
            shouldSendEmail: obj.shouldSendEmail,
            title: obj.title,
            message: obj.message
        };
        return notificationService.broadcastNotification(payload);
    }

    // single user
    return notificationService.notifyUser(obj);
};
</script>

<template>
  <div>
    <sl-breadcrumb>
      <sl-breadcrumb-item>
        <RouterLink to="/admin/dashboard" class="breadcrumb-link">
          {{ t('admin.sidebarTitle') }}
        </RouterLink>
      </sl-breadcrumb-item>

      <sl-breadcrumb-item>
        {{ t('admin.notifications.create') }}
      </sl-breadcrumb-item>
    </sl-breadcrumb>

    <h1 class="title">{{ t('admin.notifications.create') }}</h1>
    <p class="subtitle">{{ t('admin.notifications.subtitleCreate') }}</p>

    <EntityForm
        :object="form"
        :submit-function="submitFn"
        submit-text="Send Notification"
    >
        <div class="field" style="margin-bottom:1rem;">
            <label class="switch-label">
                <sl-switch :checked="broadcast" @sl-change="toggleBroadcast"></sl-switch>
                <span class="switch-text">
                    {{ t('admin.notifications.broadcastToggle') }}
                </span>
            </label>
        </div>

        <EntityDropdown
          v-if="!broadcast"
          :name="t('admin.notifications.fields.email.title') + '*'"
          v-model=form.targetUserEmail
          :fetchFunction="fetchUsers"
          :fetchOnMount="true"
          valueKey="email"
          labelKey="email"
          :placeholderText="t('admin.notifications.fields.email.placeholder')"
          required
        />

        <FormField
            input-id="notif-title"
            class="field"
            :required="true"
            :name="t('admin.notifications.fields.title.title') + '*'"
            :placeholder-text="t('admin.notifications.fields.title.placeholder')"
            v-model="form.title"
        />

        <FormField
            input-id="notif-message"
            textarea
            class="field"
            :required="true"
            :name="t('admin.notifications.fields.message.title') + '*'"
            :placeholder-text="t('admin.notifications.fields.message.placeholder')"
            v-model="form.message"
        />

        <div class="field">
            <label class="form-label">{{ t('admin.notifications.fields.urgency.title') }}</label>
            <sl-select
                :value="form.urgency.toString()"
                @sl-change="(event) => form.urgency = Number(event.target.value)"
                placeholder="Select urgency level"
                style="margin-top: 7px;"
            >
            <sl-option value="0">Normal</sl-option>
            <sl-option value="1">Urgent</sl-option>
          </sl-select>
          
        </div>

        <div class="field" style="margin-bottom:1rem;">
            <label class="switch-label">
                <sl-switch :checked="form.shouldSendEmail" @sl-change="toggleSendEmail"></sl-switch>
                <span class="switch-text">
                    {{ t('admin.notifications.sendEmailToggle') }}
                </span>
            </label>
        </div>
    </EntityForm>
  </div>
</template>

<style scoped>
.switch-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}
.switch-text {
  font-size: 0.9rem;
}
</style>
