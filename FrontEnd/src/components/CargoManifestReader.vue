<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';
import type { CargoManifestItem, Container, Position } from '@/model/dto/VesselVisitNotificationDto';
import { useAlerts } from '@/composables/alerts';
import { container } from '@/inversify.config';
import type { IStorageAreaService } from '@/service/IService/IStorageAreaService';
import ObjectSelector from '@/components/crud/ObjectSelector.vue';
import TYPES from '@/inversify/types';

const storageAreaService = container.get<IStorageAreaService>(TYPES.storageAreaService);
const fetchStorageAreas = async () => {
    return storageAreaService.getStorageAreas({
        pageNumber: 1,
        pageSize: 1000,
        filter: {
            nameCode: ''
        }
    });
};

const props = defineProps({
  modelValue: {
    type: Array as () => CargoManifestItem[],
    default: () => []
  }
});

const notifications = useAlerts();

const emit = defineEmits(['update:modelValue']);

const newItem = ref<CargoManifestItem>({
    position: { bay: '', row: '', tier: '' },
    storageAreaCode: '',
    container: {
        containerNumber: '',
        cargoType: 0,
        description: ''
    }
});

const addContainer = () => {
    if (
        !newItem.value.container.containerNumber || !newItem.value.storageAreaCode || !newItem.value.container.cargoType ||
        !newItem.value.position.bay || !newItem.value.position.row || !newItem.value.position.tier
    ) {
        notifications.enqueueNotification(
            'Container fields cannot be empty. Please fill in all required fields.',
            'warning'
        );
        return;
    }

    newItem.value.storageAreaCode = String(newItem.value.storageAreaCode.nameCode);
    newItem.value.container.cargoType = Number(newItem.value.container.cargoType.cargoType);

    const updated = [...props.modelValue, JSON.parse(JSON.stringify(newItem.value))];
    emit('update:modelValue', updated);

    // Reset form
    newItem.value = {
        position: { bay: '', row: '', tier: '' },
        storageAreaCode: '',
        container: {
            containerNumber: '',
            cargoType: 0,
            description: ''
        }
    };
};

const removeContainer = (index: number) => {
    const updated = [...props.modelValue];
    updated.splice(index, 1);
    emit('update:modelValue', updated);
};

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

const pad2 = (value: string | number) => {
  const num = String(value).replace(/\D/g, '');
  return num.padStart(2, '0');
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
    
    try {
        const items: CargoManifestItem[] = [];
        const lines = text.split('\n');
        // skip header line
        lines.shift();
    
        for (let line of lines) {
            
            const row = line.split(';');
            const item = {
                position: {
                    bay: row[0].toString() || '',
                    row: row[1].toString() || '',
                    tier: row[2].toString() || ''
                } as Position,
                    storageAreaCode: row[3].toString() || '',
                    container: {
                        containerNumber: row[4].toString() || '',
                        cargoType: Number(row[5].toString() || 0),
                        description: row[6].toString() || ''
                    } as Container
            };
    
            items.push(item);
        }
        
        emit('update:modelValue', items);
        
    } catch (error) {
        
        notifications.enqueueNotification(
            'Error parsing CSV file. Please ensure the file format is correct.',
            'danger'
        );
    }
};

const openFileDialog = () => fileInput.value?.click();

const handleDragOver = (event: DragEvent) => {
  event.preventDefault();
  isDragging.value = true;
};

const handleDragLeave = () => isDragging.value = false;

const shortenedList = computed(() => {
  return props.modelValue.slice(0, expand.value ? props.modelValue.length : 5);
});

const expand = ref(false);
const toggleExpand = () => {
  expand.value = !expand.value;
};

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
        <li v-for="(item, index) in shortenedList" :key="index" class="list-item">
            <span>
              {{ item.container.containerNumber }},
              {{ types[item.container.cargoType] }}
              ({{ item.position.bay }}/{{ item.position.row }}/{{ item.position.tier }})
            </span>
          
            <!-- <sl-button
              size="small"
              variant="danger"
              @click="removeContainer(index)"
            >
              
            </sl-button> -->
            <sl-icon v-if="!(!expand && index === 4 && props.modelValue.length > 5)" name="trash" style="cursor: pointer; color: var(--sl-color-danger-600);" @click="removeContainer(index)"></sl-icon>
          
            <p v-if="!expand && index === 4 && props.modelValue.length > 5">
              ...and {{ props.modelValue.length - 5 }} more items.
            </p>
          </li>          
      </ul>

        <sl-button size="small" variant="default" @click="toggleExpand" v-if="props.modelValue.length > 5">
            {{ expand ? 'Show Less' : 'Show All' }}
        </sl-button>
    </div>

    <div class="manual-add">
        <p><strong>Add Container</strong></p>
      
        <div class="grid-form">
          <input v-model="newItem.container.containerNumber" placeholder="Container Number" />
            
            <ObjectSelector
                class="field-dropdown"
                v-model="newItem.storageAreaCode"
                :fetch-function="fetchStorageAreas"
                placeholderText="Select Storage Area..."
                labelKey="nameCode"
            />
      
          <input v-model="newItem.container.description" placeholder="Description" />
      
            <sl-input
                label="Bay"
                v-model="newItem.position.bay"
                @blur="newItem.position.bay = pad2(newItem.position.bay)"
                placeholder="Bay"
            />

            <sl-input
                label="Row"
                v-model="newItem.position.row"
                @blur="newItem.position.row = pad2(newItem.position.row)"
                placeholder="Row"
            />

            <sl-input
                label="Tier"
                v-model="newItem.position.tier"
                @blur="newItem.position.tier = pad2(newItem.position.tier)"
                placeholder="Tier"
            />

            
            <ObjectSelector
                class="field-dropdown"
                name="Cargo Type"
                v-model="newItem.container.cargoType"
                :fetch-function="() => {
                    return {
                        pageNumber: 1,
                        pageSize: 100,
                        pageCount: 1,
                        items: Object.entries(types).map(([key, value]) => ({
                            cargoType: Number(key),
                            nameCode: value
                        })),
                    }
                }"
                placeholderText="Select Storage Area..."
                labelKey="nameCode"
            />


        </div>
      
        <sl-button variant="primary" @click="addContainer">
          Add Container
        </sl-button>
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
.manual-add {
    margin-top: 1.5rem;
    padding-top: 1rem;
    border-top: 1px solid var(--sl-color-neutral-200);
}

.grid-form {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 0.5rem;
    margin-bottom: 0.75rem;
}

.grid-form input,
.grid-form select {
    padding: 0.4rem;
    border: 1px solid var(--sl-color-neutral-300);
    border-radius: 4px;
}

.list-item {
    display: flex;
    justify-content: space-between;
    align-items: center;
}  

</style>
