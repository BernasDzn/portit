<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { RouterLink, useRoute } from 'vue-router';

const route = useRoute();

const sidebarItems = ref([
  // add a materialIcon property with the Material Icons name we want to render
  { name: 'Dashboard', route: '/', icon: "house", materialIcon: 'home' },
  {},
  { name: 'Vessels', route: '/vessels/dashboard', icon: "directions_boat", materialIcon: 'directions_boat' },
  { name: 'Vessel Types', route: '/vessel-types/dashboard', icon: "sailing", materialIcon: 'sailing' },
  { name: 'Docks', route: '/docks/dashboard', icon: "anchor", materialIcon: 'anchor' },
  {},
  { name: 'Qualifications', route: '/qualifications/dashboard', icon:"mortarboard", materialIcon: 'school' },
  { name: 'Staff', route: '/staff/dashboard', icon: "people", materialIcon: 'people' },
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
          <p>{{item.name}}</p>
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