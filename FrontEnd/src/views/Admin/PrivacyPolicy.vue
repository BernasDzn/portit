<script setup lang="ts">
import MarkdownEditor from '@/components/MarkdownEditor.vue';
import DiffViewer from '@/components/DiffViewer.vue';
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
const allVersions = ref<PrivacyPolicy[]>([]);
const showVersionHistory = ref<boolean>(false);
const showDiffDialog = ref<boolean>(false);
const currentVersion = ref<PrivacyPolicy | null>(null);
const olderVersion = ref<PrivacyPolicy | null>(null);
const newerVersion = ref<PrivacyPolicy | null>(null);

onMounted(async () => {
    try {
        currentVersion.value = await privacyPolicyService.getActivePrivacyPolicy();
        markdownContent.value = currentVersion.value.content;
    } catch (error) {
        notifications.enqueueNotification(
            'No privacy policy to load',
            notifications.notificationTypes.WARNING
        );
    }
});

const loadVersionHistory = async () => {
    try {
        const versions = await privacyPolicyService.getAllPrivacyPolicies();
        // Sort by date descending (newest first)
        allVersions.value = versions.sort((a, b) => 
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

const compareVersion = (version: PrivacyPolicy, index: number) => {
    // Compare with the previous version (next in the array since sorted newest first)
    if (index < allVersions.value.length - 1) {
        newerVersion.value = version;
        olderVersion.value = allVersions.value[index + 1];
        showVersionHistory.value = false;
        showDiffDialog.value = true;
    } else {
        notifications.enqueueNotification(
            'No previous version to compare with',
            notifications.notificationTypes.WARNING
        );
    }
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
            <div v-if="allVersions.length === 0" class="no-versions">
                No versions available.
            </div>
            <div v-else class="version-list">
                <div 
                    v-for="(version, index) in allVersions" 
                    :key="version.id"
                    class="version-item"
                    :class="{ 'active-version': version.active }"
                >
                    <div class="version-info">
                        <div class="version-header">
                            <strong>Updated:</strong> {{ new Date(version.updatedOn).toLocaleString() }}
                            <sl-badge v-if="version.active" variant="success" pill>Active</sl-badge>
                        </div>
                    </div>
                    <div class="version-actions">
                        <sl-button size="small" variant="text" @click="viewVersion(version)">View</sl-button>
                        <sl-button 
                            size="small" 
                            variant="default" 
                            @click="compareVersion(version, index)"
                            :disabled="index === allVersions.length - 1"
                        >
                            Compare
                        </sl-button>
                    </div>
                </div>
            </div>
        </sl-dialog>

        <sl-dialog 
            :open="showDiffDialog" 
            @sl-hide="showDiffDialog = false"
            label="Privacy Policy Changes"
            style="--width: 90vw;"
        >
            <DiffViewer 
                v-if="olderVersion && newerVersion"
                :old-text="olderVersion.content"
                :new-text="newerVersion.content"
                :old-label="`Previous Version (${new Date(olderVersion.updatedOn).toLocaleString()})`"
                :new-label="`Newer Version (${new Date(newerVersion.updatedOn).toLocaleString()})${newerVersion.active ? ' - Active' : ''}`"
            />
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
    transition: background-color 0.2s;
}

.version-item.active-version {
    border-color: var(--sl-color-success-500);
    border-width: 2px;
    background-color: var(--sl-color-success-50);
}

.version-item:hover {
    background-color: var(--sl-color-neutral-50);
}

.version-item.active-version:hover {
    background-color: var(--sl-color-success-100);
}

.version-info {
    display: flex;
    flex-direction: column;
}

.version-header {
    display: flex;
    align-items: center;
    gap: 0.75rem;
}

.version-actions {
    display: flex;
    gap: 0.5rem;
}

.no-versions {
    padding: 2rem;
    text-align: center;
    color: var(--sl-color-neutral-500);
}
</style>