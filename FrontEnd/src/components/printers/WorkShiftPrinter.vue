<script setup lang="ts">
import { onMounted, ref } from 'vue'
import type { OperationalWindow, Shift } from '@/model/OperationalWindow';
import { useI18n } from 'vue-i18n';

const props = defineProps<{
    op_window?: OperationalWindow;
    shift_record?: Record<number, Shift[]>;
    is_removable?: boolean,
}>();

const {t} = useI18n();

const emits = defineEmits(['removeShift'])

let shiftsPerDay = ref<Record<number, Shift[]>>({});

const weekDays = ["common.days.sunday", "common.days.monday", "common.days.tuesday", "common.days.wednesday", "common.days.thursday", "common.days.friday", "common.days.saturday"];

onMounted(
    () => {
        if(props.shift_record != null) 
            shiftsPerDay.value = props.shift_record as Record<number, Shift[]>;

        if(props.op_window != null){
            for(let shift of props.op_window.shifts){
                if(!shiftsPerDay.value[shift.day]) {
                    shiftsPerDay.value[shift.day] = [];
                }
                shiftsPerDay.value[shift.day]?.push(shift);
            }
        }
    }
)

function FormatTime(qwertyuiop:string){
    return qwertyuiop.split(":")[0] + ":" + qwertyuiop.split(":")[1];
}

</script>

<template>
	<div v-if="Object.keys(shiftsPerDay).length > 0" class="shift-flex">
		<div v-for="(shifts, day) in shiftsPerDay" :key="day">
			<sl-card v-if="shifts.length > 0" class="shift-flex
				">
				<div slot="header">
					{{t( weekDays[day]! )}}
				</div>
				<div class="tags-removable shift-flex-column">
					<sl-tag v-for="(shift, idx) in shifts" :key="idx" variant="neutral" pill :removable="is_removable == undefined || is_removable" @sl-remove="$emit('removeShift', Number(day), idx)">
						{{ FormatTime(shift.startTime) }} <span class="shift-arrow"> -> </span> {{ FormatTime(shift.endTime) }}
					</sl-tag>
				</div>
			</sl-card>
		</div>
	</div>
	<div v-else>
		<p style="color:gray;">
			{{ t('operationalWindow.printer.noShifts') }}
		</p>
	</div>
</template>