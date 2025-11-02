<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import FormField from '@/components/crud/FormField.vue';
import { AxiosHttpService } from '@/service/AxiosHttpService';
import { DockService } from '@/service/DockService';
import type { Dock } from '@/model/Dock';
import router from '@/router';

const code = ref('');
const name = ref('');
const location = ref('');
const depth = ref('');
const draft = ref('');
const length = ref('');

const vesselTypes = ref<string[]>([]);
const selectedTypes = ref<string[]>(['Vessel Types']);
const submitting = ref(false);
const error = ref('');
const success = ref('');
const cancelDialog = ref<HTMLElement | null>(null);

const hasUnsaved = computed(() => {
    return !!(code.value || name.value || location.value || depth.value || draft.value || length.value || (selectedTypes.value && selectedTypes.value.length > 0 && selectedTypes.value[0] !== 'Vessel Types'));
});

const http = new AxiosHttpService('https://localhost:5001');
const dockService = new DockService(http);
const isFormValid = computed(() => {
    if (!code.value.trim()) return false;
    if (!name.value.trim()) return false;
    if (!location.value.trim()) return false;
    if (!selectedTypes.value || selectedTypes.value.length === 0) return false;
    const d = parseFloat(depth.value as unknown as string);
    const dr = parseFloat(draft.value as unknown as string);
    const l = parseFloat(length.value as unknown as string);
    if (!isFinite(d) || !isFinite(dr) || !isFinite(l)) return false;
    return true;
});

async function submit() {
    error.value = '';
    success.value = '';
    if (!isFormValid.value) {
        error.value = 'Please fill all fields correctly.';
        return;
    }

    submitting.value = true;
    try {
        const dock: Dock = {
            code: code.value,
            name: name.value,
            location: location.value,
            physicalCharacteristics: {
                length: parseFloat(length.value as unknown as string),
                depth: parseFloat(depth.value as unknown as string),
                draft: parseFloat(draft.value as unknown as string)
            },
            supportedVesselTypes: selectedTypes.value.filter(t => t !== 'Vessel Types').map(t => t)
        };

        await dockService.createDock(dock);
        success.value = "Dock created successfully.";
        // reset form
        code.value = '';
        name.value = '';
        location.value = '';
        selectedTypes.value = ['Vessel Types'];
        depth.value = '';
        draft.value = '';
        length.value = '';
    } catch (e: any) {
        error.value = e?.message ?? 'Failed to create dock';
        console.error(e);
    } finally {
        submitting.value = false;
    }
}

function onCancel() {
    if (hasUnsaved.value) {
        (cancelDialog.value as any)?.show?.();
    } else {
        router.back();
    }
}

function confirmCancel() {
    (cancelDialog.value as any)?.hide?.();
    router.back();
}

onMounted(async () => {
    try {
        const res = await fetch('https://localhost:5001/VesselType');
        if (!res.ok) {
            console.error('Failed to fetch vessel types', res.statusText);
            return;
        }
        const data = await res.json();
        if (Array.isArray(data)) {
            if (data.length === 0) {
                vesselTypes.value = [];
            } else if (typeof data[0] === 'string') {
                vesselTypes.value = data;
            } else {
                vesselTypes.value = data.map((d: any) => d.name ?? d.Name ?? String(d));
            }
        }
    } catch (e) {
        console.error('Error loading vessel types', e);
    }
});
</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item><RouterLink to="../docks/dashboard" class="breadcrumb-link">Dock Dashboard</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>Create Dock</sl-breadcrumb-item>
        </sl-breadcrumb>
        <h1 class="title">Create Dock</h1>
        <p class="subtitle">Register a new dock into the system</p>
        <sl-card class="create-dock-form">
            <div class="formContainer">
                <FormField class="field" name="Code" v-model="code" placeholderText="e.g. DCKXXXX" />
                <FormField class="field" name="Name" v-model="name" placeholderText="e.g. Main Dock" />
                <FormField class="field" name="Location" v-model="location" placeholderText="e.g. Location A" />
                <div class="field-dropdown">
                    <sl-label>Supported Vessel Types</sl-label>
                    <sl-dropdown class="type" placement="bottom-start">
                        <sl-button slot="trigger" caret>{{ selectedTypes.join(', ') }}</sl-button>
                        <sl-menu multiple>
                            <template v-if="vesselTypes.length">
                                <sl-menu-item type="checkbox" v-for="type in vesselTypes" :key="type" :value="type"
                                    @click="
                                        if(selectedTypes.includes('Vessel Types'))
                                            selectedTypes = [];
                                        
                                        if (selectedTypes.includes(type))
                                            selectedTypes = selectedTypes.filter(t => t !== type);
                                        else
                                            selectedTypes.push(type);

                                        if(selectedTypes.length === 0)
                                            selectedTypes = ['Vessel Types'];
                                        ">
                                    {{ type }}
                                </sl-menu-item>
                            </template>
                            <template v-else>
                                <sl-menu-item disabled>Loading...</sl-menu-item>
                            </template>
                        </sl-menu>
                    </sl-dropdown>
                </div>
            </div>
            <div class="measurements">
                <FormField class="field" name="Length (m)" v-model="length" placeholderText="e.g. 100" />
                <FormField class="field" name="Depth (m)" v-model="depth" placeholderText="e.g. 10" />
                <FormField class="field" name="Draft (m)" v-model="draft" placeholderText="e.g. 2.5" />
            </div>
            <p class="subtitle form-tip" v-if="!isFormValid">All fields are required.</p>
            <div class="buttons">
                <sl-button class="form-button" variant="danger" outline @click="onCancel">
                    Cancel
                </sl-button>
                <sl-button class="form-button" variant="primary" :disabled="!isFormValid || submitting" @click="submit">
                    Create
                </sl-button>
            </div>
        </sl-card>
        <div class="form-messages">
            <sl-alert variant="danger" open v-if="error">{{ error }}</sl-alert>
            <sl-alert variant="success" open v-if="success">{{ success }}</sl-alert>
        </div>

        <sl-dialog ref="cancelDialog" label="Discard changes?">
            <div>You have unsaved changes. Do you really want to discard them and leave?</div>
            <sl-button slot="footer" variant="text" @click="(cancelDialog as any).hide()">Keep editing</sl-button>
            <sl-button slot="footer" variant="danger" @click="confirmCancel">Discard</sl-button>
        </sl-dialog>
    </div>
</template>

<style scoped>
.formContainer {
    display: flex;
    gap: .5rem;
}

.measurements {
    display: flex;
    gap: .5rem;
}

.create-dock-form {
    width: 100%;
}

.field {
    margin-bottom: 1rem;
    max-width: 30rem;
    padding: .5rem
}

.field-dropdown {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    margin-bottom: 1rem;
    margin-top: 0.5rem;
    margin-left: 0.5rem;
    max-width: 30rem;
}

.buttons {
    display: flex;
    justify-content: flex-end;
    gap: 1rem;
}

.form-button {
    min-width: 100px;
}

.form-messages {
    margin: 0.5rem 0 1rem 0;
    bottom: 1rem;
}

.form-tip {
    font-size: 0.9rem;
    color: #666666;
    margin-bottom: 1rem;
    display: flex;
    justify-content: flex-end;
}
</style>