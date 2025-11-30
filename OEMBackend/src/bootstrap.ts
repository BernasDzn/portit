import mongoose from 'mongoose';
import { OperationPlans } from './domain/operation_plans';

export const bootstrap = async () => {
    await bootstrapOperationPlans();
}

const bootstrapOperationPlans = async () => {
    try {
        const samplePlan = new OperationPlans({
            dockPlanMap: [{
                dockId: "dock-1",
                schedule: [{
                    cranes: ["crane-1", "crane-2"],
                    loadingEnterTime: new Date("2024-01-15T08:00:00"),
                    loadingLeaveTime: new Date("2024-01-15T12:00:00"),
                    vvnId: "vessel-001"
                }]
            }],
            metrics: [{
                algorithm: "optimal",
                computationTime: 1200,
                strategy: "first-fit",
                totalDelay: 0,
                vesselCount: 3
            }]
        });

        await samplePlan.save();
    } catch (error) {
        console.error("Error bootstrapping operation plans:", error);
    }
};