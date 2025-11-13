<script setup>
import { ref, computed, watch, onMounted } from 'vue'

const modelValue = defineModel() 

onMounted(() => {
  if (!window.ShoelaceLoaded) {
    const script = document.createElement('script')
    script.type = 'module'
    script.src = 'https://cdn.jsdelivr.net/npm/@shoelace-style/shoelace@2.15.0/dist/shoelace.js'
    document.head.appendChild(script)
    window.ShoelaceLoaded = true
  }
})

const selected = ref(modelValue.value ? new Date(modelValue.value) : null)
const showCalendar = ref(false)
const current = ref(selected.value ? new Date(selected.value) : new Date())

watch(modelValue, (newVal) => {
  if (newVal) selected.value = new Date(newVal)
})

const year = computed(() => current.value.getFullYear())
const month = computed(() => current.value.getMonth())

const weekDays = ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa']

const monthName = computed(() =>
  current.value.toLocaleString('default', { month: 'long' })
)

const days = computed(() => {
  const first = new Date(year.value, month.value, 1).getDay()
  const total = new Date(year.value, month.value + 1, 0).getDate()
  const blanks = Array(first).fill(null)
  const nums = Array.from({ length: total }, (_, i) => i + 1)
  return [...blanks, ...nums]
})

const formattedDate = computed(() =>
  selected.value ? selected.value.toLocaleDateString() : ''
)

function toggleCalendar() {
  showCalendar.value = !showCalendar.value
}

function prevMonth() {
  current.value = new Date(year.value, month.value - 1, 1)
}

function nextMonth() {
  current.value = new Date(year.value, month.value + 1, 1)
}

function selectDate(day) {
  if (!day) return
  const date = new Date(year.value, month.value, day)
  selected.value = date
  modelValue.value = date
  showCalendar.value = false
}

function isSelected(day) {
  return (
    selected.value &&
    day === selected.value.getDate() &&
    month.value === selected.value.getMonth() &&
    year.value === selected.value.getFullYear()
  )
}
</script>

<template>
    <div class="date-picker">
      <sl-input
        readonly
        placeholder="Click to pick"
        :value="formattedDate"
        @click="toggleCalendar"
      >
        <sl-icon name="calendar" slot="suffix"></sl-icon>
      </sl-input>
  
      <div v-if="showCalendar" class="calendar">
        <div class="calendar-header">
          <button @click="prevMonth">‹</button>
          <span>{{ monthName }} {{ year }}</span>
          <button @click="nextMonth">›</button>
        </div>
  
        <div class="calendar-grid">
          <div v-for="d in weekDays" :key="d" class="day-name">{{ d }}</div>
          <div
            v-for="(d, i) in days"
            :key="i"
            class="day"
            :class="{ empty: !d, selected: isSelected(d) }"
            @click="selectDate(d)"
          >
            {{ d }}
          </div>
        </div>
      </div>
    </div>
  </template>
  
  <style scoped>
  .date-picker {
    position: relative;
    width: 220px;
  }
  
  .calendar {
    position: absolute;
    top: 100%;
    left: 0;
    z-index: 10;
    background: var(--sl-panel-background-color);
    border: 1px solid var(--sl-color-neutral-200);
    border-radius: var(--sl-border-radius-medium);
    box-shadow: var(--sl-shadow-x-small);
    padding: 0.5rem;
    width: 100%;
  }
  
  .calendar-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    font-weight: 600;
    margin-bottom: 0.25rem;
  }
  
  .calendar-header button {
    all: unset;
    cursor: pointer;
    padding: 0 0.5rem;
    font-size: 1.2rem;
  }
  
  .calendar-grid {
    display: grid;
    grid-template-columns: repeat(7, 1fr);
    text-align: center;
    gap: 0.25rem;
  }
  
  .day-name {
    font-size: 0.8rem;
    opacity: 0.7;
  }
  
  .day {
    cursor: pointer;
    padding: 0.3rem 0;
    border-radius: var(--sl-border-radius-small);
    transition: background 0.2s;
  }
  
  .day:hover {
    background: var(--sl-color-primary-100);
  }
  
  .empty {
    visibility: hidden;
  }
  
  .selected {
    background: var(--sl-color-primary-600);
    color: white;
  }
  </style>
  