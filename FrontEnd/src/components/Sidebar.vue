<script setup lang="ts">
import { ref } from 'vue';
import { RouterLink } from 'vue-router';


const sidebarItems = ref([
  { name: 'Dashboard', route: '/', icon: "house" },
  { name: 'Qualifications', route: '/qualifications', icon:"mortarboard" },
]);

</script>

<template>
  <nav class="sidebar">
    <ul class="sidebar-menu">
      <li v-for="item in sidebarItems" class="sidebar-menu-item">
        <RouterLink :to="item.route" class="sidebar-menu-link">
          <sl-icon class="icon" :name="item.icon"></sl-icon>
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
}

</style>