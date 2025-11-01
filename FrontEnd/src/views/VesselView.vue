<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { AxiosHttpService } from '@/service/AxiosHttpService';
import { VesselService } from '@/service/VesselService';
import type { Vessel } from '@/model/Vessel';
import Loading from '@/components/Loading.vue';
import NoResults from '@/components/NoResults.vue';

const route = useRoute();
const imoNumber = decodeURIComponent((route.params.imo ?? '') as string);

const vessel = ref<Vessel | null>(null);
const loading = ref(false);
const error = ref('');

const http = new AxiosHttpService('https://localhost:5001');
const vesselService = new VesselService(http);

onMounted(async () => {
  loading.value = true;
  try {
    vessel.value = await (vesselService as any).getVesselByIMO(imoNumber);
  } catch (e: any) {
    error.value = e?.message ?? 'Failed to load vessel';
  } finally {
    loading.value = false;
  }
});
</script>

<template>
  <div>
    <sl-breadcrumb>
      <sl-breadcrumb-item><RouterLink to="/vessels/dashboard" class="link">Vessel Dashboard</RouterLink></sl-breadcrumb-item>
      <sl-breadcrumb-item>View Vessel</sl-breadcrumb-item>
    </sl-breadcrumb>
    <h1>View Vessel</h1>

    <Loading v-if="loading" />
    <sl-card v-else-if ="vessel" class="content-item-description">
        <div class="item-description">
            <h2>{{ vessel.name }}</h2>
            <p>
                {{ vessel.imoNumber }}<br/>
                {{ vessel.type.name }}<br/>
                {{ vessel.owner.name }}<br/>
                <div class="physical-characteristics-card">
                    <h4>Physical Characteristics</h4>
                    Length: {{ vessel.physicalCharacteristics.length }}m<br/>
                    Draft: {{ vessel.physicalCharacteristics.draft }}m<br/>
                    Depth: {{ vessel.physicalCharacteristics.depth }}m<br/>
                </div>
            </p>
            <div class="buttons">
                <sl-button variant="primary" size="medium" class="button">
                <RouterLink :to="`/vessels/edit/${encodeURIComponent(vessel.imoNumber)}`" class="link">Edit Vessel</RouterLink>
                </sl-button>
            </div>
        </div>
    </sl-card>
    <NoResults noResultsMessage="Vessel not found." v-if="error"/>
  </div>
</template>

<style scoped> 
.link {
  text-decoration: none;
  color: inherit;
}

.content-item-description {
  width: 100%;
}

.physical-characteristics-card {
  margin-top: 1em;
}

.buttons {
    display: flex;
    justify-content: flex-end;
    gap: 1rem;
}

.button {
    max-width: 100px;
}
</style>