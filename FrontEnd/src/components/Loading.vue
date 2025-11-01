<script setup lang="ts">
import { onMounted, onBeforeUnmount, ref, watch} from 'vue';
import lottie from 'lottie-web';

const props = withDefaults(defineProps<{ color?: string }>(), { color: '#485EA9' });

const containerRef = ref<HTMLElement | null>(null);
let animationInstance: any = null;
const originalData = ref<any>(null);

function hexToNormalizedRGB(hex: string) {
    if (!hex) return [0, 0, 0];

    const h = hex.replace('#', '');
    const bigint = h.length === 3
        ? parseInt(h.split('').map(c => c + c).join(''), 16)
        : parseInt(h, 16);
    const r = (bigint >> 16) & 255;
    const g = (bigint >> 8) & 255;
    const b = bigint & 255;
    return [+(r / 255).toFixed(4), +(g / 255).toFixed(4), +(b / 255).toFixed(4)];
}

function replaceColors(obj: any, rgb: number[]) {
    if (!obj || typeof obj !== 'object') return;

    if (obj.c && obj.c.k && Array.isArray(obj.c.k) && obj.c.k.length >= 3 && obj.c.k.every((n: any) => typeof n === 'number')) {
        obj.c.k[0] = rgb[0];
        obj.c.k[1] = rgb[1];
        obj.c.k[2] = rgb[2];
        return;
    }

    for (const key in obj) {
        if (!Object.prototype.hasOwnProperty.call(obj, key)) continue;
        const val = obj[key];
        if (Array.isArray(val)) {
            for (const item of val) replaceColors(item, rgb);
        } else if (typeof val === 'object' && val !== null) {
            replaceColors(val, rgb);
        }
    }
}

async function loadAndPlay(colorHex: string) {
    const container = containerRef.value || document.querySelector('.loader');
    if (!container) return;

    if (!originalData.value) {
        try {
            const res = await fetch('/animation.json');
            originalData.value = await res.json();
        } catch (e) {
            const res = await fetch('animation.json');
            originalData.value = await res.json();
        }
    }

    const dataCopy = JSON.parse(JSON.stringify(originalData.value));
    const rgb = hexToNormalizedRGB(colorHex || props.color || '#485EA9');
    replaceColors(dataCopy, rgb);

    if (animationInstance) {
        try { animationInstance.destroy(); } catch (e) {}
        animationInstance = null;
    }

    animationInstance = lottie.loadAnimation({
        container: container as Element,
        renderer: 'svg',
        loop: true,
        autoplay: true,
        animationData: dataCopy
    });
}

onMounted(() => {
    containerRef.value = document.querySelector('.loader') as HTMLElement | null;
    loadAndPlay(props.color as string);
});

watch(() => props.color, (newColor) => {
    loadAndPlay(newColor as string);
});

onBeforeUnmount(() => {
    if (animationInstance) {
        try { animationInstance.destroy(); } catch (e) {}
        animationInstance = null;
    }
});
</script>

<template>
    <div class="loader" aria-hidden="true"></div>
</template>