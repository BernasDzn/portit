import type { Dock } from "./Dock";
import type { Qualification } from "./Qualifications";

interface OperationalWindow {
    shifts: Shift[];
}

interface Shift {
    startTime: string;
    endTime: string;
    dayOfWeek: number;
}

export interface PhysicalResource {
    type: string;
    code: string;
    description: string;
    status: number;
    setupTimeInMinutes: number;
    operationalWindow: OperationalWindow;
    qualificationsCodes: Qualification[];
}

export interface STSCrane extends PhysicalResource {
    type: 'STS Crane';
    liftingCapacity: number;
    servingDockCode: Dock;
    containersPerHour: number;
}

export interface YardCrane extends PhysicalResource {
    type: 'Yard Crane';
    liftingCapacity: number;
    containersPerHour: number;
}

export interface Truck extends PhysicalResource {
    type: 'Truck';
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