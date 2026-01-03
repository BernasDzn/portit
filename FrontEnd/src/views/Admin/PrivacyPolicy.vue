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
const previousVersions = ref<PrivacyPolicy[]>([]);
const showVersionHistory = ref<boolean>(false);

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

const loadVersionHistory = async () => {
    try {
        const allVersions = await privacyPolicyService.getAllPrivacyPolicies();
        previousVersions.value = allVersions.filter(p => !p.active).sort((a, b) => 
            new Date(b.updatedOn).getTime() - new Date(a.updatedOn).getTime()
        );
        showVersionHistory.value = true;
    } catch (error) {
        notifications.enqueueNotification(
            'Failed to load version history',
            notifications.notificationTypes.DANGER
        );
    }
};

const viewVersion = (version: PrivacyPolicy) => {
    markdownContent.value = version.content;
    showVersionHistory.value = false;
};

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

        <div class="button-group mt-3">
            <sl-button
                variant="primary"
                @click="updatePolicy"
            >
                Update Privacy Policy
            </sl-button>

            <sl-button
                variant="default"
                @click="loadVersionHistory"
            >
                View Previous Versions
            </sl-button>
        </div>

        <sl-dialog 
            :open="showVersionHistory" 
            @sl-hide="showVersionHistory = false"
            label="Privacy Policy Version History"
        >
            <div v-if="previousVersions.length === 0" class="no-versions">
                No previous versions available.
            </div>
            <div v-else class="version-list">
                <div 
                    v-for="version in previousVersions" 
                    :key="version.id"
                    class="version-item"
                    @click="viewVersion(version)"
                >
                    <div class="version-info">
                        <strong>Updated:</strong> {{ new Date(version.updatedOn).toLocaleString() }}
                    </div>
                    <sl-button size="small" variant="text">View</sl-button>
                </div>
            </div>
        </sl-dialog>

    </div>
</template>

<style scoped>
.button-group {
    display: flex;
    gap: 1rem;
}

.version-list {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.version-item {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1rem;
    border: 1px solid var(--sl-color-neutral-200);
    border-radius: 4px;
    cursor: pointer;
    transition: background-color 0.2s;
}

.version-item:hover {
    background-color: var(--sl-color-neutral-50);
}

.version-info {
    display: flex;
    flex-direction: column;
}

.no-versions {
    padding: 2rem;
    text-align: center;
    color: var(--sl-color-neutral-500);
}
</style>