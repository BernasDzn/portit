import { DockPlanDto, MetricDto, OperationPlanDto, ScheduleDto } from "./dto/operationPlansDto";

export class Schedule {
    cranes: string[];
    loadingEnterTime: Date;
    loadingLeaveTime: Date;
    vvnId: string;

    constructor(params: {
        cranes: string[];
        loading_enter_time: Date;
        loading_leave_time: Date;
        name: string;
    }) {
        this.cranes = params.cranes;
        this.loadingEnterTime = params.loading_enter_time;
        this.loadingLeaveTime = params.loading_leave_time;
        this.vvnId = params.name;
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
    dock: string;
    schedule: Schedule[];

    constructor(params: {
        dock: string;
        schedule: Schedule[];
    }) {
        this.dock = params.dock;
        this.schedule = params.schedule;
    }

    toDto(): DockPlanDto {
        return {
            dockId: this.dock,
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
    date: Date;
    dockPlanMap: DockPlan[];
    metrics: Metric[];

    constructor(params: {
        id: string;
        data: DockPlan[];
        metrics: Metric[];
        date: Date;
    }) {

        this.id = params.id;
        this.dockPlanMap = params.data;
        this.metrics = params.metrics;
        this.date = params.date;
    }

    toDto(): OperationPlanDto {
        return {
            id: this.id,
            dockPlanMap: this.dockPlanMap.map(dp => dp.toDto()),
            metrics: this.metrics.map(m => m.toDto()),
            date: this.date,
        };
    }
}