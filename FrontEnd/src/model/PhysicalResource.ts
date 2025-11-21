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

export enum ResourceStatus {
    Available = 0,
    Maintenance = 1,
    OutOfService = 2
}

export abstract class PhysicalResource {
    protected _type: string;
    protected _code: string;
    protected _description: string;
    protected _status: ResourceStatus;
    protected _setupTimeInMinutes: number;
    protected _operationalWindow: OperationalWindow;
    protected _qualifications: Qualification[];
    protected _active: boolean = true;

    constructor(params: {
        type: string;
        code: string;
        description: string;
        status: number;
        setupTimeInMinutes: number;
        operationalWindow: OperationalWindow;
        qualifications?: Qualification[];
    }) {
        if (!params.code) throw new Error('Code cannot be null or empty.');
        if (!params.description) throw new Error('Description cannot be null or empty.');
        if (!params.operationalWindow) throw new Error('Operational window cannot be null.');

        this._type = params.type;
        this._code = params.code;
        this._description = params.description;
        this._status = params.status;
        this._setupTimeInMinutes = params.setupTimeInMinutes;
        this._operationalWindow = params.operationalWindow;
        this._qualifications = params.qualifications ?? [];
    }

    get type(): string { return this._type; }
    get code(): string { return this._code; }
    get description(): string { return this._description; }
    get status(): ResourceStatus { return this._status; }
    get setupTimeInMinutes(): number { return this._setupTimeInMinutes; }
    get operationalWindow(): OperationalWindow { return this._operationalWindow; }
    get qualifications(): Qualification[] { return this._qualifications; }
    get active(): boolean { return this._active; }

    deactivate(): void {
        this._active = false;
        this._status = ResourceStatus.OutOfService;
    }

    updateDescription(description: string): void {
        if (!description) throw new Error('Description cannot be null or empty.');
        this._description = description;
    }

    updateStatus(status: ResourceStatus): void {
        this._status = status;
    }

    updateSetupTime(setupTimeInMinutes: number): void {
        this._setupTimeInMinutes = setupTimeInMinutes;
    }

    updateQualifications(qualifications: Qualification[]): void {
        this._qualifications = qualifications;
    }

    updateOperationalWindow(operationalWindow: OperationalWindow): void {
        if (!operationalWindow) throw new Error('Operational window cannot be null.');
        this._operationalWindow = operationalWindow;
    }

    abstract toDto(): PhysicalResourceDto;
}

export class STSCrane extends PhysicalResource {
    private _liftingCapacity: number;
    private _servingDock: Dock;
    private _containersPerHour: number;

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
        this._liftingCapacity = params.liftingCapacity;
        this._servingDock = params.servingDock;
        this._containersPerHour = params.containersPerHour;
    }

    get liftingCapacity(): number { return this._liftingCapacity; }
    get servingDock(): Dock { return this._servingDock; }
    get containersPerHour(): number { return this._containersPerHour; }

    updateServingDock(dock: Dock): void {
        if (!dock) throw new Error('Serving dock cannot be null.');
        this._servingDock = dock;
    }

    updateLiftingCapacity(liftingCapacity: number): void {
        this._liftingCapacity = liftingCapacity;
    }

    updateContainersPerHour(containersPerHour: number): void {
        this._containersPerHour = containersPerHour;
    }

    toDto(): STSCraneDto {
        return {
            type: 'STS Crane',
            code: this._code,
            description: this._description,
            status: this._status,
            setupTimeInMinutes: this._setupTimeInMinutes,
            operationalWindow: this._operationalWindow,
            qualificationsCodes: this._qualifications.map(q => q.idCode),
            liftingCapacity: this._liftingCapacity,
            servingDockCode: this._servingDock.code,
            containersPerHour: this._containersPerHour
        } as any as STSCraneDto;
    }
}

export class YardCrane extends PhysicalResource {
    private _liftingCapacity: number;
    private _containersPerHour: number;

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
        this._liftingCapacity = params.liftingCapacity;
        this._containersPerHour = params.containersPerHour;
    }

    get liftingCapacity(): number { return this._liftingCapacity; }
    get containersPerHour(): number { return this._containersPerHour; }

    updateLiftingCapacity(liftingCapacity: number): void {
        this._liftingCapacity = liftingCapacity;
    }

    updateContainersPerHour(containersPerHour: number): void {
        this._containersPerHour = containersPerHour;
    }

    toDto(): YardCraneDto {
        return {
            type: 'Yard Crane',
            code: this._code,
            description: this._description,
            status: this._status,
            setupTimeInMinutes: this._setupTimeInMinutes,
            operationalWindow: this._operationalWindow,
            qualificationsCodes: this._qualifications.map(q => q.idCode),
            liftingCapacity: this._liftingCapacity,
            containersPerHour: this._containersPerHour
        } as any as YardCraneDto;
    }
}

export class Truck extends PhysicalResource {
    private _maxLoadCapacity: number;
    private _averageSpeed: number;
    private _containersPerTrip: number;

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
        this._maxLoadCapacity = params.maxLoadCapacity;
        this._averageSpeed = params.averageSpeed;
        this._containersPerTrip = params.containersPerTrip;
    }

    get maxLoadCapacity(): number { return this._maxLoadCapacity; }
    get averageSpeed(): number { return this._averageSpeed; }
    get containersPerTrip(): number { return this._containersPerTrip; }

    updateContainersPerTrip(containersPerTrip: number): void {
        this._containersPerTrip = containersPerTrip;
    }

    updateAverageSpeed(averageSpeed: number): void {
        this._averageSpeed = averageSpeed;
    }

    updateMaxLoadCapacity(maxLoadCapacity: number): void {
        this._maxLoadCapacity = maxLoadCapacity;
    }

    toDto(): TruckDto {
        return {
            type: 'Truck',
            code: this._code,
            description: this._description,
            status: this._status,
            setupTimeInMinutes: this._setupTimeInMinutes,
            operationalWindow: this._operationalWindow,
            qualificationsCodes: this._qualifications.map(q => q.idCode),
            maxLoadCapacity: this._maxLoadCapacity,
            averageSpeed: this._averageSpeed,
            containersPerTrip: this._containersPerTrip
        } as any as TruckDto;
    }
}