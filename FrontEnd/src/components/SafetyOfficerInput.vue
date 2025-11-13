<script setup lang="ts">
import type { Person } from '@/model/dto/VesselVisitNotificationDto';
import { ref, watch } from 'vue';

const props = defineProps({
  modelValue: {
    type: Array as () => Person[],
    default: () => []
  }
});

const emit = defineEmits(['update:modelValue']);

const officers = ref<Person[]>([...props.modelValue]);

watch(() => props.modelValue, (val) => {
  officers.value = [...val];
}, { deep: true });

// form for adding new officer
const newOfficer = ref<Person>({
  name: '',
  citizenshipId: 0,
  emailAddress: '',
  phone: ''
});

const addOfficer = () => {
  if (!newOfficer.value.name) return; // minimal validation
  officers.value.push({ ...newOfficer.value });
  emit('update:modelValue', officers.value);
  newOfficer.value = { name: '', citizenshipId: 0, emailAddress: '', phone: '' };
};

const removeOfficer = (index: number) => {
  officers.value.splice(index, 1);
  emit('update:modelValue', officers.value);
};

const updateOfficer = (index: number, field: keyof Person, value: string | number) => {
  officers.value[index][field] = value as any;
  emit('update:modelValue', officers.value);
};
</script>

<template>
  <div class="safety-officers">

    <div class="officer-list">
      <div class="officer-item" v-for="(officer, index) in officers" :key="index">
        <sl-card>
            
            <p> {{ officer.name }} ({{ officer.citizenshipId }})</p>
            <p> Email: {{ officer.emailAddress }} </p>
            <p> Phone: {{ officer.phone }} </p>

            <sl-button variant="danger" size="small" @click="removeOfficer(index)">Remove</sl-button>
        </sl-card>
      </div>
    </div>

    <div class="officer-add">
      <sl-input
        placeholder="Name"
        v-model="newOfficer.name"
        filled
      />
      <sl-input
        placeholder="Email"
        v-model="newOfficer.emailAddress"
        filled
      />
      <sl-input
        placeholder="Phone"
        v-model="newOfficer.phone"
        filled
      />
      <sl-input
        type="number"
        placeholder="Citizenship ID"
        v-model.number="newOfficer.citizenshipId"
        filled
      />
      <sl-button variant="primary" @click="addOfficer">Add Officer</sl-button>
    </div>
  </div>
</template>

<style scoped>
.safety-officers {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.officer-list {
  display: flex;
  flex-direction: row;
  gap: 0.5rem;
}

.officer-item {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
  align-items: center;
}

.officer-add {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
  align-items: center;
}
</style>
