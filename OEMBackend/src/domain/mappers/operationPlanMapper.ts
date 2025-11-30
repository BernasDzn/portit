import { OperationPlan, DockPlan, Schedule, Metric } from "../operationPlans";

export class OperationPlanMapper {
    
    static fromSchema(doc: any): OperationPlan {
        return new OperationPlan({
            id: doc._id.toString(),

            dockPlanMap: (doc.dockPlanMap ?? []).map((dp: any) =>
                new DockPlan({
                    dockId: dp.dockId,
                    schedule: (dp.schedule ?? []).map((s: any) =>
                        new Schedule({
                            cranes: s.cranes ?? [],
                            loadingEnterTime: new Date(s.loadingEnterTime),
                            loadingLeaveTime: new Date(s.loadingLeaveTime),
                            vvnId: s.vvnId
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
