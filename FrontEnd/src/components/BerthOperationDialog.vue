<script setup lang="ts">
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useAlerts } from '@/composables/alerts';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IVesselVisitExecutionService } from '@/service/IService/IVesselExecutionService';
import type { IDockService } from '@/service/IService/IDockService';
import EntityForm from './crud/EntityForm.vue';
import ObjectSelector from './crud/ObjectSelector.vue';
import FormField from './crud/FormField.vue';

const { t } = useI18n();
const notifications = useAlerts();
const vveService = container.get<IVesselVisitExecutionService>(TYPES.vesselVisitExecutionService);
const dockService = container.get<IDockService>(TYPES.dockService);

const dialogRef = ref<any>(null);
const formRef = ref<any>(null);

const operation = ref({
    dock: null,
    berthTime: null
});

const props = defineProps<{
    relatedVVN: string;
}>();

const emit = defineEmits<{
    berthUpdated: [];
}>();

const open = async () => {
    try {
        const now = new Date();
        operation.value.berthTime = now.toISOString().slice(0, 16);
        operation.value.dock = '';

        dialogRef.value?.show();
    } catch (error: any) {
        notifications.enqueueNotification(error.message || 'Failed to load docks', 'error');
    }
};

const close = () => {
    dialogRef.value?.hide();
    operation.value.dock = '';
    operation.value.berthTime = '';
};

const submitBerthOperation = async (formData: any) => {
    await vveService.updateBerthDetails(
        props.relatedVVN,
        formData.dock.code,
        formData.berthTime
    );

    console.log(formData.dock.code, formData.berthTime);

    emit('berthUpdated');
    close();
};


const handleSubmit = (event?: Event) => {
    // Find the native form element and trigger submit
    const formElement = formRef.value?.$el?.querySelector('form') || formRef.value?.$el;
    if (formElement && formElement.tagName === 'FORM') {
        formElement.requestSubmit();
    }
};

defineExpose({ open });

</script>

<template>
    <sl-dialog ref="dialogRef" :label="t('execution.berthOperation.title')" class="berth-dialog">
        <EntityForm ref="formRef" :object="operation" :submit-function="submitBerthOperation" :hide-buttons="true" :redirect="false">
            <div class="dialog-content">
                <p class="dialog-description">{{ t('execution.berthOperation.description') }}</p>

                <ObjectSelector :name="t('execution.berthOperation.dock') + ' *'" v-model="operation.dock"
                    :fetch-function="() => dockService.getDocks()" :fetch-on-mount="true"
                    :placeholderText="t('execution.berthOperation.dockPlaceholder')" labelKey="name" required />

                <FormField :name="t('execution.berthOperation.berthTime') + ' *'" v-model="operation.berthTime"
                    type="datetime-local" required />
            </div>
        </EntityForm>

        <div slot="footer" class="dialog-footer">
            <sl-button variant="text" @click="close">
                {{ t('buttons.cancel') }}
            </sl-button>
            <sl-button variant="primary" @click="handleSubmit">
                {{ t('buttons.save') }}
            </sl-button>
        </div>
    </sl-dialog>
</template>

<style scoped>
.berth-dialog::part(panel) {
    max-width: 500px;
}

.dialog-content {
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
    padding: 0.5rem 0;
}

.dialog-description {
    color: var(--sl-color-neutral-600);
    font-size: 0.95rem;
    margin: 0;
}

.form-field {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.form-field label {
    font-weight: 500;
    color: var(--sl-color-neutral-700);
    font-size: 0.9rem;
}

.note {
    display: flex;
    align-items: flex-start;
    gap: 0.5rem;
    padding: 0.75rem;
    background: var(--sl-color-primary-50);
    border-radius: var(--sl-border-radius-medium);
    color: var(--sl-color-primary-700);
    font-size: 0.875rem;
    margin: 0;
}

.note sl-icon {
    flex-shrink: 0;
    margin-top: 0.1rem;
}

.dialog-footer {
    display: flex;
    gap: 0.5rem;
    justify-content: flex-end;
}
</style>
