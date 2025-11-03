<script setup lang="ts">
import NotificationQueue from '@/components/NotificationQueue.vue';
import { RouterLink } from 'vue-router';
</script>

<template>
  <div class="unauthorized-page">
    <sl-card class="unauthorized-card">
      <div class="content">
        <sl-icon name="shield-exclamation" style="font-size: 3rem; color: var(--sl-color-danger-600);" />
        <h1>403 — Unauthorized</h1>
        <p>You don't have permission to access this page.</p>

        <RouterLink to="/login">
          <sl-button variant="primary" size="medium">
            <sl-icon slot="prefix" name="box-arrow-in-right" />
            Go back to Login
          </sl-button>
        </RouterLink>
      </div>
    </sl-card>

    <NotificationQueue />
  </div>
</template>

<style scoped>
.unauthorized-page {
    position: absolute;
    
    top: calc(50% - 200px);
    left: calc(50% - 200px);
}

.unauthorized-card {
  text-align: center;
  padding: 2rem;
  max-width: 400px;
}

.content h1 {
  font-size: 1.75rem;
  margin: 1rem 0 0.5rem;
  color: var(--sl-color-neutral-900);
}

.content p {
  color: var(--sl-color-neutral-600);
  margin-bottom: 1.5rem;
}

sl-button::part(base) {
  width: 100%;
  justify-content: center;
}
</style>
