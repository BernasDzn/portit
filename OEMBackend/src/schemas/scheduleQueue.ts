import mongoose from "mongoose";

export const WorkQueueItemSchema = new mongoose.Schema({
    day: String,
    alg: String,
    daysAhead: Number,
}, { _id: false });

const ScheduleResultSchema = new mongoose.Schema({
    data: [
        {
            dock: String,
            schedule: [
                {
                    name: String,
                    cranes: [String],
                    unloading_enter_time: Number,
                    unloading_exit_time: Number,
                    loading_enter_time: Number,
                    loading_exit_time: Number,
                }
            ]
        }
    ],
    date: String,
    metrics: [
        {
            algorithm: String,
            computationTime: Number,
            selection: {
                auto: Boolean,
                reason: String
            },
            strategy: String,
            totalDelay: Number,
            vesselCount: Number,
        }
    ]
}, { _id: false });

const WorkQueueSchema = new mongoose.Schema({
    requestData: WorkQueueItemSchema,
    result: {
        type: ScheduleResultSchema,
        default: null,
    },
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