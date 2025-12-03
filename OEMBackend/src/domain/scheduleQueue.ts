import { WorkQueueItem } from "./workQueueItem";

export class ScheduleQueueItem {
    
    public id: string;
    public data: WorkQueueItem;
    public priority: number;
    public requestedAt: Date;
    public status: string;
    public issuer: string;
    public estimatedStartTime: Date | null;
    public estimatedEndTime: Date | null;

    constructor(
        params: { 
            id: string;
            data: WorkQueueItem; 
            priority: number; 
            requestedAt: Date; 
            status: string; 
            issuer: string; 
            estimatedStartTime?: Date | null; 
            estimatedEndTime?: Date | null; 
        }
    ) {
        this.id = params.id;
        this.data = params.data;
        this.priority = params.priority;
        this.requestedAt = params.requestedAt;
        this.status = params.status;
        this.issuer = params.issuer;
        this.estimatedStartTime = params.estimatedStartTime || null;
        this.estimatedEndTime = params.estimatedEndTime || null;
    }
}