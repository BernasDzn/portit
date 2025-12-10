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

// A reousrce used is not restricted to physical resources, we should do an enum perhaps later
// But it could also be a staff that is allocated to perform the operation
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
    },
    // The payload of an operation can vary depending on the operation type
    // It carries additional data needed to execute the operation
    // For example a load operation can have a container ID as payload
    payload: {
        type: mongoose.Schema.Types.Mixed,
        required: true
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