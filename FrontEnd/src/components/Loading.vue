<script setup lang="ts">
import { onMounted } from 'vue';
import lottie from 'lottie-web';

onMounted(() => {
    const container = document.getElementById('loader');
    if (!container) return;

    lottie.loadAnimation({
        container,
        renderer: 'svg',
        loop: true,
        autoplay: true,
        path: '../../public/animation.json'
    });
});
</script>

<template>
    <div id="loader" aria-hidden="true"></div>
</template>

<style scoped>
#loader {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 100px;
    height: 100px;
}
</style>