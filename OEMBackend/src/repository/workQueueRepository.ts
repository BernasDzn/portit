import { WorkQueueItem } from "../domain/workQueueItem";
import { ScheduleQueue } from "../schemas/scheduleQueue";
import { ScheduleQueueItem } from "../domain/scheduleQueue";
import { ScheduleQueueMapper } from "../mappers/scheduleQueueMapper";

export class WorkQueueRepository {

    async enqueueRequest(day: string, alg: string, daysAhead: number = 2, priority: number = 0, issuer: string): Promise<number> {
        const newRequest = new WorkQueueItem({
            day,
            alg,
            daysAhead,
        });

        const workQueueEntry = new ScheduleQueue({
            requestData: newRequest,
            priority: priority,
            requestedAt: new Date(),
            status: 'pending',
            estimatedStartTime: null,
            estimatedEndTime: null,
            issuer: issuer,
        });

        await workQueueEntry.save();

        const numberOnQueue = await ScheduleQueue.countDocuments({
            status: 'pending',
            requestedAt: { $lt: workQueueEntry.requestedAt }
        }).exec();

        return numberOnQueue;
    }

    async getById(id: string): Promise<ScheduleQueueItem | null> {
        const entry = await ScheduleQueue.findById(id).exec();
        if (!entry)
            return null;
        return ScheduleQueueMapper.fromSchema(entry);
    }

    async getQueueState(): Promise<ScheduleQueueItem[]> {
        const queueEntries = await ScheduleQueue.find().sort({ priority: -1, requestedAt: -1 }).limit(10).exec();
        return queueEntries.map(entry => ScheduleQueueMapper.fromSchema(entry));
    }

    async dequeueRequest(): Promise<ScheduleQueueItem | null> {

        const nextEntry = await ScheduleQueue.findOneAndUpdate(
            { status: 'pending' },
            { status: 'in-progress' },
            { sort: { priority: -1, requestedAt: 1 }, new: true }
        ).exec();
        
        if (!nextEntry)
            return null;

        return ScheduleQueueMapper.fromSchema(nextEntry);
    }

    async finishRequest(id: string, status: string, resul: any = null): Promise<void> {

        const update: any = {
            status: status,
            result: resul,
        };

        await ScheduleQueue.findByIdAndUpdate(
            id,
            update,
            { new: true }
        ).exec();
    }
}

export const workQueueRepository = new WorkQueueRepository();