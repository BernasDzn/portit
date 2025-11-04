<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';

const router = useRouter();

const displayedRoutes = computed(() => {
  return router.getRoutes()
    .filter(route => route.name != undefined && !route.meta.hideFromSearch)
    .filter(route => {
      if (value.value === '') return true;
      return route.name!.toString().toLowerCase().includes(value.value.toLowerCase());
    });
});

const value = ref('');
const showMenu = ref(false);

const {t} = useI18n();

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
      :placeholder="t('buttons.search').concat('...')"
      size="large"
      clearable
      v-model="value"
      @sl-clear="handleClear"
    >
      <span slot="prefix" class="material-icons material-icons--prefix">search</span>
    </sl-input>

    <transition name="fade">
      <sl-menu v-if="showMenu" class="search-menu">
        <sl-menu-item v-for="item in displayedRoutes">
            <RouterLink :to="item.path" @click="handleClear">
                <div class="search-icon">
                    <span class="material-icons icon" style="color: var(--accent-1);">{{ item.meta.icon }}</span>
                    {{ item.name }}
                </div>
            </RouterLink>
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

.search-icon {
    display: flex;
    align-items: center;
    gap: 10px;
}

.search-menu a {
    text-decoration: none; 
    color: inherit;        
    display: block;        
}

</style>
