import mongoose from "mongoose";

const ScheduleSchema = new mongoose.Schema({
    cranes: [String],
    loadingEnterTime: Date,
    loadingLeaveTime: Date,
    vvnId: String,
});

const DockPlanSchema = new mongoose.Schema({
    dockId: String,
    schedule: [ScheduleSchema],
});

const MetricSchema = new mongoose.Schema({
    algorithm: {
        type: String,
        enum: ['optimal', 'greedy', 'genetic'],
    },
    computationTime: Number,
    strategy: String,
    totalDelay: Number,
    vesselCount: Number,
});

const OperationPlansSchema = new mongoose.Schema({
    dockPlanMap: [DockPlanSchema],
    metrics: [MetricSchema],
});

export const OperationPlans = mongoose.model('OperationPlans', OperationPlansSchema);