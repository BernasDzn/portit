<script setup lang="ts">
import type { StorageArea } from '@/model/StorageArea';
import type { Filter, Page } from '@/model/Page';
import { useI18n } from 'vue-i18n';
import ListingBox from '@/components/crud/ListingBox.vue';
import StorageAreaPrinter from '@/components/printers/StorageAreaPrinter.vue';
import type { IStorageAreaService } from '@/service/IService/IStorageAreaService';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';

const storageAreaService = container.get<IStorageAreaService>(TYPES.storageAreaService);

const { t } = useI18n();

const fetchStorageAreas = async (filtering?: Filter<{ nameCode: string }>): Promise<Page<StorageArea>> => {
  return storageAreaService.getStorageAreas(filtering);
}

</script>

<template>
  <div>

    <sl-breadcrumb>
      <sl-breadcrumb-item><RouterLink to="/storage-areas/dashboard" class="breadcrumb-link">{{ t('storageArea.tabs.dashboard') }}</RouterLink></sl-breadcrumb-item>
      <sl-breadcrumb-item>{{ t('storageArea.tabs.search') }}</sl-breadcrumb-item>
    </sl-breadcrumb>

    <header>
      <h1 class="title">{{ t('storageArea.title') }}</h1>
      <p class="subtitle">{{ t('storageArea.subtitle.search') }}</p>

      <ListingBox listingStyle="listing-grid" :fetch-function="fetchStorageAreas" search-filter="nameCode" v-slot="{elements}">
        <li v-for="storageArea in elements" :key="storageArea.nameCode">
          <StorageAreaPrinter class="listing-box" :storage-area="storageArea" :link="`/storage-areas/view/${storageArea.nameCode}`" />
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