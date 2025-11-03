<script setup lang="ts">
import { RouterLink } from 'vue-router';

const props = defineProps<{
  route: string;
}>();

</script>

<template>
  <!-- Use Shoelace sl-card as the visible wrapper so the component uses Shoelace properly -->
  <sl-card class="port3d-card" aria-label="3D Port Visualization Card">
    <div class="hero-content" role="region">
      <div class="hero-left">
        <h3 class="hero-title">
          <span class="material-icons">widgets</span>
          3D Port Visualization
        </h3>
        <p class="hero-description">
          View an interactive 3D representation of the entire port facility. Monitor dock occupancy, vessel
          positions, storage areas, and resources in real-time from a bird's-eye view.
        </p>
        <div class="hero-actions">
          <!-- Use RouterLink's v-slot to get a navigate() callback and call it from the Shoelace button click
               This avoids placing a self-closing RouterLink inside the button which does nothing. -->
          <RouterLink :to="props.route">
            <sl-button variant="default" size="large" class="hero-cta"  >
              <span slot="prefix" class="material-symbols-outlined">open_in_new</span>
              Open 3D View
            </sl-button>
          </RouterLink>
        </div>
      </div>

      <div class="hero-right" aria-hidden="true">
        <div class="hero-icon-bg">
          <span class="material-symbols-outlined cube">deployed_code</span>
        </div>
      </div>
    </div>
  </sl-card>
</template>

<style scoped>
.port3d-card {
  width: 100%;
  margin-top: 1rem;
  display: block;
}
.hero-content {
    display: flex;
    gap: 2rem;
    align-items: center;
    justify-content: space-between;
    background: linear-gradient(135deg, #3b5998 0%, #4b6cb7 50%, #2b4b7a 100%);
    color: #fff;
    border-radius: 12px;
    padding: 28px 32px;
    box-shadow: 0 6px 18px rgba(15, 23, 42, 0.08);
}
.hero-left {
    flex: 1 1 60%;
    min-width: 0;
}
.hero-title {
    display: flex;
    align-items: center;
    gap: 12px;
    margin: 0 0 8px 0;
    font-size: 20px;
    font-weight: 600;
}
.hero-title-icon {
    font-size: 28px;
    opacity: 0.95;
}
.hero-description {
    margin: 0 0 18px 0;
    color: rgba(255,255,255,0.92);
    max-width: 60ch;
    line-height: 1.5;
}
.hero-actions {
    display: flex;
    gap: 12px;
}
.hero-cta::part(base) {
    display: inline-flex;
    align-items: center;
    gap: 8px;
    background: rgba(255,255,255,0.95);
    color: #2b4b7a;
    border-radius: 8px;
}
.hero-right {
    flex: 0 0 180px;
    display: flex;
    align-items: center;
    justify-content: center;
}
.hero-icon-bg {
    width: 140px;
    height: 140px;
    border-radius: 12px;
    background: rgba(255,255,255,0.05);
    display: flex;
    align-items: center;
    justify-content: center;
}
.material-icons.cube,
.material-symbols-outlined.cube { opacity: 0.95; height: 80px; width: 80px; font-size: 80px; color: #ffffff;  }

/* Responsive: stack vertically on narrow screens */
@media (max-width: 720px) {
  .hero-content {
    flex-direction: column;
    align-items: stretch;
  }
  .hero-right {
    align-self: center;
  }
}

</style>
