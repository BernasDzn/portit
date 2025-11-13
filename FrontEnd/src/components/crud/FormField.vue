<script setup>
import { ref, computed, watch } from 'vue';


const props = defineProps({
    name: { type: String, required: true },
    placeholderText: { type: String, default: '' },
    modelValue: { type: [String, Number, Boolean], default: '' },
    enabled: { type: Boolean, default: true },
    pattern: { type: String, default: '' },
    required: { type: Boolean, default: false },
    // optional explicit id for the inner input element (useful for tests)
    inputId: { type: String, default: null },
    type: { type: String, default: 'text' }
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
                v-if="type==='text'"
                :id="inputId"
                :value="inputValue"
                @sl-input="inputValue = $event.target.value"
                :placeholder="props.placeholderText"
                :disabled="!props.enabled"
                v-bind="pattern ? { pattern } : {}"
                :required="props.required"
                filled
            />
            
            <sl-textarea
                v-else-if="type==='textarea'"
                :id="inputId"
                :value="inputValue"
                @sl-input="inputValue = $event.target.value"
                :placeholder="props.placeholderText"
                :disabled="!props.enabled"
                v-bind="pattern ? { pattern } : {}"
                :required="props.required"
                filled
            />
            
            <sl-input
                v-else-if="type==='datetime-local'"
                type="datetime-local"
                :id="inputId"
                :value="inputValue"
                @sl-change="inputValue = $event.target.value"
                :placeholder="props.placeholderText"
                :disabled="!props.enabled"
                v-bind="pattern ? { pattern } : {}"
                :required="props.required"
                filled
            />
            
            <sl-checkbox
                v-else-if="type==='checkbox'"
                :id="inputId"
                :checked="inputValue"
                @sl-change="inputValue = $event.target.checked"
                :disabled="!props.enabled"
                :required="props.required"
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