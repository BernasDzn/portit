<script setup lang="ts">
    import ListingBox from '@/components/crud/ListingBox.vue';
    import type { IncidentDto } from '@/model/dto/IncidentDto'
    import type { Filter } from '@/model/Page'
    import type { Page } from '@/model/Page'
    import { useI18n } from 'vue-i18n';
    import { ref, onMounted, watch } from 'vue';
    import type { IIncidentService } from '@/service/IService/IIncidentService';
    import { container } from '@/inversify.config';
    import TYPES from '@/inversify/types';
import IncidentPrinter from '@/components/printers/IncidentPrinter.vue';
    
    const incidentService = container.get<IIncidentService>(TYPES.incidentService);
    
    const fetchIncidents = async (filtering?: Filter<any>): Promise<Page<IncidentDto>> => {
      return await incidentService.getAllIncidents(filtering);
    }
    
    const { t, locale } = useI18n();
    const filterDefinition = ref({});
    
    function buildFilterDefinition() {
      filterDefinition.value = {
        vveCode: {
          type: 'text',
          label: t('incident.fields.vveCode.title'),
        },
        severity: {
          type: 'select',
          label: t('incident.fields.severity.title'),
          options: [
            { value: 'Minor', text: t('incident.severity.Minor') },
            { value: 'Major', text: t('incident.severity.Major') },
            { value: 'Critical', text: t('incident.severity.Critical') },
          ],
        },
        isResolved: {
          type: 'select',
          label: t('incident.fields.status.title'),
          options: [
            { value: 'false', text: t('incident.fields.status.options.active') },
            { value: 'true', text: t('incident.fields.status.options.resolved') },
          ],
        },
        filterStartTime: {
          type: 'date',
          label: t('incident.fields.filterStartTime.title'),
        },
        filterEndTime: {
          type: 'date',
          label: t('incident.fields.filterEndTime.title'),
        },
      };
    }
    
    onMounted(() => buildFilterDefinition());
    watch(locale, () => buildFilterDefinition());
    </script>
    
    <template>
      <div>
        <sl-breadcrumb>
          <sl-breadcrumb-item>
            <RouterLink to="/incidents/dashboard" class="breadcrumb-link">
              {{ t('incident.tabs.dashboard') }}
            </RouterLink>
          </sl-breadcrumb-item>
          <sl-breadcrumb-item>{{ t('incident.tabs.search') }}</sl-breadcrumb-item>
        </sl-breadcrumb>
        <header>
          <h1 class="title">{{ t('incident.tabs.search') }}</h1>
          <p class="subtitle">{{ t('incident.subtitle.search') }}</p>
          <ListingBox 
            :fetch-function="fetchIncidents" 
            v-slot="{elements}" 
            :filter-definition="filterDefinition"
          >
            <li v-for="incident in elements" :key="incident.bid">
              <IncidentPrinter
                class="listing-box" 
                :incident="incident" 
                :link="`/incidents/view/${incident.bid}`" 
              />
            </li>
          </ListingBox>
        </header>
      </div>
    </template>
    
    <style scoped> 
    .link {
      text-decoration: none;
      color: inherit;
    }
    </style>