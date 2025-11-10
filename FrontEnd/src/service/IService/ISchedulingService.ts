import type { Filter, Page } from "@/model/Page";
import type { Representative } from "@/model/Representative";
import type { Schedule } from "@/model/Schedule";

export interface ISchedulingService {
    scheduleForDay(day: Date): Promise<Schedule>
}