<template>
    <div class="calendar-with-events">
      <div class="header">
        <button @click="prevMonth">‹</button>
        <span>{{ monthName }} {{ year }}</span>
        <button @click="nextMonth">›</button>
      </div>
  
      <div class="grid">
        <div v-for="d in weekDays" :key="d" class="weekday">{{ d }}</div>
  
        <div
          v-for="(day, i) in days"
          :key="i"
          class="day-cell"
          :class="{
            empty: !day,
            selected: isSelected(day),
            hasEvent: hasEvent(day)
          }"
          @click="selectDate(day)"
        >
          <div class="date-num">{{ day }}</div>
  
          <div v-if="eventsForDay(day).length" class="events">
            <!-- <div
              v-for="(ev, idx) in eventsForDay(day)"
              :key="idx"
              class="event"
              :title="ev.title"
            >
              {{ ev.title }}
            </div> -->
          </div>
        </div>
      </div>
    </div>
</template>
  
<script setup>
  import { computed, ref, watch, onMounted } from "vue"
  
  const props = defineProps({
    modelValue: Date,
    events: {
      type: Array,
      default: () => []
    }
  })
  const emit = defineEmits(["update:modelValue", "month-change"])
  
    const current = ref(props.modelValue ? new Date(props.modelValue) : new Date())
    const selected = ref(props.modelValue || null)
  
    watch(
        () => props.modelValue,
        (val) => {
            if (val) selected.value = new Date(val)
        }
    )
  
    watch(selected, (val, oldVal) => {
        if (!oldVal || val.getTime() !== oldVal.getTime()) {
            emit("update:modelValue", val);
        }
    });
    
    const year = computed(() => current.value.getFullYear())
    const month = computed(() => current.value.getMonth())
    
    const weekDays = ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"]
    
    const monthName = computed(() =>
        current.value.toLocaleString("default", { month: "long" })
    )
    
    const days = computed(() => {
        const firstDay = new Date(year.value, month.value, 1).getDay()
        const totalDays = new Date(year.value, month.value + 1, 0).getDate()
        return [
        ...Array(firstDay).fill(null),
        ...Array.from({ length: totalDays }, (_, i) => i + 1)
        ]
    })
    
    function prevMonth() {
        current.value = new Date(year.value, month.value - 1, 1)
    }
    
    function nextMonth() {
        current.value = new Date(year.value, month.value + 1, 1)
    }
    
    function selectDate(day) {
        if (!day) return
        selected.value = new Date(year.value, month.value, day)
    }
    
    function isSelected(day) {
        if (!day || !selected.value) return false
        return (
            day === selected.value.getDate() &&
            month.value === selected.value.getMonth() &&
            year.value === selected.value.getFullYear()
        )
    }
    function hasEvent(day) {
        if (!day) return false
        const date = new Date(year.value, month.value, day)
        return props.events.some((e) => isStartDay(date, e))
    }

    function eventsForDay(day) {
        if (!day) return []
        const date = new Date(year.value, month.value, day)
        return props.events.filter((e) => isStartDay(date, e))
    }

    // Check if the date is exactly the start day of the event
    function isStartDay(date, event) {
        const start = new Date(event.start)
        start.setHours(0, 0, 0, 0)
        date.setHours(0, 0, 0, 0)
        return date.getTime() === start.getTime()
    }

    watch(
        () => [month.value, year.value],
        ([newMonth, newYear], [oldMonth, oldYear]) => {
            if (newMonth !== oldMonth || newYear !== oldYear) {
                emit("month-change", new Date(newYear, newMonth, 1))
            }
        }
    )

</script>
  
<style scoped>
.calendar-with-events {
    width: 320px;
    background: var(--sl-color-neutral-0);
    border-radius: var(--sl-border-radius-medium);
    padding: 0.75rem;
    box-shadow: var(--sl-shadow-small);
    display: flex;
    flex-direction: column;
}

.header {
    display: flex;
    justify-content: center;
    align-items: center;
    font-weight: 600;
    margin-bottom: 0.5rem;
    gap: 1rem;
    position: relative;
}

.header span {
    flex: 1;
    text-align: center;
}

.header button {
    all: unset;
    cursor: pointer;
    padding: 0 0.5rem;
    font-size: 1.2rem;
    position: absolute;
}

.header button:first-child {
    left: 0;
}

.header button:last-child {
    right: 0;
}

.grid {
    display: grid;
    grid-template-columns: repeat(7, 1fr);
    gap: 0.25rem;
    text-align: center;
    margin-top: 0.25rem;
}

.weekday {
    font-size: 0.8rem;
    opacity: 0.7;
}

.day-cell {
    min-height: 64px;
    background: var(--sl-color-neutral-50);
    border-radius: var(--sl-border-radius-small);
    position: relative;
    cursor: pointer;
    transition: background 0.2s;
    padding: 0.25rem;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    text-align: center;
}

.day-cell:hover {
    background: var(--sl-color-primary-100);
}

.day-cell.empty {
    visibility: hidden;
}

.day-cell.selected {
    border: 2px solid var(--sl-color-primary-600);
}

.day-cell.hasEvent {
    background: var(--sl-color-primary-200);
}

.date-num {
    font-weight: 600;
    font-size: 0.9rem;
}

.events {
    margin-top: 0.25rem;
    width: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
}

.event {
    background: var(--sl-color-primary-500);
    color: white;
    border-radius: var(--sl-border-radius-small);
    font-size: 0.65rem;
    padding: 0 0.2rem;
    text-overflow: ellipsis;
    overflow: hidden;
    white-space: nowrap;
    max-width: 90%;
}
  
</style>
  