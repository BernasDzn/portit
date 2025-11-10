<script setup>
import { ref, computed, watch } from 'vue';


const props = defineProps({
    name: { type: String, required: true },
    placeholderText: { type: String, default: '' },
    modelValue: { type: [String, Number], default: '' },
    enabled: { type: Boolean, default: true },
    pattern: { type: String, default: '' },
    required: { type: Boolean, default: false },
    // optional explicit id for the inner input element (useful for tests)
    inputId: { type: String, default: null }
});

const emit = defineEmits(['update:modelValue']);

const inputValue = ref(props.modelValue);
const inputId = computed(() => props.inputId ?? `form-field-${props.name.replace(/\s+/g, '-').toLowerCase()}`);

watch(inputValue, (val) => emit('update:modelValue', val));
watch(() => props.modelValue, (val) => (inputValue.value = val));

</script>

<template>
    <div class="form-field">
        <sl-label class="label" :for="inputId" v-if="name != 'null'">{{ name }}</sl-label>
        <slot>
            <sl-input 
                :id="inputId" 
                v-model="inputValue" 
                :placeholder="props.placeholderText" 
                :disabled="!props.enabled" 
                v-bind="pattern ? { pattern } : {}" 
                :required="props.required" 
                filled 
            />
        </slot>
    </div>
</template>

<style scoped>

.form-field {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

</style>