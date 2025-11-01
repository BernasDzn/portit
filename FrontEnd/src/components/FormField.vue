<template>
    <div class="form-field">
        <sl-label :for="inputId">{{ name }}</sl-label>
        <slot>
            <sl-input :id="inputId" v-model="inputValue" :placeholder="placeholderText"></sl-input>
        </slot>
    </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue';

const props = defineProps({
    name: {
        type: String,
        required: true
    },
    placeholderText: {
        type: String,
        default: ''
    },
    modelValue: {
        type: [String, Number],
        default: ''
    }
});

const emit = defineEmits(['update:modelValue']);

const inputValue = ref(props.modelValue);

const inputId = computed(() => `form-field-${props.name.replace(/\s+/g, '-').toLowerCase()}`);

watch(inputValue, (val) => {
    emit('update:modelValue', val);
});
watch(() => props.modelValue, (val) => {
    inputValue.value = val;
});
</script>

<style scoped>
.form-field {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}
</style>