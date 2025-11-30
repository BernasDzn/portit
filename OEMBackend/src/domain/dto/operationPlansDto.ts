export interface OperationPlanDto {
    id: string;
    dockPlanMap: DockPlanDto[];
    metrics: MetricDto[];
    date: Date;
}

export interface DockPlanDto {
    dockId: string;
    schedule: ScheduleDto[];
}

export interface ScheduleDto {
    cranes: string[];
    loadingEnterTime: Date;
    loadingLeaveTime: Date;
    vvnId: string;
}

export interface MetricDto {
    algorithm: 'optimal' | 'greedy' | 'genetic';
    computationTime: number;
    strategy: string;
    totalDelay: number;
    vesselCount: number;
}