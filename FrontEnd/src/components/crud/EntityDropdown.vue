<script setup lang="ts">
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
    required: { type: Boolean, default: false },
    selectedElement: { type: [String, Number, Object], default: null }
});

const emit = defineEmits(['update:modelValue']);

const internalValue = ref(props.modelValue);
const options = ref<any[]>(Array.isArray(props.items) ? props.items.slice() : []);
const loading = ref(false);

const displayLabel = computed(() => {
    if (internalValue.value === null || internalValue.value === undefined || internalValue.value === '') {
        return props.placeholderText;
    }

    if (options.value && options.value.length) {
        const match = (options.value as Record<string, any>[]).find((opt) => {
            if (opt && typeof opt === 'object') {
                return opt[props.valueKey] === internalValue.value;
            }
            return opt === internalValue.value;
        });

        if (match !== undefined) {
            return (match && typeof match === 'object') ? match[props.labelKey] : match;
        }
    }

    return internalValue.value;
});

function applySelectedElement() {
    const sel = props.selectedElement;
    if (sel === null || sel === undefined) return;
    if (internalValue.value !== null && internalValue.value !== undefined && internalValue.value !== '') return;
    if (!options.value || !options.value.length) return;

    const setFromMatch = (match: unknown) => {
        if (match === undefined) return;
        internalValue.value = (match && typeof match === 'object') ? (match as Record<string, any>)[props.valueKey] : match;
    };

    if (typeof sel === 'object') {
        const match = options.value.find(opt => {
            if (opt && typeof opt === 'object') {
                return ((sel as Record<string, any>)[props.valueKey] !== undefined && (opt as Record<string, any>)[props.valueKey] === (sel as Record<string, any>)[props.valueKey]) || opt === sel;
            }
            return opt === sel;
        });
        setFromMatch(match);
    } else {
        const match = options.value.find(opt => {
            if (opt && typeof opt === 'object') {
                return (opt as Record<string, any>)[props.valueKey] === sel;
            }
            return opt === sel;
        });
        setFromMatch(match);
    }
}

watch(() => props.items, (val) => {
    options.value = Array.isArray(val) ? val.slice() : [];
    applySelectedElement();
});

watch(() => props.modelValue, (val) => {
    if (val === null || val === undefined || val === '') {
        internalValue.value = val;
        return;
    }

    if (val && typeof val === 'object') {
        const key = val[props.valueKey];
        if (key !== undefined) {
            internalValue.value = key;
            return;
        }
    }

    internalValue.value = val;
});


watch(internalValue, (val) => {
    if (options.value && options.value.length) {
        const match = options.value.find(opt => {
            if (opt && typeof opt === 'object') {
                return (opt as Record<string, any>)[props.valueKey] === val;
            }
            return opt === val;
        });
        if (match !== undefined) {
            emit('update:modelValue', (typeof match === 'object') ? match : val);
            return;
        }
    }

    emit('update:modelValue', val);
});

async function loadItems() {
    if (typeof props.fetchFunction === 'function') {
        try {
            loading.value = true;
            const result = props.fetchFunction();
            const resolved = result instanceof Promise ? await result : result;

            if (Array.isArray(resolved)) {
                options.value = resolved;
            } else if (resolved && Array.isArray(resolved.data)) {
                options.value = resolved.data;
            } else if (resolved && Array.isArray(resolved.items)) {
                options.value = resolved.items;
            } else {
                console.warn('EntityDropdown: fetchFunction returned unexpected shape', resolved);
                options.value = [];
            }
        } catch (err) {
            console.error('EntityDropdown: error loading items', err);
            options.value = [];
        } finally {
            loading.value = false;
            applySelectedElement();
        }
    }
}

onMounted(() => {
    if (props.fetchOnMount && typeof props.fetchFunction === 'function') {
        loadItems();
    }
    applySelectedElement();
});

watch(() => props.selectedElement, () => applySelectedElement());

const inputId = computed(() => `entity-dropdown-${props.name.replace(/\s+/g, '-').toLowerCase()}`);

function selectOption(opt: any) {
    internalValue.value = (opt && typeof opt === 'object') ? opt[props.valueKey] : opt;
}

</script>

<template>
    <div class="form-field">
        <label class="label" :for="inputId">{{ name }}</label>

        <div>
            <input
                v-if="required"
                :id="inputId"
                type="text"
                :value="internalValue"
                required
                style="position: absolute; width: 0; height: 0; padding: 0; border: 0; opacity: 0; pointer-events: none;"
                aria-hidden="true"
            />

            <sl-dropdown :disabled="!enabled || loading" hoist>
                <sl-button slot="trigger" caret variant="default" type="button">
                    {{ displayLabel }}
                </sl-button>

                <sl-menu>
                    <sl-menu-item
                        v-for="(opt, idx) in options"
                        :key="idx + '-' + (opt && (opt as Record<string, any>)[valueKey] !== undefined ? (opt as Record<string, any>)[valueKey] : opt)"
                        @click="() => selectOption(opt)"
                    >
                        {{ (opt && typeof opt === 'object') ? (opt as Record<string, any>)[labelKey] : opt }}
                    </sl-menu-item>
                </sl-menu>
            </sl-dropdown>

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
