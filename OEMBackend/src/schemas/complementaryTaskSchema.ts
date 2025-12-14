import mongoose from "mongoose";

const ComplementaryTaskSchema = new mongoose.Schema({
    categoryId: {
        type: mongoose.Schema.Types.ObjectId,
        ref: "TaskCategory",
        required: true
    },
    responsibleTeam: {
        type: String,
        required: true
    },
    startTimestamp: {
        type: Date,
        required: true
    },
    endTimestamp: {
        type: Date,
        required: false
    },
    status: {
        type: String,
        enum: ['ongoing', 'completed', 'cancelled'],
        required: true,
        default: 'ongoing'
    },
    vesselVisitEventId: {
        type: String,
        required: true
    },
    impact: {
        type: String,
        enum: ['parallel', 'suspends_operations'],
        required: true
    },
    description: {
        type: String,
        required: false
    }
}, {
    timestamps: true
});

// Indexes for common queries
ComplementaryTaskSchema.index({ vesselVisitEventId: 1 });
ComplementaryTaskSchema.index({ status: 1 });
ComplementaryTaskSchema.index({ startTimestamp: 1 });
ComplementaryTaskSchema.index({ categoryId: 1 });

export default mongoose.model("ComplementaryTask", ComplementaryTaskSchema);
