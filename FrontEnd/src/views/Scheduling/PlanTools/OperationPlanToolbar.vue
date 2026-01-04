<script setup lang="ts">
import { ref } from 'vue';

const emit = defineEmits<{
    shiftOperations: [minutes: number];
    optimizeSchedule: [];
    resetSchedule: [];
}>();

const shiftMinutes = ref<number>(30);
const showShiftDialog = ref(false);
const showOptimizeDialog = ref(false);
const showResetDialog = ref(false);
const shiftDirection = ref<'forward' | 'backward'>('forward');

const openShiftDialog = (direction: 'forward' | 'backward') => {
    shiftDirection.value = direction;
    showShiftDialog.value = true;
};

const handleShift = () => {
    const minutes = shiftDirection.value === 'forward' ? shiftMinutes.value : -shiftMinutes.value;
    emit('shiftOperations', minutes);
    showShiftDialog.value = false;
};

const handleOptimize = () => {
    showOptimizeDialog.value = true;
};

const confirmOptimize = () => {
    emit('optimizeSchedule');
    showOptimizeDialog.value = false;
};

const handleReset = () => {
    showResetDialog.value = true;
};

const confirmReset = () => {
    emit('resetSchedule');
    showResetDialog.value = false;
};
</script>

<template>
    <div class="toolbar">
        <div class="toolbar-section">
            <sl-button variant="default" size="medium" @click="openShiftDialog('backward')">
                <sl-icon slot="prefix" name="arrow-left"></sl-icon>
                Shift Backward
            </sl-button>
            <sl-button variant="default" size="medium" @click="openShiftDialog('forward')">
                <sl-icon slot="prefix" name="arrow-right"></sl-icon>
                Shift Forward
            </sl-button>
        </div>

        <div class="toolbar-section">
            <sl-button variant="danger" size="medium" @click="handleReset">
                <sl-icon slot="prefix" name="arrow-counterclockwise"></sl-icon>
                Reset
            </sl-button>
        </div>

        <!-- Shift Dialog -->
        <sl-dialog :open="showShiftDialog" @sl-hide="showShiftDialog = false" label="Shift Operations">
            <p>
                Move all operations <strong>{{ shiftDirection }}</strong> by:
            </p>
            <sl-input 
                v-model="shiftMinutes" 
                type="number" 
                label="Minutes" 
                :min="1"
                :max="1440"
                help-text="Enter the number of minutes to shift all operations"
            >
                <sl-icon slot="prefix" name="clock"></sl-icon>
            </sl-input>

            <div slot="footer" class="dialog-footer">
                <sl-button variant="default" @click="showShiftDialog = false">
                    Cancel
                </sl-button>
                <sl-button variant="primary" @click="handleShift">
                    Apply Shift
                </sl-button>
            </div>
        </sl-dialog>

        <!-- Optimize Dialog -->
        <sl-dialog :open="showOptimizeDialog" @sl-hide="showOptimizeDialog = false" label="Optimize Schedule">
            <div class="dialog-content">
                <sl-icon name="magic" style="font-size: 3rem; color: var(--sl-color-primary-600);"></sl-icon>
                <p style="margin-top: 1rem;">
                    This will optimize the schedule to:
                </p>
                <ul>
                    <li>Sort operations by start time</li>
                    <li>Remove gaps between operations</li>
                </ul>
                <p style="margin-top: 1rem; font-weight: bold;">
                    Are you sure you want to continue?
                </p>
            </div>

            <div slot="footer" class="dialog-footer">
                <sl-button variant="default" @click="showOptimizeDialog = false">
                    Cancel
                </sl-button>
                <sl-button variant="primary" @click="confirmOptimize">
                    <sl-icon slot="prefix" name="check"></sl-icon>
                    Optimize
                </sl-button>
            </div>
        </sl-dialog>

        <!-- Reset Dialog -->
        <sl-dialog :open="showResetDialog" @sl-hide="showResetDialog = false" label="Reset Schedule">
            <div class="dialog-content">
                <sl-icon name="exclamation-triangle" style="font-size: 3rem; color: var(--sl-color-danger-600);"></sl-icon>
                <p style="margin-top: 1rem;">
                    This will reset all schedule modifications back to the original state.
                </p>
                <p style="margin-top: 0.5rem; font-weight: bold; color: var(--sl-color-danger-600);">
                    This action cannot be undone!
                </p>
            </div>

            <div slot="footer" class="dialog-footer">
                <sl-button variant="default" @click="showResetDialog = false">
                    Cancel
                </sl-button>
                <sl-button variant="danger" @click="confirmReset">
                    <sl-icon slot="prefix" name="arrow-counterclockwise"></sl-icon>
                    Reset Schedule
                </sl-button>
            </div>
        </sl-dialog>
    </div>
</template>

<style scoped>
.toolbar {
    display: flex;
    align-items: center;
    gap: 1rem;
    padding: 1rem;
    background: var(--sl-color-neutral-50);
    border-radius: 8px;
    margin-bottom: 1.5rem;
    flex-wrap: wrap;
}

.toolbar-section {
    display: flex;
    gap: 0.5rem;
    align-items: center;
}

.toolbar-divider {
    width: 1px;
    height: 32px;
    background: var(--sl-color-neutral-300);
}

.dialog-footer {
    display: flex;
    gap: 0.5rem;
    justify-content: flex-end;
}

.dialog-content {
    display: flex;
    flex-direction: column;
    align-items: center;
    text-align: center;
    padding: 1rem 0;
}

.dialog-content ul {
    text-align: left;
    margin: 0.5rem 0;
    padding-left: 1.5rem;
}

.dialog-content li {
    margin: 0.25rem 0;
}

@media (max-width: 768px) {
    .toolbar {
        flex-direction: column;
        align-items: stretch;
    }
    
    .toolbar-section {
        width: 100%;
    }
    
    .toolbar-divider {
        display: none;
    }
}
</style>