import type { Filter, Page } from "@/model/Page";
import type { Representative } from "@/model/Representative";
import type { Schedule } from "@/model/values/Schedule";

export interface ISchedulingService {
    scheduleForDay(day: Date, alg: string, daysAhead: number): Promise<any>
    generateSchedulePDF(schedule: Schedule, date: Date): Promise<Uint8Array>

    getQueueState(): Promise<any[]>
    getUnplannedVVNs(): Promise<string[]>

    acceptSchedulingRequest(id: string): Promise<any>;
    rejectSchedulingRequest(id: string): Promise<any>;
}
