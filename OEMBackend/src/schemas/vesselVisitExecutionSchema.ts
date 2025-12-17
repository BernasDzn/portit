import mongoose from "mongoose";
import { OperationSchema } from "./operationPlanSchema";

const OperationWithStatusSchema = new mongoose.Schema({

    operation: {
        type: OperationSchema,
        required: true        
    },
    status: {
        type: String,
        required: true,
        enum: ['Pending', 'InProgress', 'Completed', 'Failed'],
        default: 'Pending'
    }
});

const VesselVisitExecutionSchema = new mongoose.Schema({
    dock: {
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
    }
}, {
    timestamps: true
});

export const VesselVisitExecutionModel = mongoose.model('VesselVisitExecution', VesselVisitExecutionSchema);