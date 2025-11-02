<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRoute } from 'vue-router';
import FormField from '@/components/crud/FormField.vue';
import Loading from '@/components/Loading.vue';
import { AxiosHttpService } from '@/service/AxiosHttpService';
import { VesselService } from '@/service/VesselService';
import type { Vessel } from '@/model/Vessel';
import router from '@/router';

const route = useRoute();
const http = new AxiosHttpService('https://localhost:5001');
const vesselService = new VesselService(http);

const loading = ref(true);
const submitting = ref(false);
const error = ref('');
const success = ref('');
const cancelDialog = ref<HTMLElement | null>(null);


const vesselName = ref('');
const vesselIMO = ref('');
const selectedType = ref('Vessel Type');
const length = ref('');
const depth = ref('');
const draft = ref('');

const vesselTypes = ref<string[]>([]);
const originalIMO = ref<string | null>(null);

const isFormValid = computed(() => {
    if (!vesselName.value.trim()) return false;
    if (!vesselIMO.value.trim()) return false;
    if (!selectedType.value || selectedType.value === 'Vessel Type') return false;
    const nL = parseFloat(length.value as unknown as string);
    const nD = parseFloat(depth.value as unknown as string);
    const nDr = parseFloat(draft.value as unknown as string);
    if (!isFinite(nL) || !isFinite(nD) || !isFinite(nDr)) return false;
    return true;
});

// Snapshot of initial form state (stringified) used to detect real changes
const initialSnapshot = ref<string>('');
const isDirty = computed(() => {
    const current = JSON.stringify({
        name: vesselName.value,
        imo: vesselIMO.value,
        type: selectedType.value,
        length: length.value,
        depth: depth.value,
        draft: draft.value
    });
    return initialSnapshot.value !== '' && current !== initialSnapshot.value;
});

function selectType(t: string) {
    selectedType.value = t;
}

async function loadVesselAndTypes(imo: string) {
    loading.value = true;
    error.value = '';
    try {
        const [v, typesRes] = await Promise.all([
            vesselService.getVesselByIMO(imo),
            fetch('https://localhost:5001/VesselType')
        ]);

        // populate form from vessel (support nested or flat shapes)
        if (v) {
            originalIMO.value = imo;
            vesselName.value = v.name ?? '';
            vesselIMO.value = v.imoNumber ?? imo;
            selectedType.value = typeof v.type === 'string' ? v.type : ((v.type as any)?.name ?? String(v.type ?? 'Vessel Type'));
            const pc: any = (v as any).physicalCharacteristics;
            length.value = pc?.length != null ? String(pc.length) : ((v as any).length != null ? String((v as any).length) : '');
            depth.value = pc?.depth != null ? String(pc.depth) : ((v as any).depth != null ? String((v as any).depth) : '');
            draft.value = pc?.draft != null ? String(pc.draft) : ((v as any).draft != null ? String((v as any).draft) : '');
        }

        // parse types response
        if (typesRes && typesRes.ok) {
            const data = await typesRes.json();
            if (Array.isArray(data)) {
                vesselTypes.value = data.map((d: any) => (typeof d === 'string' ? d : d.name ?? d.Name ?? String(d)));
            }
        }

        // capture initial snapshot for dirty-checking
        initialSnapshot.value = JSON.stringify({
            name: vesselName.value,
            imo: vesselIMO.value,
            type: selectedType.value,
            length: length.value,
            depth: depth.value,
            draft: draft.value
        });
    } catch (e: any) {
        error.value = e?.message ?? 'Failed to load vessel data';
        // eslint-disable-next-line no-console
        console.error('loadVesselAndTypes error', e);
    } finally {
        loading.value = false;
    }
}

onMounted(async () => {
    const imo = decodeURIComponent((route.params.imo ?? '') as string || '');
    if (!imo) {
        error.value = 'Missing IMO in route. Cannot edit vessel.';
        loading.value = false;
        return;
    }

    await loadVesselAndTypes(imo);
});

async function submit() {
    error.value = '';
    success.value = '';
    if (!isFormValid.value) {
        error.value = 'Please fill all fields correctly.';
        return;
    }

    if (!originalIMO.value) {
        error.value = 'Missing original IMO; cannot update vessel.';
        return;
    }

    submitting.value = true;
    try {
        const payload: Vessel = {
            name: vesselName.value,
            imoNumber: vesselIMO.value,
            type: selectedType.value,
            owner: 'Global Shipping Co.', // keep existing placeholder
            length: parseFloat(length.value as unknown as string),
            depth: parseFloat(depth.value as unknown as string),
            draft: parseFloat(draft.value as unknown as string)
        } as any;

        await vesselService.updateVessel(originalIMO.value, payload);
        success.value = 'Vessel updated successfully.';
    } catch (e: any) {
        error.value = e?.message ?? 'Failed to update vessel';
        // eslint-disable-next-line no-console
        console.error('update error', e);
    } finally {
        submitting.value = false;
    }
}

function onCancel() {
    if (isDirty.value) {
        (cancelDialog.value as any)?.show?.();
    } else {
        router.back();
    }
}

function confirmCancel() {
    (cancelDialog.value as any)?.hide?.();
    router.back();
}
</script>

<template>
    <div class="vessel-edit">
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/vessels/dashboard" class="link">Vessel Dashboard</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink to="/vessels/search" class="link">Search Vessels</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink :to="vesselIMO ? `/vessels/view/${vesselIMO}` : '/vessels/search'" class="link">
                    {{ vesselIMO || 'IMO' }}
                </RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>Edit Vessel</sl-breadcrumb-item>
        </sl-breadcrumb>

        <header class="page-header">
            <h1>Edit Vessel</h1>
        </header>

        <Loading v-if="loading" />

        <sl-card v-else class="create-vessel-form">
            <div class="name-imo">
                <FormField class="field" name="Vessel Name" v-model="vesselName" placeholderText="Vessel Name" />
                <FormField class="field" name="Vessel IMO" v-model="vesselIMO" placeholderText="IMO XXXXXXX" :enabled="false"/>

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
                <FormField class="field" name="Length (m)" v-model="length" placeholderText="e.g. 100" />
                <FormField class="field" name="Depth (m)" v-model="depth" placeholderText="e.g. 10" />
                <FormField class="field" name="Draft (m)" v-model="draft" placeholderText="e.g. 2.5" />
            </div>

            <p class="subtitle form-tip" v-if="!isFormValid">All fields are required.</p>

            <div class="buttons">
                <sl-button class="form-button" variant="danger" outline @click="onCancel">Cancel</sl-button>
                <sl-button class="form-button" variant="primary" :disabled="!isFormValid || submitting" @click="submit">Save changes</sl-button>
            </div>

            <div class="form-messages">
                <sl-alert variant="danger" open v-if="error">{{ error }}</sl-alert>
                <sl-alert variant="success" open v-if="success">{{ success }}</sl-alert>
            </div>

            <sl-dialog ref="cancelDialog" label="Discard changes?">
                <div>You have unsaved changes. Do you really want to discard them and leave?</div>
                <sl-button slot="footer" variant="text" @click="(cancelDialog as any).hide()">Keep editing</sl-button>
                <sl-button slot="footer" variant="danger" @click="confirmCancel">Discard</sl-button>
            </sl-dialog>
        </sl-card>
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

.vessel-edit {
    width: 100%;
}

.page-header {
    display: flex;
    justify-content: space-between;
    gap: 1rem;
    margin-bottom: 1rem;
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

.link {
  text-decoration: none;
  color: inherit;
}

</style>