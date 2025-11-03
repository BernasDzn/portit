<script setup lang="ts">
import { ref, watch } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const listOfRoutes = router.getRoutes().map(route => route.name);

const value = ref('');
const showMenu = ref(false);

watch(value, (newValue) => {
  // Show the menu only when the input has text
  showMenu.value = newValue.length > 0;
});

function handleClear() {
    value.value = '';
}

</script>

<template>
  <div class="search-container">
    <sl-input
      class="main-searchbar"
      placeholder="Search..."
      size="large"
      clearable
      v-model="value"
      @sl-clear="handleClear"
    >
      <span slot="prefix" class="material-icons material-icons--prefix">search</span>
    </sl-input>

    <transition name="fade">
      <sl-menu v-if="showMenu" class="search-menu">
        <sl-menu-item v-for="item in listOfRoutes">
            {{ item }}
        </sl-menu-item>
        <sl-divider></sl-divider>
      </sl-menu>
    </transition>
  </div>
</template>

<style scoped>

.main-searchbar {
    padding: 15px;
}
  
.main-searchbar::part(base) {
  
    font-size: var(--sl-font-size-medium);
    background-color: transparent;
    border: none;
}
  
.main-searchbar::part(input) {
    color: var(--text-primary);
}
  
.main-searchbar::part(prefix) {
    color: var(--text-primary);
}
  
.search-menu {
    position: absolute;
    margin-top: 0.25rem;
    background: var(--sl-color-neutral-0);
    border: 1px solid var(--sl-color-neutral-300);
    border-radius: var(--sl-border-radius-medium);
    box-shadow: var(--sl-shadow-large);
    z-index: 10;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.15s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
