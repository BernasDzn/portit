import mongoose from "mongoose";

const OperationPlanMetadataSchema = new mongoose.Schema({
    createdBy: {
        type: String,
        required: true
    },
    createdAt: {
        type: Date,
        required: true
    },
    algorithmUsed: {
        type: String,
        required: true
    }
});

const ResourceSchema = new mongoose.Schema({
    name: {
        type: String,
        required: true
    },
    type: {
        type: String,
        required: true
    }
});

const OperationSchema = new mongoose.Schema({
    operationType: {
        type: mongoose.Schema.Types.ObjectId,
        ref: "TaskCategory",
        required: true
    },
    startTime: {
        type: Date,
        required: true
    },
    endTime: {
        type: Date,
        required: true
    },
    resources: {
        type: [ResourceSchema],
        required: true,
        default: []
    }
});

const OperationPlanSchema = new mongoose.Schema({
    dock: {
        type: String,
        required: true
    },
    relatedVVN: {
        type: String,
        required: true
    },
    operationSchedule: {
        type: [OperationSchema],
        required: true,
        default: []
    },
    metadata: {
        type: OperationPlanMetadataSchema,
        required: true
    }
}, {
    timestamps: true
});

export const OperationPlanModel = mongoose.model('OperationPlans', OperationPlanSchema);