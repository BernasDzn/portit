import mongoose from "mongoose";

export const WorkQueueItemSchema = new mongoose.Schema({
    day: String,
    alg: String,
    daysAhead: Number,
});

const WorkQueueSchema = new mongoose.Schema({
    requestData: WorkQueueItemSchema,
    priority: {
        type: Number,
        default: 0,
    },
    requestedAt: {
        type: Date,
        default: Date.now,
    },
    status: {
        type: String,
        enum: ['pending', 'in-progress', 'completed', 'failed'],
        default: 'pending',
    },
    issuer: String,
    estimatedStartTime: Date,
    estimatedEndTime: Date,
});

export const ScheduleQueue = mongoose.model('ScheduleQueue', WorkQueueSchema);