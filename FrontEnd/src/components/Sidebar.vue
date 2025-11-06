<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { RouterLink, useRoute } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { useSession } from '@/composables/session';

const route = useRoute();
const { t } = useI18n();

const sidebarItems = ref([
  // add a materialIcon property with the Material Icons name we want to render
  { name: 'dashboard.sidebarTitle', route: '/', icon: "house", materialIcon: 'home', roles: [0] },
  {},
  { name: 'vessel.title', route: '/vessels/dashboard', icon: "directions_boat", materialIcon: 'directions_boat', roles: [0, 1] },
  { name: 'vesselType.title', route: '/vessel-types/dashboard', icon: "sailing", materialIcon: 'sailing', roles: [0, 1] },
  { name: 'dock.title', route: '/docks/dashboard', icon: "anchor", materialIcon: 'anchor', roles: [0, 1] },
  { name: 'notification.title', route: '/vessel-visit-notifications/dashboard', icon: "ballot", materialIcon: 'ballot', roles: [0, 2, 1] },
  {},
  { name: 'qualification.title', route: '/qualifications/dashboard', icon:"mortarboard", materialIcon: 'school', roles: [0, 3] },
  { name: 'physicalResource.title', route: '/resources/dashboard', icon: "inventory", materialIcon: 'build', roles: [0, 3] },
  { name: 'staff.title', route: '/staff/dashboard', icon: "people", materialIcon: 'people', roles: [0, 3] },
  { name: 'storageArea.title', route: '/storage-areas/dashboard', icon: "warehouse", materialIcon: 'warehouse', roles: [0, 1] },
  {},
  { name: "admin.sidebarTitle", route: '/admin/dashboard', icon: "admin_panel_settings", materialIcon: 'admin_panel_settings', roles: [0] }
]);

onMounted(() => {
    const session = useSession();
    console.log(session.authenticatedUser?.role);
})

const isCurrentTab = (itemRoute: string) => {
  return route.path === itemRoute;
};

const itemsToShow = computed(() => {
  const session = useSession();
  return sidebarItems.value.filter(item => {
    if (!item.route) return true; // keep separators
    return item.roles?.includes(session.authenticatedUser?.role || -1);
  });
});

</script>

<template>
  <nav class="sidebar">
    <ul class="sidebar-menu">
      <li v-for="item in itemsToShow" class="sidebar-menu-item">
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