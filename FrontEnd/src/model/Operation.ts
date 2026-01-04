import type { OperationDto, ResourceDto } from './dto/VesselVisitExecutionDto';
import type { TaskCategory } from './TaskCategory';

export class Operation {
    private _id?: string;
    private _operationType: TaskCategory;
    private _startTime: Date;
    private _endTime: Date;
    private _resources: ResourceDto[];
    private _payload?: Object;

    constructor(params: {
        id?: string;
        operationType: TaskCategory;
        startTime: Date;
        endTime: Date;
        resources: ResourceDto[];
        payload?: Object;
    }) {
        if (params.startTime >= params.endTime) {
            throw new Error('Start time must be before end time.');
        }
        if (!params.resources || params.resources.length === 0) {
            throw new Error('Operation must have at least one resource.');
        }

        this._id = params.id;
        this._operationType = params.operationType;
        this._startTime = params.startTime;
        this._endTime = params.endTime;
        this._resources = params.resources;
        this._payload = params.payload;
    }

    get id(): string | undefined { return this._id; }
    get operationType(): TaskCategory { return this._operationType; }
    get startTime(): Date { return this._startTime; }
    get endTime(): Date { return this._endTime; }
    get resources(): ResourceDto[] { return this._resources; }
    get payload(): Object | undefined { return this._payload; }

    toDto(): OperationDto {
        return {
            id: this._id ?? '',
            type: this._operationType.toDto(),
            startTime: this._startTime.toISOString(),
            endTime: this._endTime.toISOString(),
            resources: this._resources,
            payload: this._payload ?? null
        };
    }
}