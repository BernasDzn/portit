<script setup lang="ts">
import { ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import type { IncidentType } from '@/model/IncidentType';
import EntityView from '@/components/crud/EntityView.vue';
import type { IIncidentTypeService } from '@/service/IService/IIncidentTypeService';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';

const route = useRoute();
const router = useRouter();

const incidentTypeService = container.get<IIncidentTypeService>(TYPES.incidentTypeService);
const incidentTypeId = route.params.id as string;
const deleteDialog = ref<HTMLElement | null>(null);

const fetchIncidentType = async (): Promise<IncidentType | undefined> => {
    return await incidentTypeService.getIncidentTypeById(incidentTypeId);
};

const confirmDelete = () => {
    (deleteDialog.value as any)?.show?.();
};

const doDelete = async () => {
    try {
        await incidentTypeService.deleteIncidentType(incidentTypeId);
        router.push('/incident-types/search');
    } catch (error) {
        console.error('Failed to delete incident type', error);
        alert('Failed to delete incident type');
    } finally {
        (deleteDialog.value as any)?.hide?.();
    }
};

const getSeverityVariant = (severity: string): string => {
    switch (severity) {
        case 'Critical': return 'danger';
        case 'Major': return 'warning';
        case 'Minor': return 'primary';
        default: return 'neutral';
    }
};

</script>

<template>
    <div>
        <sl-breadcrumb>
            <sl-breadcrumb-item>
                <RouterLink to="/incident-types/dashboard" class="breadcrumb-link">Incident Types Dashboard</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink to="/incident-types/search" class="breadcrumb-link">Search Incident Types</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>{{ incidentTypeId }}</sl-breadcrumb-item>
        </sl-breadcrumb>

        <EntityView :fetch-function="fetchIncidentType" v-slot="entity">
            <div>
                <div class="opposed">
                    <div class="view-header">
                        <span class="material-icons icon" aria-hidden="true">warning</span>
                        <div>
                            <h2 class="title">{{ entity.element.name }}</h2>
                            <p class="subtitle">
                                <sl-badge :variant="getSeverityVariant(entity.element.severity)">
                                    {{ entity.element.severity }}
                                </sl-badge>
                            </p>
                        </div>
                    </div>
                    <div style="display: flex; gap: 0.5rem;">
                        <RouterLink :to="`/incident-types/edit/${entity.element.id}`">
                            <sl-button variant="default" size="large">
                                <sl-icon slot="prefix" name="pencil"></sl-icon>
                                Edit
                            </sl-button>
                        </RouterLink>
                        <sl-button variant="danger" size="large" @click="confirmDelete">
                            <sl-icon slot="prefix" name="trash"></sl-icon>
                            Delete
                        </sl-button>
                    </div>
                </div>
                <div class="viewing-content">
                    <sl-card class="info-card" style="flex: 100%;">
                        <p>Incident Type Information</p>
                        <div class="info-grid">
                            <div class="info-block">
                                <span class="label">Name</span>
                                <p>{{ entity.element.name }}</p>
                            </div>
                            <div class="info-block">
                                <span class="label">Severity</span>
                                <p>
                                    <sl-badge :variant="getSeverityVariant(entity.element.severity)">
                                        {{ entity.element.severity }}
                                    </sl-badge>
                                </p>
                            </div>
                            <div class="info-block" style="flex: 100%;">
                                <span class="label">Description</span>
                                <p>{{ entity.element.description }}</p>
                            </div>
                        </div>
                    </sl-card>

                    <sl-card class="info-card" style="flex: 100%;" v-if="entity.element.subtypeOfId">
                        <p>Parent Type</p>
                        <div class="info-grid">
                            <div class="info-block">
                                <span class="label">Subtype Of</span>
                                <p>{{ entity.element.subtypeOfId }}</p>
                            </div>
                        </div>
                    </sl-card>

                    <sl-card class="info-card" style="flex: 100%;" v-if="entity.element.subtypesIds && entity.element.subtypesIds.length > 0">
                        <p>Subtypes ({{ entity.element.subtypesIds.length }})</p>
                        <div class="info-grid">
                            <div class="info-block" v-for="subtypeId in entity.element.subtypesIds" :key="subtypeId">
                                <p>{{ subtypeId }}</p>
                            </div>
                        </div>
                    </sl-card>
                </div>
            </div>
        </EntityView>

        <sl-dialog ref="deleteDialog" label="Confirm delete">
            <div>Are you sure you want to delete this incident type?</div>
            <sl-button slot="footer" variant="text" @click="(deleteDialog as any).hide()">Cancel</sl-button>
            <sl-button slot="footer" variant="danger" @click="doDelete">Delete</sl-button>
        </sl-dialog>
    </div>
</template>

<style scoped>
.info-block {
    flex: 1 1 45%;
    min-width: 200px;
}

.viewing-content {
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
}

.info-grid {
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
}

.label {
    font-weight: 600;
    color: var(--sl-color-neutral-600);
    font-size: 0.9rem;
}
</style>
