export interface ScheduleDataDto {
	data : {
		dock: string,
		schedule : {
			name: string,
			cranes: string[],
			unloading_enter_time: number,
			unloading_exit_time: number,
			loading_enter_time: number,
			loading_exit_time: number,
		}[],
	}[],
	date: string,
	metrics: {
		algorithm: string,
		computationTime: number,
		selection: {
			auto: boolean,
			reason: string,
		},
		strategy: string,
		totalDelay: number,
		vesselCount: number,
	}[]
}