<script setup lang="ts">
import { useAlerts } from '@/composables/alerts';
import { onMounted, ref, watch } from 'vue';

const props = defineProps({
    name: { type: String, required: true },
    placeholderText: { type: String, default: 'Select an option' },
    
    // The model is always an object, want it other way? map it i dont give a fuck
    modelValue: { type: [Object], default: {} },
    enabled: { type: Boolean, default: true },
    
    // Fetching is always done via function, want a static list? pass a function that returns it I dont give a fuck
    fetchFunction: { type: Function, default: null },
    fetchOnMount: { type: Boolean, default: false },
    labelKey: { type: String, default: 'name' },
    
    required: { type: Boolean, default: false },
    multiple: { type: Boolean, default: false },

    inputId: { type: String, default: '' },
});

const emit = defineEmits(['update:modelValue']);
const loading = ref(false); 
const options = ref([]); // The select options
const notifications = useAlerts();

// The value bound to the select component
const internalValue = ref<Array<Object> | Object | null>(props.multiple ? [] : null);

// Keys represent an object in string form in our case
// Since this time we're trying to store objects directly
// We'll hash the object to make a key
const makeKey = (opt: Object) => {
    return btoa(JSON.stringify(opt));
};

const applyModelValueToInternal = () => {
    console.log('Applying modelValue to internalValue:', props.modelValue);
    if (props.multiple)
        internalValue.value = (props.modelValue as any[]).map((obj) => makeKey(obj));
    else
        internalValue.value = props.modelValue ? makeKey(props.modelValue) : null;
}

onMounted(async () => {

    // Get options via fetch function
    if (props.fetchFunction) {
   
        loading.value = true;
        try {
            let fetched = await props.fetchFunction();

            // Check if paged result
            if (fetched.items) 
                fetched = fetched.items;

            options.value = Array.isArray(fetched) ? fetched : [];

        } catch (error) {
            console.error('Failed to fetch options for ObjectSelector:', error);
            notifications.enqueueNotification(
                'Failed to load options. Please try again later.',
                notifications.notificationTypes.DANGER
            );
        } finally {
            loading.value = false;
        }

        // Fill internalValue based on modelValue if requested
        if (props.fetchOnMount) {
            applyModelValueToInternal();
        }
    }
});

const onChange = (event: any) => {
    const value = event.target.value || event.currentTarget.value; // This comes from shoelace

    if (props.multiple) {

        const selectedKeys: string[] = value;
        //console.log('Selected keys:', selectedKeys.);
        const selectedObjects = selectedKeys
            .map(key => options.value.find(opt => makeKey(opt) === key))
            .filter(Boolean);

        emit('update:modelValue', selectedObjects);
    } else {
        const selectedKey = value;
        const selectedObject = options.value.find(opt => makeKey(opt) === selectedKey);
        emit('update:modelValue', selectedObject || null);
    }
};

// Sync internalValue when modelValue changes
watch(() => props.modelValue, (newVal) => {
    console.log('modelValue changed:', newVal);
    if (props.multiple)
        internalValue.value = newVal ? (newVal as any[]).map((obj) => makeKey(obj)) : [];
    else 
        internalValue.value = newVal ? makeKey(newVal) : null;
}, { immediate: true });

</script>

<template>
    <div class="form-field">
        <label class="label">{{ name }}</label>

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
                hoist
            >
                <sl-option
                    v-for="opt in options"
                    :key="makeKey(opt)"
                    :value="makeKey(opt)"
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