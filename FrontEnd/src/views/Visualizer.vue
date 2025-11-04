<script setup lang="ts">
const loading = false
</script>

<template>
  <div class="viewer-container">
    <iframe
      src="/visualizer/index.html"
      class="viewer-iframe"
      title="Port3D"
    ></iframe>
  </div>
</template>

<style scoped>
.viewer-container {
  width: 100vw;
  height: 100vh;
  background: black; /* letterbox background color */
  display: flex;
  justify-content: center;
  align-items: center;
  overflow: hidden;
  margin: 0;
  padding: 0;
}

.viewer-iframe {
  aspect-ratio: 16 / 9;
  width: 100%;
  height: auto;
  max-width: 100%;
  max-height: 100%;
  border: none;
  display: block;
  background: black;
  object-fit: contain;
}
</style>
