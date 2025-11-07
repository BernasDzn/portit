<script setup lang="ts">
import { ref } from 'vue';

const height = ref(window.innerHeight - 120);

</script>

<template>
  <div class="viewer-container" :style="`height: ${height}px;`">
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
    top:100px;
    left: 0;

    background: black; /* letterbox background color */
    display: flex;
    justify-content: center;
    align-items: center;
    overflow: hidden;
    margin: 0;
    padding: 0;

    width: 100%;
}

html {
    background-color: black !important;
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
