<script setup lang="ts">
import { ref } from 'vue';
import { RouterLink } from 'vue-router';


const sidebarItems = ref([
  // add a materialIcon property with the Material Icons name we want to render
  { name: 'Dashboard', route: '/', icon: "house", materialIcon: 'home' },
  { name: 'Qualifications', route: '/qualifications', icon:"mortarboard", materialIcon: 'school' },
  { name: 'Vessels', route: '/vessels', icon: "ship", materialIcon: 'directions_boat' },
]);

</script>

<template>
  <nav class="sidebar">
    <ul class="sidebar-menu">
      <li v-for="item in sidebarItems" class="sidebar-menu-item">
        <RouterLink :to="item.route" class="sidebar-menu-link">
          <span class="material-icons icon" aria-hidden="true">{{ item.materialIcon }}</span>
          <p>{{item.name}}</p>
        </RouterLink>
      </li>
    </ul>
  </nav>
</template>

<style scoped>

.icon {
  margin: auto 10px auto 0;
  font-size: 24px;
  color: inherit; /* inherit color from the link so hover can change it */
  display: inline-flex;
  align-items: center;
}

.sidebar-menu-link:hover .icon {
  /* make hover color explicit in case of specificity issues */
  color: var(--text-black);
}

</style>