<script setup lang="ts">


</script>

<template>
    <sl-card>

        <div class="box">
            <p class="hint">Drop down a cargo manifest file</p>
        </div>
        
    </sl-card>
</template>

<style scoped>

.box {
    border: 2px dashed var(--sl-color-primary-500);
    border-radius: 8px;
    padding: 40px;
    text-align: center;
    cursor: pointer;
    transition: background-color 0.3s, border-color 0.3s;
}

.box:hover {
    background-color: var(--sl-color-primary-50);
    border-color: var(--sl-color-primary-700);
}

.hint {
    color: var(--sl-color-gray-600);
    font-size: 1.1em;
}

</style>