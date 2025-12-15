import mongoose from 'mongoose';
import { TaskCategoryModel } from './schemas/taskCategories';
import { TaskCategory } from './domain/taskCategory';
import IncidentType from './domain/incidentType';

export const bootstrap = async () => {
    await bootstrapOperationPlans();
    await bootstrapTaskCategories();
};

const bootstrapTaskCategories = async () => {
    try {
        const categories = [
            { category: 'BERTH', description: 'Vessel berthing procedures', name: "Berthing" },
            { category: 'LOAD', description: 'Container loading procedures', name: "Loading" },
            { category: 'UNLOAD', description: 'Container unloading procedures', name: "Unloading" },
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

const bootstrapIncidentTypes = async () => {

    try {

        const types = [{
            "_id": {
              "$oid": "693f3679a9f04d506b429c29"
            },
            "id": "INC-EEC8473E5",
            "name": "Environmental Conditions",
            "description": "Natural or weather-related conditions that may affect operations and safety.",
            "severity": "Major",
            "subtypeOf": null,
            "subtypes": [
              {
                "$oid": "693f36b8a9f04d506b429c47"
              },
              {
                "$oid": "693f36cea9f04d506b429c5f"
              },
              {
                "$oid": "693f36e1a9f04d506b429c77"
              }
            ],
            "__v": 3
          },
          {
            "_id": {
              "$oid": "693f36b8a9f04d506b429c47"
            },
            "id": "INC-EED7AA56C",
            "name": "Fog",
            "description": "Reduced visibility impacting safe movement.",
            "severity": "Major",
            "subtypeOf": {
              "$oid": "693f3679a9f04d506b429c29"
            },
            "subtypes": [],
            "__v": 0
          },
          {
            "_id": {
              "$oid": "693f36cea9f04d506b429c5f"
            },
            "id": "INC-EEE6DC1BB",
            "name": "Strong Winds",
            "description": "High wind speeds affecting lifting and stability.",
            "severity": "Major",
            "subtypeOf": {
              "$oid": "693f3679a9f04d506b429c29"
            },
            "subtypes": [],
            "__v": 0
          },
          {
            "_id": {
              "$oid": "693f36e1a9f04d506b429c77"
            },
            "id": "INC-EEE11BF94",
            "name": "Heavy Rain",
            "description": "Intense rainfall causing surface and access issues.",
            "severity": "Minor",
            "subtypeOf": {
              "$oid": "693f3679a9f04d506b429c29"
            },
            "subtypes": [],
            "__v": 0
          },
          {
            "_id": {
              "$oid": "693f36fca9f04d506b429c86"
            },
            "id": "INC-EEEC04CB1",
            "name": "Operational Failures",
            "description": "Failures of equipment, systems, or processes required for normal operations.",
            "severity": "Critical",
            "subtypeOf": null,
            "subtypes": [
              {
                "$oid": "693f3709a9f04d506b429c94"
              },
              {
                "$oid": "693f3719a9f04d506b429ca6"
              }
            ],
            "__v": 2
          },
          {
            "_id": {
              "$oid": "693f3709a9f04d506b429c94"
            },
            "id": "INC-EEEA1B75C",
            "name": "Crane Malfunction",
            "description": "Crane operates incorrectly or stops working.",
            "severity": "Critical",
            "subtypeOf": {
              "$oid": "693f36fca9f04d506b429c86"
            },
            "subtypes": [],
            "__v": 0
          },
          {
            "_id": {
              "$oid": "693f3719a9f04d506b429ca6"
            },
            "id": "INC-EEF042254",
            "name": "Power Outage",
            "description": "Loss of electrical power to facilities or equipment.",
            "severity": "Major",
            "subtypeOf": {
              "$oid": "693f36fca9f04d506b429c86"
            },
            "subtypes": [],
            "__v": 0
          },
          {
            "_id": {
              "$oid": "693f375da9f04d506b429cbb"
            },
            "id": "INC-EF0E7A230",
            "name": "Safety and Security",
            "description": "Incidents that threaten the safety of personnel or the security of assets.",
            "severity": "Critical",
            "subtypeOf": null,
            "subtypes": [
              {
                "$oid": "693f376ba9f04d506b429cc9"
              }
            ],
            "__v": 1
          },
          {
            "_id": {
              "$oid": "693f376ba9f04d506b429cc9"
            },
            "id": "INC-EF0063D9D",
            "name": "Security Alert",
            "description": "Identified or suspected security threat.",
            "severity": "Critical",
            "subtypeOf": {
              "$oid": "693f375da9f04d506b429cbb"
            },
            "subtypes": [],
            "__v": 0
        }];

        types.map((t) => new IncidentType({
            name: t.name,
            description: t.description,
            severity: t.severity as 'Minor' | 'Major' | 'Critical',
            subtypeOf: undefined,
            subtypes: undefined
        }, t.id));

        

    } catch (error) {
        console.error("Error bootstrapping incident types:", error);
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
