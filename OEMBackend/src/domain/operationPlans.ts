import { DockPlanDto, MetricDto, OperationPlanDto, ScheduleDto } from "./dto/operationPlansDto";

export class Schedule {
    cranes: string[];
    loadingEnterTime: Date;
    loadingLeaveTime: Date;
    vvnId: string;

    constructor(params: {
        cranes: string[];
        loadingEnterTime: Date;
        loadingLeaveTime: Date;
        vvnId: string;
    }) {
        this.cranes = params.cranes;
        this.loadingEnterTime = params.loadingEnterTime;
        this.loadingLeaveTime = params.loadingLeaveTime;
        this.vvnId = params.vvnId;
    }

    toDto(): ScheduleDto {
        return {
            cranes: this.cranes,
            loadingEnterTime: this.loadingEnterTime,
            loadingLeaveTime: this.loadingLeaveTime,
            vvnId: this.vvnId,
        };
    }
}

export class DockPlan {
    dockId: string;
    schedule: Schedule[];

    constructor(params: {
        dockId: string;
        schedule: Schedule[];
    }) {
        this.dockId = params.dockId;
        this.schedule = params.schedule;
    }

    toDto(): DockPlanDto {
        return {
            dockId: this.dockId,
            schedule: this.schedule.map(s => s.toDto()),
        };
    }
}

export class Metric {
    algorithm: 'optimal' | 'greedy' | 'genetic';
    computationTime: number;
    strategy: string;
    totalDelay: number;
    vesselCount: number;

    constructor(params: {
        algorithm: 'optimal' | 'greedy' | 'genetic';
        computationTime: number;
        strategy: string;
        totalDelay: number;
        vesselCount: number;
    }) {
        this.algorithm = params.algorithm;
        this.computationTime = params.computationTime;
        this.strategy = params.strategy;
        this.totalDelay = params.totalDelay;
        this.vesselCount = params.vesselCount;
    }

    toDto(): MetricDto {
        return {
            algorithm: this.algorithm,
            computationTime: this.computationTime,
            strategy: this.strategy,
            totalDelay: this.totalDelay,
            vesselCount: this.vesselCount,
        };
    }
}

export class OperationPlan {
    id: string;
    dockPlanMap: DockPlan[];
    metrics: Metric[];

    constructor(params: {
        id: string;
        dockPlanMap: DockPlan[];
        metrics: Metric[];
    }) {
        this.id = params.id;
        this.dockPlanMap = params.dockPlanMap;
        this.metrics = params.metrics;
    }

    toDto(): OperationPlanDto {
        return {
            id: this.id,
            dockPlanMap: this.dockPlanMap.map(dp => dp.toDto()),
            metrics: this.metrics.map(m => m.toDto()),
        };
    }
}