<script setup lang="ts">
import { ref } from 'vue';

const height = ref(window.innerHeight - 120);

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

  position: fixed;
  top: 80px;
  bottom: 0;
  right: 0;
  left: 0;

  background: black;
  display: flex;
  justify-content: center;
  align-items: center;
  overflow: hidden;
  margin: 0;
  padding: 0;
}

html {
    background-color: black !important;
}

.viewer-iframe {
  width: 100%;
  height: 100%;
  border: none;
  display: block;
  background: black;
  object-fit: contain;
}
</style>
