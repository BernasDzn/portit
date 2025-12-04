import { OperationPlan, DockPlan, Schedule, Metric } from "../operationPlans";

export class OperationPlanMapper {
    
    static fromSchema(doc: any): OperationPlan {
        return new OperationPlan({
            id: doc._id.toString(),

            date: new Date(doc.date),

            data: (doc.dockPlanMap ?? []).map((dp: any) =>
                new DockPlan({
                    dock: dp.dockId,
                    schedule: (dp.schedule ?? []).map((s: any) =>
                        new Schedule({
                            cranes: s.cranes ?? [],
                            loading_enter_time: new Date(s.loadingEnterTime),
                            loading_leave_time: new Date(s.loadingLeaveTime),
                            name: s.vvnId
                        })
                    )
                })
            ),

            metrics: (doc.metrics ?? []).map((m: any) =>
                new Metric({
                    algorithm: m.algorithm,
                    computationTime: m.computationTime,
                    strategy: m.strategy,
                    totalDelay: m.totalDelay,
                    vesselCount: m.vesselCount
                })
            )
        });
    }
}
