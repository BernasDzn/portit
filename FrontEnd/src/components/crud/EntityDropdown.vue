<script setup>
import { ref, computed, watch, onMounted } from 'vue';

const props = defineProps({
    name: { type: String, required: true },
    placeholderText: { type: String, default: 'Select an option' },
    modelValue: { type: [String, Number, Object], default: '' },
    enabled: { type: Boolean, default: true },
    items: { type: Array, default: () => [] },
    // Optional function to fetch items: should return an array or a Promise that resolves to an array
    fetchFunction: { type: Function, default: null },
    // If true and fetchFunction provided, fetch on mounted
    fetchOnMount: { type: Boolean, default: true },
    // Keys for value/label when items are objects
    valueKey: { type: String, default: 'id' },
    labelKey: { type: String, default: 'name' },
    required: { type: Boolean, default: false }
});

const emit = defineEmits(['update:modelValue']);

const internalValue = ref(props.modelValue);
const options = ref(Array.isArray(props.items) ? props.items.slice() : []);
const loading = ref(false);

watch(() => props.items, (val) => {
    options.value = Array.isArray(val) ? val.slice() : [];
});

watch(() => props.modelValue, (val) => (internalValue.value = val));

watch(internalValue, (val) => emit('update:modelValue', val));

async function loadItems() {
    if (typeof props.fetchFunction === 'function') {
        try {
            loading.value = true;
            const result = props.fetchFunction();
            const resolved = result instanceof Promise ? await result : result;

            // Accept multiple shapes: plain array, axios-style { data: [...] }, or paged { items: [...] }
            if (Array.isArray(resolved)) {
                options.value = resolved;
            } else if (resolved && Array.isArray(resolved.data)) {
                options.value = resolved.data;
            } else if (resolved && Array.isArray(resolved.items)) {
                options.value = resolved.items;
            } else {
                // Unknown shape — try to be helpful by logging
                // eslint-disable-next-line no-console
                console.warn('EntityDropdown: fetchFunction returned unexpected shape', resolved);
                options.value = [];
            }
        } catch (err) {
            // eslint-disable-next-line no-console
            console.error('EntityDropdown: error loading items', err);
            options.value = [];
        } finally {
            loading.value = false;
        }
    }
}

onMounted(() => {
    if (props.fetchOnMount && typeof props.fetchFunction === 'function') {
        loadItems();
    }
});

const inputId = computed(() => `entity-dropdown-${props.name.replace(/\s+/g, '-').toLowerCase()}`);
</script>

<template>
    <div class="form-field">
        <label class="label" :for="inputId">{{ name }}</label>

        <div>
            <select
                :id="inputId"
                class="entity-dropdown"
                v-model="internalValue"
                :disabled="!enabled || loading"
                :required="required"
            >
                <option value="" disabled>{{ placeholderText }}</option>
                <option
                    v-for="(opt, idx) in options"
                    :key="idx + '-' + (opt && opt[valueKey] !== undefined ? opt[valueKey] : opt)"
                    :value="(opt && typeof opt === 'object') ? opt[valueKey] : opt"
                >
                    {{ (opt && typeof opt === 'object') ? opt[labelKey] : opt }}
                </option>
            </select>
            <span v-if="loading" class="loading">Loading...</span>
        </div>
    </div>
</template>

<style scoped>
.form-field {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.entity-dropdown {
    padding: 0.65rem;
    border: 1px solid var(--sl-color-neutral-300, #cfcfcf);
    border-radius: 4px;
    background: white;
}

.loading {
    margin-left: 1rem;
    font-size: 0.9rem;
    color: var(--sl-color-primary-600, #555);
}
</style>
