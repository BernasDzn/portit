import type { Filter, Page } from "@/model/Page";
import type { Representative } from "@/model/Representative";
import type { Schedule } from "@/model/values/Schedule";

export interface ISchedulingService {
    scheduleForDay(day: Date, dock: string, alg: string, daysAhead: number): Promise<Schedule>
    generateSchedulePDF(schedule: Schedule, date: Date, dock: string): Promise<Uint8Array>
}