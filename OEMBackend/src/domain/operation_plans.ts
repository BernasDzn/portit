import mongoose from "mongoose";

const OperationPlansSchema = new mongoose.Schema({
    // The list of plans associated with each dock
    dockPlanMap: Array<{
        // The dock at which the plans are to be executed
        dockId: string;
        // The timeline of plans for the dock
        schedule: Array<{
            // The cranes assigned for the plan
            cranes: Array<string>;
            loadingEnterTime: Date;
            loadingLeaveTime: Date;
            vvnId: string;
        }>;
    }>,
    metrics: Array<{
        algorithm: 'optimal' | 'greedy' | 'genetic';
        computationTime: number;
        streategy: string;
        totalDelay: number;
        vesselCount: number;
    }>,
});

export const OperationPlans = mongoose.model('OperationPlans', OperationPlansSchema);