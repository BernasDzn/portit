<script setup lang="ts">

</script>

<template>
  <div>
    <sl-input class="main-searchbar" placeholder="Search..." size="large" clearable>
      <sl-icon class="small" name="search" slot="prefix"></sl-icon>
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
  color: white;
}

</style>