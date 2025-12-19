<script setup lang="ts">
import { useAlerts } from '@/composables/alerts';
import { computed, onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import Loading from '../Loading.vue';
import { useI18n } from 'vue-i18n';

const {t} = useI18n();
const notification = useAlerts();

const props = defineProps({
    object: {
        type: Object,
        required: true
    },
    editingId: {
        type: String,
        required: false,
        default: null
    },
    submitFunction: {
        type: Function,
        required: true
    },
    fetchingFunction: {
        type: Function,
        required: false,
        default: null
    },
    successMessage: {
        type: String,
        default: 'Form submitted successfully!',
        required: false
    },
    hideButtons: {
        type: Boolean,
        default: false,
        required: false
    }
});

const router = useRouter();
const cancelDialog = ref<HTMLElement | null>(null);
const loading = ref(false);
const buttonLoading = ref(false);

const form = ref<HTMLFormElement | null>(null);

const hasUnsaved = computed(() => {
    const obj = props.object;

    for (const k in obj) {
        const value = obj[k];

        if (Array.isArray(value)) {
            if (value.length > 0) return true;
        } else if (value && typeof value === 'object') {
            for (const v of Object.values(value)) {
                if (v != null && v !== '') return true;
            }
        } else {
            if (value != null && value !== '') return true;
        }
    }

    return false;
});

const submit = async () => {
    if (form.value && !form.value.checkValidity()) {
        form.value.reportValidity();
        return;
    }

    buttonLoading.value = true;
    
    try {
    
        const res = await props.submitFunction(props.object)
        notification.enqueueNotification(
            props.successMessage,
            notification.notificationTypes.SUCCESS,
        );
        router.back();
        
    } catch (error) {

        let message = (error as any)?.response?.data;
        if (message && typeof message === 'object') {
            // Handle common error shapes from different backends
            if (Array.isArray(message.errors)) {
                message = message.errors[0].error;
            } else if (message.error) {
                message = message.error;
            } else if (message.message) {
                message = message.message;
            } else {
                try {
                    message = JSON.stringify(message);
                } catch (e) {
                    message = String(message);
                }
            }
        }

        if ((error as any)?.response?.status === 400) {

            // Validation error from server
            notification.enqueueNotification(
                'Validation error: ' + message || 'Please check your input.',
                notification.notificationTypes.DANGER,
            );
            return;
        }
        
        // Include reponse message in notification if available
        notification.enqueueNotification(
            'Failed to submit form: ' + (message || (error as Error).message),
            notification.notificationTypes.DANGER,
        );
        
    } finally {
        buttonLoading.value = false;
    }
}

const onCancel = () => {
    if (hasUnsaved.value) {
        // show confirmation dialog
        (cancelDialog.value as any)?.show?.();
    } else {
        // no unsaved changes, navigate back
        router.back();
    }
}

function confirmCancel() {
    (cancelDialog.value as any)?.hide?.();
    router.back();
}

onMounted(async () => {

    if (props.editingId != null && props.fetchingFunction != null) {
        try {
            loading.value = true;

            const data = await props.fetchingFunction(props.editingId);
            Object.assign(props.object, data);

            loading.value = false;

        } catch (err) {
            notification.enqueueNotification(
                'Failed to load data for editing.',
                notification.notificationTypes.DANGER,
            );
        }
    }
});

</script>

<template>
    <form ref="form" @submit.prevent="submit">
        <div class="form-content">
            <Loading v-if="loading" />
            <slot v-else class="form-content"></slot>
        </div>

        <div v-if="!props.hideButtons" class="form-operations">
            <sl-button class="form-button" variant="danger" outline @click="onCancel">
                {{ t('buttons.cancel') }}
            </sl-button>
            <sl-button class="form-button" variant="primary" type="submit" :loading="buttonLoading" :disabled="buttonLoading">
                {{ props.editingId != null ? t('buttons.save') : t('buttons.create') }}
            </sl-button>
        </div>        

        <sl-dialog ref="cancelDialog" :label="t('unsavedChanges.title')">
            <div>{{ t('unsavedChanges.message') }}</div>
            <sl-button slot="footer" variant="text" @click="(cancelDialog as any).hide()">{{ t('unsavedChanges.cancel') }} </sl-button>
            <sl-button slot="footer" variant="danger" @click="confirmCancel">{{ t('unsavedChanges.confirm') }}</sl-button>
        </sl-dialog>
    </form>
</template>

<style scoped>
.form-content {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    margin-bottom: 1rem;
}

.form-operations {
    display: flex;
    justify-content: left;
    gap: 1rem;
}
</style>