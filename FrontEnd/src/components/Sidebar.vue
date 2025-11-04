<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { RouterLink, useRoute } from 'vue-router';
import { useI18n } from 'vue-i18n';

const route = useRoute();
const { t } = useI18n();

const sidebarItems = ref([
  // add a materialIcon property with the Material Icons name we want to render
  { name: 'dashboard.sidebarTitle', route: '/', icon: "house", materialIcon: 'home' },
  {},
  { name: 'vessel.title', route: '/vessels/dashboard', icon: "directions_boat", materialIcon: 'directions_boat' },
  { name: 'vesselType.title', route: '/vessel-types/dashboard', icon: "sailing", materialIcon: 'sailing' },
  { name: 'dock.title', route: '/docks/dashboard', icon: "anchor", materialIcon: 'anchor' },
  {},
  { name: 'qualification.title', route: '/qualifications/dashboard', icon:"mortarboard", materialIcon: 'school' },
  { name: 'physicalResource.title', route: '/resources/dashboard', icon: "inventory", materialIcon: 'build' },
  { name: 'staff.title', route: '/staff/dashboard', icon: "people", materialIcon: 'people' },
  {},
  { name: "Admin", route: '/admin/dashboard', icon: "admin_panel_settings", materialIcon: 'admin_panel_settings' }
]);

const isCurrentTab = (itemRoute: string) => {
  return route.path === itemRoute;
};

</script>

<template>
  <nav class="sidebar">
    <ul class="sidebar-menu">
      <li v-for="item in sidebarItems" class="sidebar-menu-item">
        <RouterLink v-if="item.route" 
          :to="item.route" 
          :class="(isCurrentTab(item.route) ? 'link-active' : '') + ' sidebar-menu-link'"
        >
          <span class="material-icons icon" aria-hidden="true">{{ item.materialIcon }}</span>
          <p>{{ t(item.name) }}</p>
        </RouterLink>
        <hr v-else class="sidebar-separator"/>
      </li>
    </ul>
  </nav>
</template>

<style scoped>

.icon {
  margin: auto 10px auto 0;
  font-size: 24px;
  color: inherit;
  display: inline-flex;
  align-items: center;
}

</style>