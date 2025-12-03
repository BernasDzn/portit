import { ScheduleQueueItem } from "../scheduleQueue";
import { WorkQueueItem } from "../workQueueItem";

export class ScheduleQueueMapper {

    static fromSchema(doc: any): ScheduleQueueItem {
        return new ScheduleQueueItem({
            id: doc._id.toString(),
            data: new WorkQueueItem({
                day: doc.requestData?.day,
                alg: doc.requestData?.alg,
                daysAhead: doc.requestData?.daysAhead,
            }),
            priority: doc.priority ?? 0,
            requestedAt: new Date(doc.requestedAt),
            status: doc.status,
            issuer: doc.issuer,
            estimatedStartTime: doc.estimatedStartTime
                ? new Date(doc.estimatedStartTime)
                : null,
            estimatedEndTime: doc.estimatedEndTime
                ? new Date(doc.estimatedEndTime)
                : null,
        });
    }
}
