<script setup lang="ts">
import { computed, ref } from 'vue';
import type { CargoManifestItem, Container, Position } from '@/model/dto/VesselVisitNotificationDto';

const props = defineProps({
  modelValue: {
    type: Array as () => CargoManifestItem[],
    default: () => []
  }
});

const emit = defineEmits(['update:modelValue']);

const fileInput = ref<HTMLInputElement | null>(null);
const isDragging = ref(false);

const handleDrop = (event: DragEvent) => {
  event.preventDefault();
  isDragging.value = false;
  if (event.dataTransfer?.files) handleFiles(event.dataTransfer.files);
};

const handleFiles = (files: FileList) => {
  if (files.length === 0) return;
  const file = files[0];
  const reader = new FileReader();

  reader.onload = () => {
    const text = reader.result as string;
    parseCSV(text);
  };
  reader.readAsText(file);
};

const types: Record<number, string> = {
    0: 'Refrigerated Goods',
    1: 'General consumer goods',
    2: 'Electronics',
    3: 'Hazmat',
    4: 'Oversized industrial equipment',
    5: 'Other',
};

const parseCSV = (text: string) => {
    console.log('CSV content:', text);

    const items: CargoManifestItem[] = [];
    const lines = text.split('\n');
    // skip header line
    lines.shift();

    for (let line of lines) {
        
        const row = line.split(';');
        const item = {
            position: {
                bay: row[0] || '',
                row: row[1] || '',
                tier: row[2] || ''
            } as Position,
                storageAreaCode: row[3] || '',
                container: {
                containerNumber: row[4] || '',
                cargoType: Number(row[5] || 0),
                description: row[6] || ''
            } as Container
        };

        items.push(item);
    }
  
    emit('update:modelValue', items);
};

const openFileDialog = () => fileInput.value?.click();

const handleDragOver = (event: DragEvent) => {
  event.preventDefault();
  isDragging.value = true;
};

const handleDragLeave = () => isDragging.value = false;

const shortenedList = computed(() => {
  return props.modelValue.slice(0, 5);
});

</script>

<template>
  <sl-card>
    <div
      class="box"
      :class="{ dragging: isDragging }"
      @drop="handleDrop"
      @dragover="handleDragOver"
      @dragleave="handleDragLeave"
      @click="openFileDialog"
    >
      <p class="hint">Drop a cargo manifest CSV here or click to select</p>
      <input ref="fileInput" type="file" accept=".csv" class="hidden" @change="e => handleFiles((e.target as HTMLInputElement).files!)" />
    </div>

    <div v-if="props.modelValue.length" class="list">
      <p>Loaded Cargo Items:</p>
      <ul>
        <li v-for="(item, index) in shortenedList" :key="index">
          {{ item.container.containerNumber }}, {{ types[item.container.cargoType] }} ({{ item.position.bay }}/{{ item.position.row }}/{{ item.position.tier }})

            <p v-if="index === 4 && props.modelValue.length > 5">
                ...and {{ props.modelValue.length - 5 }} more items.
            </p>
        </li>
      </ul>
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

.box.dragging {
  background-color: var(--sl-color-primary-50);
  border-color: var(--sl-color-primary-700);
}

.hint {
  color: var(--sl-color-gray-600);
  font-size: 1.1em;
}

.hidden {
  display: none;
}

.list {
  margin-top: 1rem;
  font-size: 0.95rem;
}
</style>
