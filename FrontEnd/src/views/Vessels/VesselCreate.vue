<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import FormField from '@/components/crud/FormField.vue';
import { AxiosHttpService } from '@/service/AxiosHttpService';
import { VesselService } from '@/service/VesselService';
import type { Vessel } from '@/model/Vessel';
import router from '@/router';

const vesselName = ref('');
const vesselIMO = ref('');
const depth = ref('');
const draft = ref('');
const length = ref('');

const vesselTypes = ref<string[]>([]);
const selectedType = ref('Vessel Type');
const submitting = ref(false);
const error = ref('');
const success = ref('');
const cancelDialog = ref<HTMLElement | null>(null);

const hasUnsaved = computed(() => {
    return !!(vesselName.value || vesselIMO.value || depth.value || draft.value || length.value || (selectedType.value && selectedType.value !== 'Vessel Type'));
});

const http = new AxiosHttpService('https://localhost:5001');
const vesselService = new VesselService(http);
const isFormValid = computed(() => {
    if (!vesselName.value.trim()) return false;
    if (!vesselIMO.value.trim()) return false;
    if (!selectedType.value || selectedType.value === 'Vessel Type') return false;
    const d = parseFloat(depth.value as unknown as string);
    const dr = parseFloat(draft.value as unknown as string);
    const l = parseFloat(length.value as unknown as string);
    if (!isFinite(d) || !isFinite(dr) || !isFinite(l)) return false;
    return true;
});

function selectType(t: string) {
    selectedType.value = t;
}

async function submit() {
    error.value = '';
    success.value = '';
    if (!isFormValid.value) {
        error.value = 'Please fill all fields correctly.';
        return;
    }

    submitting.value = true;
    try {
        const vessel: Vessel = {
            name: vesselName.value,
            imoNumber: vesselIMO.value,
            type: selectedType.value,
            owner: 'Global Shipping Co.', // TROCAR MAIS TARDE PELO OWNER DO REPRESENTATIVE LOGGED IN
            length: parseInt(length.value as unknown as string),
            depth: parseInt(depth.value as unknown as string),
            draft: parseInt(draft.value as unknown as string)
        };

        await vesselService.createVessel(vessel);
        success.value = "Vessel created successfully.";
        // reset form
        vesselName.value = '';
        vesselIMO.value = '';
        selectedType.value = 'Vessel Type';
        depth.value = '';
        draft.value = '';
        length.value = '';
    } catch (e: any) {
        error.value = e?.message ?? 'Failed to create vessel';
        console.error(e);
    } finally {
        submitting.value = false;
    }
}

function onCancel() {
    if (hasUnsaved.value) {
        // show confirmation dialog
        (cancelDialog.value as any)?.show?.();
    } else {
        // no unsaved changes - navigate back
        router.push('/vessels/dashboard');
    }
}

function confirmCancel() {
    (cancelDialog.value as any)?.hide?.();
    router.push('/vessels/dashboard');
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
            <sl-breadcrumb-item><RouterLink to="/vessels/dashboard" class="breadcrumb-link">Vessel Dashboard</RouterLink></sl-breadcrumb-item>
            <sl-breadcrumb-item>Create Vessel</sl-breadcrumb-item>
        </sl-breadcrumb>
        <h1 class="title">Create Vessel</h1>
        <p class="subtitle">Register a new vessel into the system</p>
        <sl-card class="create-vessel-form">
                <div class="name-imo">
                    <FormField class="field" name="Vessel Name" v-model="vesselName" placeholderText="Vessel Name"/>
                    <FormField class="field" name="Vessel IMO" v-model="vesselIMO" placeholderText="IMO XXXXXXX"/>
                    <div class="field-dropdown">
                        <sl-label>Vessel Type</sl-label>
                        <sl-dropdown class="type" placement="bottom-start">
                            <sl-button slot="trigger" caret>{{ selectedType }}</sl-button>
                            <sl-menu>
                                <template v-if="vesselTypes.length">
                                    <sl-menu-item v-for="type in vesselTypes" :key="type" @click="selectType(type)">{{ type }}</sl-menu-item>
                                </template>
                                <template v-else>
                                    <sl-menu-item disabled>Loading...</sl-menu-item>
                                </template>
                            </sl-menu>
                        </sl-dropdown>
                    </div>
                </div>
            <div class="measurements">
                <FormField class="field" name="Length (m)" v-model="length" placeholderText="e.g. 100"/>
                <FormField class="field" name="Depth (m)" v-model="depth" placeholderText="e.g. 10"/>
                <FormField class="field" name="Draft (m)" v-model="draft" placeholderText="e.g. 2.5"/>
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

    <sl-dialog ref="cancelDialog" label="Discard changes?" >
        <div>You have unsaved changes. Do you really want to discard them and leave?</div>
        <sl-button slot="footer" variant="text" @click="(cancelDialog as any).hide()">Keep editing</sl-button>
        <sl-button slot="footer" variant="danger" @click="confirmCancel">Discard</sl-button>
    </sl-dialog>
    </div>
</template>

<style scoped>
.name-imo {
    display: flex;
    gap: .5rem;
}

.measurements {
    display: flex;
    gap: .5rem;
}

.create-vessel-form {
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