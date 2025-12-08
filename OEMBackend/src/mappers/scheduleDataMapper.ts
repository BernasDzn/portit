import { ScheduleDataDto } from "../dto/scheduleDataDto";

export class ScheduleDataMapper {

	static toDto(scheduleData: any): ScheduleDataDto {
		const dto: ScheduleDataDto = {
			data: scheduleData.data.map((dockData: any) => ({
				dock: dockData.dock,
				schedule: dockData.schedule.map((item: any) => ({
					name: item.name,
					cranes: item.cranes,
					unloading_enter_time: item.unloading_enter_time,
					unloading_exit_time: item.unloading_exit_time,
					loading_enter_time: item.loading_enter_time,
					loading_exit_time: item.loading_exit_time,
				})),
			})),
			date: scheduleData.date,
			metrics: scheduleData.metrics.map((metric: any) => ({
				algorithm: metric.algorithm,
				computationTime: metric.computationTime,
				selection: {
					auto: metric.selection.auto,
					reason: metric.selection.reason,
				},
				strategy: metric.strategy,
				totalDelay: metric.totalDelay,
				vesselCount: metric.vesselCount,
			})),
		};
		return dto;
	}

}