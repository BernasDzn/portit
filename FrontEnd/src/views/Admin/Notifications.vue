<script setup lang="ts">
import EntityForm from '@/components/crud/EntityForm.vue';
import FormField from '@/components/crud/FormField.vue';

import { ref, computed } from 'vue';
import { useI18n } from 'vue-i18n';

import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';

import type { ISystemNotificationService } from '@/service/IService/ISystemNotificationService';
import type { SystemNotificationDto } from '@/model/dto/SystemNotificationDto';

const { t } = useI18n();

const notificationService =
    container.get<ISystemNotificationService>(TYPES.systemNotificationService);

// ------------------
// FORM STATE
// ------------------
const form = ref<SystemNotificationDto>({
    urgency: 0,
    shouldSendEmail: false,
    title: "",
    message: "",
    targetUserEmail: ""
});

// Broadcast toggle
const isBroadcast = ref(false);

// Which submit function to call
const submitFn = async (obj: any) => {
    if (isBroadcast.value) {
        // broadcast: remove target email
        const payload = {
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
                <sl-switch v-model="isBroadcast"></sl-switch>
                <span class="switch-text">
                    {{ t('admin.notifications.broadcastToggle') }}
                </span>
            </label>
        </div>

        <FormField
            v-if="!isBroadcast"
            input-id="notif-email"
            class="field"
            :required="true"
            :name="t('admin.notifications.fields.email') + '*'"
            v-model="form.targetUserEmail"
            placeholder="user@example.com"
        />

        <FormField
            input-id="notif-title"
            class="field"
            :required="true"
            :name="t('admin.notifications.fields.title') + '*'"
            v-model="form.title"
        />

        <FormField
            input-id="notif-message"
            textarea
            class="field"
            :required="true"
            :name="t('admin.notifications.fields.message') + '*'"
            v-model="form.message"
        />

        <div class="field">
            <label class="form-label">{{ t('admin.notifications.fields.urgency') }}</label>
            <sl-select v-model="form.urgency">
                <sl-option :value="0">{{ t('admin.notifications.urgency.normal') }}</sl-option>
                <sl-option :value="1">{{ t('admin.notifications.urgency.urgent') }}</sl-option>
            </sl-select>
        </div>

        <div class="field">
            <label class="switch-label">
                <sl-switch v-model="form.shouldSendEmail"></sl-switch>
                <span class="switch-text">
                    {{ t('admin.notifications.fields.sendEmail') }}
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
