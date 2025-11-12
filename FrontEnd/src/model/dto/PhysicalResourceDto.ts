import type { OperationalWindow } from "../OperationalWindow";

export interface PhysicalResourceDto {
    code: string;
    description: string;
    status: number;
    setupTimeInMinutes: number;
    operationalWindow: OperationalWindow;
    qualificationsCodes: string[];
}

export interface STSCraneDto extends PhysicalResourceDto {
    liftingCapacity: number;
    servingDockCode: string;
    containersPerHour: number;
}

export interface YardCraneDto extends PhysicalResourceDto {
    liftingCapacity: number;
    containersPerHour: number;
}

export interface TruckDto extends PhysicalResourceDto {
    maxLoadCapacity: number;
    averageSpeed: number;
    containersPerTrip: number;
}

export interface PhysicalResourceFilter {
    Code?: string;
    Description?: string;
    Status?: 0 | 1 | 2;
    Type?: 0 | 1 | 2;
}