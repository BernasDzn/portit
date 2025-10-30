<script setup lang="ts">

</script>

<template>
  <div>
    <sl-input class="main-searchbar" placeholder="Search..." size="large" clearable>
      <span slot="prefix" class="material-icons material-icons--prefix">search</span>
    </sl-input>
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

</style>