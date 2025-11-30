import mongoose from 'mongoose';
import { OperationPlans } from './schemas/operationPlansSchema';

export const bootstrap = async () => {
    await bootstrapOperationPlans();
}

const bootstrapOperationPlans = async () => {
    try {
        const samplePlan = new OperationPlans({
            date: new Date("2024-01-15"),
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

        const samplePlan2 = new OperationPlans({
            date: new Date("2024-01-16"),
            dockPlanMap: [{
                dockId: "dock-2",
                schedule: [{
                    cranes: ["crane-3"],
                    loadingEnterTime: new Date("2024-01-16T09:00:00"),
                    loadingLeaveTime: new Date("2024-01-16T11:00:00"),
                    vvnId: "vessel-002"
                }]
            }],
            metrics: [{
                algorithm: "greedy",
                computationTime: 800,
                strategy: "best-fit",
                totalDelay: 15,
                vesselCount: 2
            }]
        });

        const samplePlan3 = new OperationPlans({
            date: new Date("2024-01-17"),
            dockPlanMap: [{
                dockId: "dock-3",
                schedule: [{
                    cranes: ["crane-4", "crane-5"],
                    loadingEnterTime: new Date("2024-01-17T10:00:00"),
                    loadingLeaveTime: new Date("2024-01-17T14:00:00"),
                    vvnId: "vessel-003"
                }]
            }],
            metrics: [{
                algorithm: "genetic",
                computationTime: 2000,
                strategy: "random-fit",
                totalDelay: 5,
                vesselCount: 4
            }]
        });

        await samplePlan.save();
        await samplePlan2.save();
        await samplePlan3.save();

    } catch (error) {
        console.error("Error bootstrapping operation plans:", error);
    }
};