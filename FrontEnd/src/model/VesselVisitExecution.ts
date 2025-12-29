import type { OperationWithStatusDto, VesselVisitExecutionDto } from "./dto/VesselVisitExecutionDto";
import type { Operation } from "./Operation";

export type OperationStatus = 'Pending' | 'Started' | 'Delayed' | 'Completed';
export type VesselVisitExecutionStatus = 'Open' | 'Closed';

export class OperationWithStatus {
    private _id: string;
    private _operation: Operation;
    private _status: OperationStatus;
    private _impactedOperations: string[];

    constructor(params: {
        id: string;
        operation: Operation;
        status: OperationStatus;
        impactedOperations?: string[];
    }) {
        this._id = params.id;
        this._operation = params.operation;
        this._status = params.status;
        this._impactedOperations = params.impactedOperations || [];
    }

    get id(): string { return this._id; }
    get operation(): Operation { return this._operation; }
    get status(): OperationStatus { return this._status; }
    get impactedOperations(): string[] { return this._impactedOperations; }

    set status(newStatus: OperationStatus) {
        this._status = newStatus;
    }

    toDto(): OperationWithStatusDto {
        return {
            id: this._id,
            operation: this._operation.toDto(),
            status: this._status,
            impactedOperations: this._impactedOperations
        };
    }
}

export class VesselVisitExecution {
    private _id: string;
    private _code: string;
    private _relatedVVN: string;
    private _operationsExecuted: OperationWithStatus[];
    private _dateOpen?: Date;
    private _dateClosed?: Date;
    private _status: VesselVisitExecutionStatus;
    private _createdBy: string;
    private _dock?: string;
    private _berthTime?: Date;

    constructor(params: {
        id: string;
        code: string;
        relatedVVN: string;
        operationsExecuted: OperationWithStatus[];
        dateOpen?: Date;
        dateClosed?: Date;
        status: VesselVisitExecutionStatus;
        createdBy: string;
    }) {
        if (!params.relatedVVN) {
            throw new Error('Related VVN cannot be null or empty.');
        }
        if (!params.code) {
            throw new Error('Code cannot be null or empty.');
        }
        if (!params.createdBy) {
            throw new Error('Creator email cannot be null or empty.');
        }

        this._id = params.id;
        this._code = params.code;
        this._relatedVVN = params.relatedVVN;
        this._operationsExecuted = params.operationsExecuted ?? [];
        this._dateOpen = params.dateOpen;
        this._dateClosed = params.dateClosed;
        this._status = params.status ?? 'Open';
        this._createdBy = params.createdBy;
    }

    get id(): string { return this._id; }
    get code(): string { return this._code; }
    get relatedVVN(): string { return this._relatedVVN; }
    get operationsExecuted(): OperationWithStatus[] { return this._operationsExecuted; }
    get dateOpen(): Date | undefined { return this._dateOpen; }
    get dateClosed(): Date | undefined { return this._dateClosed; }
    get status(): VesselVisitExecutionStatus { return this._status; }
    get createdBy(): string { return this._createdBy; }
    get dock(): string | undefined { return this._dock; }
    get berthTime(): Date | undefined { return this._berthTime; }

    addOperation(operation: OperationWithStatus): void {
        if (!operation) {
            throw new Error('Operation cannot be null.');
        }
        if (this._status === 'Closed') {
            throw new Error('Cannot add operations to a closed vessel visit execution.');
        }

        this._operationsExecuted.push(operation);
    }

    updateOperationStatus(operationId: string, newStatus: OperationStatus): void {
        if (this._status === 'Closed') {
            throw new Error('Cannot update operations in a closed vessel visit execution.');
        }

        const operation = this._operationsExecuted.find(op => op.id === operationId);
        if (!operation) {
            throw new Error(`Operation with id ${operationId} not found.`);
        }

        operation.status = newStatus;
    }

    close(): void {
        if (this._status === 'Closed') {
            throw new Error('Vessel visit execution is already closed.');
        }

        const hasIncompleteOperations = this._operationsExecuted.some(
            op => op.status === 'Pending' || op.status === 'Started' || op.status === 'Delayed'
        );

        if (hasIncompleteOperations) {
            throw new Error('Cannot close vessel visit execution with incomplete operations.');
        }

        this._status = 'Closed';
        this._dateClosed = new Date();
    }

    reopen(): void {
        if (this._status === 'Open') {
            throw new Error('Vessel visit execution is already open.');
        }

        this._status = 'Open';
        this._dateClosed = undefined;
    }

    toDto(): VesselVisitExecutionDto {
        return {
            id: this._id,
            code: this._code,
            relatedVVN: this._relatedVVN,
            operationsExecuted: this._operationsExecuted.map(op => op.toDto()),
            dateOpen: this._dateOpen?.toISOString(),
            dateClosed: this._dateClosed?.toISOString(),
            status: this._status,
            dock: this._dock,
            berthTime: this._berthTime?.toISOString(),
            createdBy: this._createdBy
        };
    }
}