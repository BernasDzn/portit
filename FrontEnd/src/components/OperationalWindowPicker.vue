<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue';
import type { OperationalWindow, Shift } from '@/model/OperationalWindow';
import WorkShiftPrinter from './printers/WorkShiftPrinter.vue';
import { useAlerts } from '@/composables/alerts';
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

const props = defineProps<{
    modelValue: OperationalWindow;
}>();

let shiftsPerDay = reactive<Record<number, Shift[]>>({});

const weekDays = [t("common.days.sunday"), t("common.days.monday"), t("common.days.tuesday"), t("common.days.wednesday"), t("common.days.thursday"), t("common.days.friday"), t("common.days.saturday")];
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
                    t('operationalWindow.errors.shiftOverlap'),
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

onMounted(() => {
    for(let shift of props.modelValue.shifts){
        if (!shiftsPerDay[shift.day]) {
            shiftsPerDay[shift.day] = [];
        }

        shiftsPerDay[shift.day]?.push(shift);
    }
})

</script>

<template>
	<div>
		<p>Operational Window:</p>
		<sl-card style="width: 100%;">
			<div>
				<sl-details :summary="t('operationalWindow.printer.addShift')" class="custom-icons">
					<sl-icon name="plus-square" slot="expand-icon"></sl-icon>
					<sl-icon name="dash-square" slot="collapse-icon"></sl-icon>
					<div style="display: flex; gap: 1rem; align-items: center;">
						<sl-button variant="default" size="medium" pill @click="addShift(selectedDays, startTime, endTime)">
							Add
						</sl-button>
						<sl-input :label="t('operationalWindow.printer.startTime')" type="time" placeholder="Number" v-model="startTime"></sl-input>
						<sl-input :label="t('operationalWindow.printer.endTime')" type="time" placeholder="Number" v-model="endTime"></sl-input>
						<sl-select id=multiSelect :label="t('operationalWindow.printer.weekDays')" placeholder="Weekday" multiple clearable hoist @sl-change="updateSelectedDays">
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