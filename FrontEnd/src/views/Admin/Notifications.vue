<script setup lang="ts">
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';

import { ref, computed } from 'vue';
import { useI18n } from 'vue-i18n';

import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';

import type { ISystemNotificationService } from '@/service/IService/ISystemNotificationService';
import type { SystemNotificationBroadcastDto, SystemNotificationDto } from '@/model/dto/SystemNotificationDto';
import { label } from 'three/tsl';

const { t } = useI18n();

const notificationService = container.get<ISystemNotificationService>(TYPES.systemNotificationService);

const broadcast = ref(false);

const toggleBroadcast = (event: any) => {
    broadcast.value = event.target.checked;
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

        <FormField
            :enabled="!broadcast"
            input-id="notif-email"
            class="field"
            :required="true"
            :name="t('admin.notifications.fields.email.title') + '*'"
            :placeholder-text="t('admin.notifications.fields.email.placeholder')"
            v-model="form.targetUserEmail"
            placeholder="user@example.com"
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
                v-model="form.urgency"
                @sl-change="(event) => form.urgency = Number(event.target.value)"
                placeholder="Select urgency level"
            >
            <sl-option value="0">Normal</sl-option>
            <sl-option value="1">Urgent</sl-option>
          </sl-select>
          
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
