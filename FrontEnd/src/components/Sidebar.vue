<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { RouterLink, useRoute, useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { useSession } from '@/composables/session';

const route = useRoute();
const { t } = useI18n();
const session = useSession();

const role = computed(() => session.authenticatedUser?.role ?? -1);

const sidebarItems = ref([
  // add a materialIcon property with the Material Icons name we want to render
  { name: 'dashboard.sidebarTitle', route: '/', icon: "house", materialIcon: 'home', roles: [0, 1, 2, 3] },
  {},
  { name: 'dock.title', route: '/docks/dashboard', icon: "anchor", materialIcon: 'anchor', roles: [0, 1] },
  { name: 'qualification.title', route: '/qualifications/dashboard', icon:"mortarboard", materialIcon: 'school', roles: [0, 3] },
  { name: 'physicalResource.title', route: '/resources/dashboard', icon: "inventory", materialIcon: 'build', roles: [0, 3] },
  { name: 'staff.title', route: '/staff/dashboard', icon: "people", materialIcon: 'people', roles: [0, 3] },
  { name: 'storageArea.title', route: '/storage-areas/dashboard', icon: "warehouse", materialIcon: 'warehouse', roles: [0, 1] },
  { name: 'vessel.title', route: '/vessels/dashboard', icon: "directions_boat", materialIcon: 'directions_boat', roles: [0, 1] },
  {},
  { name: 'incidentType.title', route: '/incident-types/dashboard', icon: "nearby_error", materialIcon: 'nearby_error', roles: [0, 1, 3] },
  { name: 'scheduling.title', route: '/scheduling-dashboard', icon: "calendar_month", materialIcon: 'calendar_month', roles: [0, 3] },
  { name: 'notification.title', route: '/vessel-visit-notifications/dashboard', icon: "ballot", materialIcon: 'ballot', roles: [0, 2, 1] },
  {},
  { name: "admin.sidebarTitle", route: '/admin/dashboard', icon: "admin_panel_settings", materialIcon: 'admin_panel_settings', roles: [0] },
  {},
  { name: 'about.title', route: '/about', icon: "info", materialIcon: 'info', roles: [0, 1, 2, 3] },
]);

const isCurrentTab = (itemRoute: string) => {
  return route.path === itemRoute;
};

const itemsToShow = computed(() => {
  return sidebarItems.value.filter(item => {
    // separators have no roles; show them only for admins
    if (!item.roles) return role.value === 0;
    // if unauthenticated (role -1) hide role-protected items
    if (role.value < 0) return false;
    return item.roles.includes(role.value);
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
  min-width: 24px;
  justify-content: center;
}

.sidebar-menu-link {
  overflow: hidden;
}

.sidebar{
  z-index: 1;
}

</style>