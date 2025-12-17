import mongoose from "mongoose";

const ExecutionOperationSchema = new mongoose.Schema({
    operationType: {
        type: mongoose.Schema.Types.ObjectId,
        ref: "TaskCategory",
        required: false // :(
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
        type: [{
            name: { type: String, required: true },
            type: { type: String, required: true },
            startTime: { type: Date, required: false },
            endTime: { type: Date, required: false }
        }],
        required: true,
        default: []
    },
    payload: {
        type: mongoose.Schema.Types.Mixed,
        required: false
    }
});

const OperationWithStatusSchema = new mongoose.Schema({

    operation: {
        type: ExecutionOperationSchema,
        required: true        
    },
    status: {
        type: String,
        required: true,
        enum: ['Pending', 'InProgress', 'Completed', 'Failed'],
        default: 'Pending'
    }
}, {
    _id: false
});

const VesselVisitExecutionSchema = new mongoose.Schema({
    code: {
        type: String,
        required: true
    },
    relatedVVN: {
        type: String,
        required: true
    },
    operationsExecuted: {
        type: [OperationWithStatusSchema],
        required: true,
        default: []
    },
    dateOpen: {
        type: Date,
        required: false
    },
    dateClosed: {
        type: Date,
        required: false
    },
    status: {
        type: String,
        required: true,
        enum: ['Open', 'Closed'],
        default: 'Open'
    },
    createdBy: {
        type: String,
        required: true
    }
}, {
    timestamps: true
});

export const VesselVisitExecutionModel = mongoose.model('VesselVisitExecution', VesselVisitExecutionSchema);