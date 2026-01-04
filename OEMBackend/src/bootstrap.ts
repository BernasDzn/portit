import mongoose from 'mongoose';
import { TaskCategoryModel } from './schemas/taskCategories';
import { TaskCategory } from './domain/taskCategory';
import IncidentType, { Severity } from './domain/incidentType';
import { IncidentTypeRepository } from './repository/incidentTypeRepository';

export const bootstrap = async () => {
    //await bootstrapOperationPlans();
    await bootstrapTaskCategories();
};

const bootstrapTaskCategories = async () => {
    try {
        const categories = [
            { category: 'BERTH', description: 'Vessel berthing procedures', name: "Berthing" },
            { category: 'LOAD', description: 'Container loading procedures', name: "Loading" },
            { category: 'UNLOAD', description: 'Container unloading procedures', name: "Unloading" },
            { category: 'MAINT', description: 'Equipment maintenance tasks', name: "Maintenance" },
            { category: 'SAFETY', description: 'Safety inspections and drills', name: "Safety" },
            { category: 'CUSTOMS', description: 'Customs clearance tasks', name: "Customs" },
            { category: 'CARGO', description: 'Cargo handling procedures', name: "Cargo Handling" },
            { category: 'SECURITY', description: 'Security checks and protocols', name: "Security" }
        ];

        categories.map((c) => new TaskCategory({
            id: undefined,
            category: c.category,
            name: c.name,
            description: c.description
        }));

        await TaskCategoryModel.insertMany(categories);
 
        console.log('Bootstrapped task categories successfully.');
    } catch (error) {
        console.error("Error bootstrapping task categories:", error);
    }
};

const bootstrapOperationPlans = async () => {
//    try {
//    const plans = [];
//
//        const cranes = ['crane-1','crane-2','crane-3','crane-4','crane-5'];
//        const algorithms: ('optimal' | 'greedy' | 'genetic')[] = ['optimal','greedy','genetic'];
//        const strategies = ['first-fit','best-fit','random-fit'];
//        const docks = ['dock-1','dock-2','dock-3','dock-4'];
//
//        for (let i = 0; i < 20; i++) {
//            const date = new Date(2024, 0, 15 + Math.floor(Math.random() * 11));
//
//            const dockId = docks[Math.floor(Math.random() * docks.length)];
//
//            const cranesAssigned = [];
//            const craneCount = 1 + Math.floor(Math.random() * 3);
//            for (let j = 0; j < craneCount; j++) {
//                cranesAssigned.push(cranes[Math.floor(Math.random() * cranes.length)]);
//            }
//
//            const startHour = 6 + Math.floor(Math.random() * 10);
//            const durationHours = 1 + Math.floor(Math.random() * 5);
//            const loadingEnterTime = new Date(date);
//            loadingEnterTime.setHours(startHour, 0, 0, 0);
//            const loadingLeaveTime = new Date(loadingEnterTime);
//            loadingLeaveTime.setHours(loadingEnterTime.getHours() + durationHours);
//
//            const algorithm = algorithms[Math.floor(Math.random() * algorithms.length)];
//            const strategy = strategies[Math.floor(Math.random() * strategies.length)];
//            const computationTime = 500 + Math.floor(Math.random() * 2500);
//            const totalDelay = Math.floor(Math.random() * 20);
//            const vesselCount = 1 + Math.floor(Math.random() * 5);
//
//            plans.push(new OperationPlans({
//                date,
//                dockPlanMap: [{
//                    dockId,
//                    schedule: [{
//                        cranes: cranesAssigned,
//                        loadingEnterTime,
//                        loadingLeaveTime,
//                        vvnId: `vessel-${100 + i}`
//                    }]
//                }],
//                metrics: [{
//                    algorithm,
//                    computationTime,
//                    strategy,
//                    totalDelay,
//                    vesselCount
//                }]
//            }));
//        }
//
//        await OperationPlans.insertMany(plans);
//
//        console.log('Bootstrapped 20+ operation plans successfully.');
//
//    } catch (error) {
//        console.error("Error bootstrapping operation plans:", error);
//    }
};
