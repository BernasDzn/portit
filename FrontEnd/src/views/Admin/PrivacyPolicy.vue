<script setup lang="ts">
import MarkdownEditor from '@/components/MarkdownEditor.vue';
import { useAlerts } from '@/composables/alerts';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import { PrivacyPolicy } from '@/model/PrivacyPolicy';
import type { IPrivacyPolicyService } from '@/service/IService/IPrivacyPolicyService';
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';

const { t } = useI18n();

const privacyPolicyService = container.get<IPrivacyPolicyService>(TYPES.privacyPolicyService);

const notifications = useAlerts();
const markdownContent = ref<string>('# Hello world!');
const router = useRouter();

onMounted(async () => {
    try {
        markdownContent.value = (await privacyPolicyService.getActivePrivacyPolicy()).content;
    } catch (error) {
        notifications.enqueueNotification(
            'No privacy policy to load',
            notifications.notificationTypes.WARNING
        );
    }
});

const updatePolicy = async () => {
    try {
        await privacyPolicyService.updatePrivacyPolicy(new PrivacyPolicy({
            content: markdownContent.value,
            updatedOn: new Date(),
            active: true
        }));
        notifications.enqueueNotification(
            'Privacy policy updated successfully',
            notifications.notificationTypes.SUCCESS
        );

        router.back();

    } catch (error) {
        notifications.enqueueNotification(
            'Failed to update privacy policy',
            notifications.notificationTypes.DANGER
        );
    }
};

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item><RouterLink to="/admin/dashboard" class="breadcrumb-link">{{ t('admin.sidebarTitle') }}</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>Privacy Policy</sl-breadcrumb-item>
        </sl-breadcrumb>

        <h1 class="title">Privacy Policy</h1>
        <p class="subtitle"> Update the software's privacy policy. </p>

        <MarkdownEditor 
            v-model="markdownContent" 
            height="500px"
        />

        <sl-button
            variant="primary"
            class="mt-3"
            @click="updatePolicy"
        >
            Update Privacy Policy
        </sl-button>

    </div>
</template>