<script setup>
import { ref, computed, watch, onMounted } from 'vue';

const props = defineProps({
    name: { type: String, required: true },
    placeholderText: { type: String, default: 'Select an option' },
    modelValue: { type: [String, Number, Object, Array], default: '' },
    enabled: { type: Boolean, default: true },
    items: { type: Array, default: () => [] },
    // Optional function to fetch items: should return an array or a Promise that resolves to an array
    fetchFunction: { type: Function, default: null },
    // If true and fetchFunction provided, fetch on mounted
    fetchOnMount: { type: Boolean, default: true },
    // Keys for value/label when items are objects
    valueKey: { type: String, default: 'id' },
    labelKey: { type: String, default: 'name' },
    required: { type: Boolean, default: false },
    multiple: { type: Boolean, default: false },
    // Optional initial defaults applied once on mount (or after items load)
    // Use null so an absent prop doesn't force an empty selection.
    defaultValues: { type: Array, default: null }
});

const emit = defineEmits(['update:modelValue']);
const options = ref(Array.isArray(props.items) ? props.items.slice() : []);
const loading = ref(false);
const defaultApplied = ref(false);

function encodeRaw(v) {
    return typeof v === 'string' ? encodeURIComponent(v) : encodeURIComponent(String(v ?? ''));
}

function decodeKey(k) {
    try { return decodeURIComponent(k); } catch { return k; }
}

function optionKeyFor(opt) {
    const raw = (opt && typeof opt === 'object') ? opt[props.valueKey] : opt;
    return encodeRaw(raw);
}

function applyDefaultsIfNeeded() {
    if (defaultApplied.value) return;
    const modelEmpty = props.modelValue === undefined || props.modelValue === null || (props.multiple && Array.isArray(props.modelValue) && props.modelValue.length === 0) || props.modelValue === '';
    if (!modelEmpty) return; // parent already provided a value
    if (!Array.isArray(props.defaultValues) || !props.defaultValues.length) return;

    if (props.multiple) {
        internalValue.value = props.defaultValues.map(encodeRaw);
    } else {
        internalValue.value = encodeRaw(props.defaultValues[0]);
    }
    defaultApplied.value = true;
}

const internalValue = ref(props.multiple
    ? (Array.isArray(props.modelValue) && props.modelValue.length ? props.modelValue.map(encodeRaw) : [])
    : (props.modelValue !== undefined && props.modelValue !== null ? encodeRaw(props.modelValue) : null));

watch(() => props.items, (val) => {
    // items changed — try applying defaults now that options exist
    options.value = val;
    applyDefaultsIfNeeded();
});

watch(() => props.modelValue, (val) => {
    if (props.multiple) {

        const newEnc = val.map(encodeRaw);
        internalValue.value = newEnc;

    } else {
        const newEnc = (val !== undefined && val !== null) ? encodeRaw(val) : null;
        if (newEnc !== internalValue.value) {
            internalValue.value = newEnc;
        }
    }
});

watch(internalValue, (val) => {
    if (props.multiple) {
        const arr = val.map((k) => decodeKey(String(k)));
        if (JSON.stringify(props.modelValue) !== JSON.stringify(arr)) {
            emit('update:modelValue', arr);
        }
    } else {
        const decoded = val ? decodeKey(String(val)) : null;
        if (decoded !== props.modelValue) {
            emit('update:modelValue', decoded);
        }
    }
});
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
            // After loading items from fetchFunction, apply defaults if appropriate
            applyDefaultsIfNeeded();
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

function onChange(e) {
    
    const el = e.target || e.currentTarget;

    if (props.multiple) {
        
        const raw = (el && el.selectedValues) || (e.detail && e.detail.value) || (el && el.value) || [];
        internalValue.value = raw;

    } else {
        const val = (el && el.value) || (e.detail && e.detail.value) || null;
        internalValue.value = val ? String(val) : null;
    }
}
</script>

<template>
    <div class="form-field">
        <label class="label" :for="inputId">{{ name }}</label>

        <div>
            <sl-select
                :id="inputId"
                class="entity-dropdown"
                v-model="internalValue"
                @sl-change="onChange"
                :disabled="!enabled || loading"
                :required="required"
                :multiple="multiple"
                :placeholder="placeholderText"
            >
                <sl-option
                    v-for="(opt, idx) in options"
                    :key="idx + '-' + (opt && opt[valueKey] !== undefined ? opt[valueKey] : opt)"
                    :value="optionKeyFor(opt)"
                >
                    {{ (opt && typeof opt === 'object') ? opt[labelKey] : opt }}
                </sl-option>
            </sl-select>
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

.loading {
    margin-left: 1rem;
    font-size: 0.9rem;
    color: var(--sl-color-primary-600, #555);
}


</style>
