// export interface WorkQueueItem {
//     day: String,
//     alg: String,
//     daysAhead: Number,
// }

export class WorkQueueItem {
    
    public day: string;
    public alg: string;
    public daysAhead: number;

    constructor(
        params: { day: string; alg: string; daysAhead: number }
    ) {
        this.day = params.day;
        this.alg = params.alg;
        this.daysAhead = params.daysAhead;
    }
}