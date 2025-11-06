<script setup lang="ts">
	import { onMounted, reactive, ref, watch } from 'vue';
	import type { OperationalWindow, Shift } from '@/model/OperationalWindow';
	import WorkShiftPrinter from './printers/WorkShiftPrinter.vue';
	import { useAlerts } from '@/composables/alerts';

	const props = defineProps<{
		modelValue: OperationalWindow;
	}>();

	let shiftsPerDay = reactive<Record<number, Shift[]>>({});

	const weekDays = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
	const selectedDays = ref<number[]>([]);
	const startTime = ref<string>('08:00');
	const endTime = ref<string>('13:00');
	let selectedDaysInput : HTMLSelectElement;

	const notification = useAlerts();

	function removeShift(day: number, indexInDay: number) {
		let dayShifts = shiftsPerDay[day];
		if (!dayShifts || indexInDay < 0 || indexInDay >= dayShifts.length) return;
		let modelShift = props.modelValue.shifts.find(
			shift => shift.day === day && 
			shift.startTime === dayShifts[indexInDay]?.startTime && 
			shift.endTime === dayShifts[indexInDay]?.endTime
		);

		dayShifts.splice(indexInDay, 1);
		if (dayShifts?.length == 0 || !dayShifts || dayShifts === null) {
			delete shiftsPerDay[day];
		}
		for(let i = 0; i<props.modelValue.shifts.length; i++){
			if(props.modelValue.shifts[i] === modelShift){
				props.modelValue.shifts.splice(i,1)
			}
		}
	}

	function invalid_shift(shift1: Shift, shift2: Shift): boolean {
		if(shift2.endTime < shift2.startTime){
			return true;
		}
		return !(shift1.endTime <= shift2.startTime || shift2.endTime <= shift1.startTime);
	}

	function addShift(day: number[], sTime: string, eTime: string) {
		(day).forEach(d => {
			const dd = Number(d);
			if (!shiftsPerDay[dd]) {
				shiftsPerDay[dd] = [];
			}
			for(let a_shift of shiftsPerDay[dd]){
				if(invalid_shift(a_shift, { day: dd, startTime: sTime, endTime: eTime })) {
					notification.enqueueNotification(
						'Invalid shift: overlapping or end time before start time.',
						notification.notificationTypes.DANGER,
					);
					return;
				}
			}
			shiftsPerDay[dd].push({ day: dd, startTime: sTime, endTime: eTime });
			props.modelValue.shifts.push(
				{ day: dd, startTime: sTime, endTime: eTime }
			);
		});
		selectedDays.value = [];
		selectedDaysInput.value = '';
	}

	function updateSelectedDays() {
		selectedDaysInput = document.querySelector("#multiSelect") as HTMLSelectElement;
		selectedDays.value = Array.from(selectedDaysInput.selectedOptions).map(option => Number(option.value));
	}

	function populateShiftsPerDay(shifts?: Shift[]) {
		// clear existing
		Object.keys(shiftsPerDay).forEach(k => delete shiftsPerDay[Number(k)])

		if (!shifts || shifts.length === 0) return

		for (const shift of shifts) {
			const arr = shiftsPerDay[shift.day] ?? (shiftsPerDay[shift.day] = [])
			arr.push(shift)
		}
	}

	watch(() => props.modelValue?.shifts, (newShifts) => {
		populateShiftsPerDay(newShifts as Shift[] | undefined)
	}, { immediate: true, deep: true })

</script>

<template>
	<div>
		<p>Operational Window:</p>
		<sl-card style="width: 100%;">
			<div>
				<sl-details summary="Add Shift" class="custom-icons">
					<sl-icon name="plus-square" slot="expand-icon"></sl-icon>
					<sl-icon name="dash-square" slot="collapse-icon"></sl-icon>
					<div style="display: flex; gap: 1rem; align-items: center;">
						<sl-button variant="default" size="medium" pill @click="addShift(selectedDays, startTime, endTime)">
							Add
						</sl-button>
						<sl-input label="Start Time" type="time" placeholder="Number" v-model="startTime"></sl-input>
						<sl-input label="End Time" type="time" placeholder="Number" v-model="endTime"></sl-input>
						<sl-select id=multiSelect label="Weekday" placeholder="Weekday" multiple clearable hoist @sl-change="updateSelectedDays">
							<sl-option v-for="(weekday, idx) in weekDays" :key="idx" :value="idx">{{ weekday }}</sl-option>
						</sl-select>
					</div>
				</sl-details>
				<p>Shifts:</p>
				<WorkShiftPrinter
					:shift_record="shiftsPerDay"
					:is_removable="true"
					@remove-shift="removeShift"
				/>
			</div>
		</sl-card>

	</div>
</template>