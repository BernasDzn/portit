<script setup lang="ts">
import { useAlerts } from '@/composables/alerts';
import { computed, ref } from 'vue';
import { useRouter } from 'vue-router';

const notification = useAlerts();

const props = defineProps({
    editingObject: {
        type: Object,
        required: true
    },
    submitFunction: {
        type: Function,
        required: true
    },
    successMessage: {
        type: String,
        default: 'Form submitted successfully!',
        required: false
    }
});

const router = useRouter();
const cancelDialog = ref<HTMLElement | null>(null);

const form = ref<HTMLFormElement | null>(null);

const hasUnsaved = computed(() => {
    return Object.values(props.editingObject).some(value => {
        return value !== null && value !== undefined && value !== '';
    });
});

const submit = async () => {
    if (form.value && !form.value.checkValidity()) {
        form.value.reportValidity();
        return;
    }
    
    props.submitFunction(props.editingObject)
    .catch((error: any) => {
        notification.enqueueNotification(
            error.response?.data || 'Could not pinpoint the error. Please try again later.',
            notification.notificationTypes.DANGER,
        );

        return Promise.reject(error);
    })
    .then(() => {
        notification.enqueueNotification(
            props.successMessage,
            notification.notificationTypes.SUCCESS,
        );

        router.back();
    });
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

</script>

<template>
    <form ref="form" @submit.prevent="submit">
        <div class="form-content">
            <slot class="form-content"></slot>
        </div>

        <div class="form-operations">
            <sl-button class="form-button" variant="danger" outline @click="onCancel">
                Cancel
            </sl-button>
            <sl-button class="form-button" variant="primary" type="submit">
                Create
            </sl-button>
        </div>

        <sl-dialog ref="cancelDialog" label="Discard changes?" >
            <div>You have unsaved changes. Do you really want to discard them and leave?</div>
            <sl-button slot="footer" variant="text" @click="(cancelDialog as any).hide()">Keep editing</sl-button>
            <sl-button slot="footer" variant="danger" @click="confirmCancel">Discard</sl-button>
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