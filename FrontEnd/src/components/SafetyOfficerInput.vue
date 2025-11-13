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
  citizenId: '',
  nationality: '',
});

const addOfficer = () => {
  if (!newOfficer.value.name) return; 
  officers.value.push({ ...newOfficer.value });
  emit('update:modelValue', officers.value);
  newOfficer.value = { name: '', citizenId: '', nationality: '' };
};

const removeOfficer = (index: number) => {
  officers.value.splice(index, 1);
  emit('update:modelValue', officers.value);
};

</script>

<template>
  <div class="safety-officers">

    <div class="officer-list">
      <div class="officer-item" v-for="(officer, index) in officers" :key="index">
        <sl-card>
            
            <p> {{ officer.name }} ({{ officer.citizenId }})</p>
            <p> Nationality: {{ officer.nationality }} </p>

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
        placeholder="Nationality"
        v-model="newOfficer.nationality"
        filled
      />
      <sl-input
        placeholder="Citizenship ID"
        v-model="newOfficer.citizenId"
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
