import type { Dock } from "./Dock";
import type { Qualification } from "./Qualifications";
import type { OperationalWindow } from "./values/OperationalWindow";
import type { PhysicalResourceDto, STSCraneDto, YardCraneDto, TruckDto } from "./dto/PhysicalResourceDto";

export interface PhysicalResourceFilter {
    Code?: string;
    Description?: string;
    Status?: 0 | 1 | 2;
    Type?: 0 | 1 | 2;
}

export abstract class PhysicalResource {
    readonly type: string;
    readonly code: string;
    readonly description: string;
    readonly status: number;
    readonly setupTimeInMinutes: number;
    readonly operationalWindow: OperationalWindow;
    readonly qualifications: Qualification[];

    constructor(params: {
        type: string;
        code: string;
        description: string;
        status: number;
        setupTimeInMinutes: number;
        operationalWindow: OperationalWindow;
        qualifications?: Qualification[];
    }) {
        this.type = params.type;
        this.code = params.code;
        this.description = params.description;
        this.status = params.status;
        this.setupTimeInMinutes = params.setupTimeInMinutes;
        this.operationalWindow = params.operationalWindow;
        this.qualifications = params.qualifications ?? [];
    }

    abstract toDto(): PhysicalResourceDto;
}

export class STSCrane extends PhysicalResource {
    readonly liftingCapacity: number;
    readonly servingDock: Dock;
    readonly containersPerHour: number;

    constructor(params: {
        code: string;
        description: string;
        status: number;
        setupTimeInMinutes: number;
        operationalWindow: OperationalWindow;
        qualifications?: Qualification[];
        liftingCapacity: number;
        servingDock: Dock;
        containersPerHour: number;
    }) {
        super({
            type: 'STS Crane',
            code: params.code,
            description: params.description,
            status: params.status,
            setupTimeInMinutes: params.setupTimeInMinutes,
            operationalWindow: params.operationalWindow,
            qualifications: params.qualifications
        });
        this.liftingCapacity = params.liftingCapacity;
        this.servingDock = params.servingDock;
        this.containersPerHour = params.containersPerHour;
    }

    toDto(): STSCraneDto {
        return {
            type: 'STS Crane',
            code: this.code,
            description: this.description,
            status: this.status,
            setupTimeInMinutes: this.setupTimeInMinutes,
            operationalWindow: this.operationalWindow,
            qualificationsCodes: this.qualifications.map(q => (q as any).idCode ?? (q as any).code ?? ''),
            liftingCapacity: this.liftingCapacity,
            servingDockCode: (this.servingDock as any).code ?? (this.servingDock as any).idCode ?? '',
            containersPerHour: this.containersPerHour
        } as any as STSCraneDto;
    }
}

export class YardCrane extends PhysicalResource {
    readonly liftingCapacity: number;
    readonly containersPerHour: number;

    constructor(params: {
        code: string;
        description: string;
        status: number;
        setupTimeInMinutes: number;
        operationalWindow: OperationalWindow;
        qualifications?: Qualification[];
        liftingCapacity: number;
        containersPerHour: number;
    }) {
        super({
            type: 'Yard Crane',
            code: params.code,
            description: params.description,
            status: params.status,
            setupTimeInMinutes: params.setupTimeInMinutes,
            operationalWindow: params.operationalWindow,
            qualifications: params.qualifications
        });
        this.liftingCapacity = params.liftingCapacity;
        this.containersPerHour = params.containersPerHour;
    }

    toDto(): YardCraneDto {
        return {
            type: 'Yard Crane',
            code: this.code,
            description: this.description,
            status: this.status,
            setupTimeInMinutes: this.setupTimeInMinutes,
            operationalWindow: this.operationalWindow,
            qualificationsCodes: this.qualifications.map(q => (q as any).idCode ?? (q as any).code ?? ''),
            liftingCapacity: this.liftingCapacity,
            containersPerHour: this.containersPerHour
        } as any as YardCraneDto;
    }
}

export class Truck extends PhysicalResource {
    readonly maxLoadCapacity: number;
    readonly averageSpeed: number;
    readonly containersPerTrip: number;

    constructor(params: {
        code: string;
        description: string;
        status: number;
        setupTimeInMinutes: number;
        operationalWindow: OperationalWindow;
        qualifications?: Qualification[];
        maxLoadCapacity: number;
        averageSpeed: number;
        containersPerTrip: number;
    }) {
        super({
            type: 'Truck',
            code: params.code,
            description: params.description,
            status: params.status,
            setupTimeInMinutes: params.setupTimeInMinutes,
            operationalWindow: params.operationalWindow,
            qualifications: params.qualifications
        });
        this.maxLoadCapacity = params.maxLoadCapacity;
        this.averageSpeed = params.averageSpeed;
        this.containersPerTrip = params.containersPerTrip;
    }

    toDto(): TruckDto {
        return {
            type: 'Truck',
            code: this.code,
            description: this.description,
            status: this.status,
            setupTimeInMinutes: this.setupTimeInMinutes,
            operationalWindow: this.operationalWindow,
            qualificationsCodes: this.qualifications.map(q => (q as any).idCode ?? (q as any).code ?? ''),
            maxLoadCapacity: this.maxLoadCapacity,
            averageSpeed: this.averageSpeed,
            containersPerTrip: this.containersPerTrip
        } as any as TruckDto;
    }
}