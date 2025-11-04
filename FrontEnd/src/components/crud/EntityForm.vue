<script setup lang="ts">
import { useAlerts } from '@/composables/alerts';
import { computed, onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import Loading from '../Loading.vue';

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
    }
});

const router = useRouter();
const cancelDialog = ref<HTMLElement | null>(null);
const loading = ref(false);

const form = ref<HTMLFormElement | null>(null);

const hasUnsaved = computed(() => {
    console.log(props.object);
    return Object.values(props.object).some(value => {
        if (typeof value === 'object' && value !== null) 
            return Object.values(value).some(v => {return (v !== null && v !== undefined && v !== '');});
        else if (Array.isArray(value))
            return value.length > 0;
        else 
            return (value !== null && value !== undefined && value !== '');
        
    });
});

const submit = async () => {
    if (form.value && !form.value.checkValidity()) {
        form.value.reportValidity();
        return;
    }

    props.submitFunction(props.object)
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

onMounted(
    async () => {
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
    }
);

</script>

<template>
    <form ref="form" @submit.prevent="submit">
        <div class="form-content">
            <Loading v-if="loading" />
            <slot v-else class="form-content"></slot>
        </div>

        <div class="form-operations">
            <sl-button class="form-button" variant="danger" outline @click="onCancel">
                Cancel
            </sl-button>
            <sl-button class="form-button" variant="primary" type="submit" :loading="loading">
                {{ props.editingId != null ? 'Save changes' : 'Create'}}
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