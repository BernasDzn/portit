<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import type { IncidentType } from '@/model/IncidentType';
import EntityView from '@/components/crud/EntityView.vue';
import IncidentTypePrinter from '@/components/printers/IncidentTypePrinter.vue';
import type { IIncidentTypeService } from '@/service/IService/IIncidentTypeService';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
const route = useRoute();
const router = useRouter();

const incidentTypeService = container.get<IIncidentTypeService>(TYPES.incidentTypeService);
const incidentTypeId = computed(() => route.params.id as string);
const deleteDialog = ref<HTMLElement | null>(null);

const parent = ref<IncidentType | null>(null);
const subtypes = ref<IncidentType[]>([]);

// btw this should probably be moved to either a composable, service layer or domain class to keep it clean
const fetchIncidentType = async (): Promise<IncidentType | undefined> => {
    const incidentType = await incidentTypeService.getIncidentTypeById(incidentTypeId.value);

    // Fetch subtypes and save them in the ref var
    subtypes.value = [];
    if (incidentType?.subtypesIds?.length) {
        const fetched = await Promise.all(
            incidentType.subtypesIds.map((id) => incidentTypeService.getIncidentTypeById(id))
        );
        subtypes.value = fetched.filter((item): item is IncidentType => Boolean(item));
    }
    
    // Fetch parent type and save it in the ref var
    parent.value = null;
    if (incidentType?.subtypeOfId) {
        const parentType = await incidentTypeService.getIncidentTypeById(incidentType.subtypeOfId);
        if (parentType) {
            parent.value = parentType;
        }
    } else {
        parent.value = null;
    }

    return incidentType;
};
    

const confirmDelete = () => {
    (deleteDialog.value as any)?.show?.();
};

const doDelete = async () => {
    try {
        await incidentTypeService.deleteIncidentType(incidentTypeId.value);
        router.push('/incident-types/search');
    } catch (error) {
        console.error('Failed to delete incident type', error);
        alert(t('incidentType.failedToDelete'));
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
                <RouterLink to="/incident-types/dashboard" class="breadcrumb-link">{{ t('incidentType.tabs.dashboard') }}</RouterLink>
            </sl-breadcrumb-item>
            <sl-breadcrumb-item>
                <RouterLink to="/incident-types/search" class="breadcrumb-link">{{ t('incidentType.tabs.search') }}</RouterLink>
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
                                    {{ t(`incidentType.severity.${entity.element.severity}`) }}
                                </sl-badge>
                            </p>
                        </div>
                    </div>
                    <div style="display: flex; gap: 0.5rem;">
                        <RouterLink :to="`/incident-types/edit/${entity.element.id}`">
                            <sl-button variant="default" size="large">
                                <sl-icon slot="prefix" name="pencil"></sl-icon>
                                {{ t('buttons.edit') }}
                            </sl-button>
                        </RouterLink>
                        <sl-button variant="danger" size="large" @click="confirmDelete">
                            <sl-icon slot="prefix" name="trash"></sl-icon>
                            {{ t('buttons.delete') }}
                        </sl-button>
                    </div>
                </div>
                <div class="viewing-content">
                    <sl-card class="info-card" style="flex: 100%;">
                        <p>{{ t('incidentType.incidentTypeInformation') }}</p>
                        <div class="info-grid">
                            <div class="info-block">
                                <span class="label">{{ t('incidentType.fields.name.title') }}</span>
                                <p>{{ entity.element.name }}</p>
                            </div>
                            <div class="info-block">
                                <span class="label">{{ t('incidentType.fields.severity.title') }}</span>
                                <p>
                                    <sl-badge :variant="getSeverityVariant(entity.element.severity)">
                                        {{ t(`incidentType.severity.${entity.element.severity}`) }}
                                    </sl-badge>
                                </p>
                            </div>
                            <div class="info-block" style="flex: 100%;">
                                <span class="label">{{ t('incidentType.fields.description.title') }}</span>
                                <p>{{ entity.element.description }}</p>
                            </div>
                        </div>
                    </sl-card>

                    <sl-card class="info-card" style="flex: 100%;" v-if="entity.element.subtypeOfId">
                        <p>{{ t('incidentType.parentType') }}</p>
                        <div class="info-grid">
                            <div class="info-block" :key="parent?.id">
                                <IncidentTypePrinter
                                    class="listing-box"
                                    :incident-type="parent as IncidentType"
                                    :link="'/incident-types/view/' + parent.id"
                                />
                            </div>
                        </div>
                    </sl-card>

                    <sl-card class="info-card" style="flex: 100%;" v-if="subtypes.length > 0">
                        <p>{{ t('incidentType.fields.subtypes.title') }} ({{ subtypes.length }})</p>
                        <div class="info-grid">
                            <div class="info-block" v-for="subtype in subtypes" :key="subtype.id">
                                <IncidentTypePrinter
                                    class="listing-box"
                                    :incident-type="subtype as IncidentType"
                                    :link="'/incident-types/view/' + subtype.id"
                                />
                            </div>
                        </div>
                    </sl-card>
                </div>
            </div>
        </EntityView>

        <sl-dialog ref="deleteDialog" :label="t('incidentType.confirmDelete')">
            <div>{{ t('incidentType.confirmDeleteMessage') }}</div>
            <sl-button slot="footer" variant="text" @click="(deleteDialog as any).hide()">{{ t('buttons.cancel') }}</sl-button>
            <sl-button slot="footer" variant="danger" @click="doDelete">{{ t('buttons.delete') }}</sl-button>
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
