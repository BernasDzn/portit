import Operation from "./value/operation";
import { Entity } from '../core/domain/entity';
import mongoose from 'mongoose';
import { VesselVisitExecutionDto } from "../dto/vesselVisitExecutionDto";

export type OperationStatus =
  | 'Pending'
  | 'InProgress'
  | 'Completed'
  | 'Failed';

export type VesselVisitExecutionStatus = 'Open' | 'Closed';

interface OperationWithStatusProps {
  operation: Operation;
  status: OperationStatus;
}

interface VesselVisitExecutionProps {

    code: string;
    relatedVVN: string;
    operationsExecuted: OperationWithStatus[];
    dateOpen?: Date;
    dateClosed?: Date;
    status: VesselVisitExecutionStatus;
    createdBy: string;
}

export class OperationWithStatus extends Entity<OperationWithStatusProps> {
  get id(): string { return this._id; }
  get operation(): Operation { return this.props.operation; }
  get status() { return this.props.status; }

  constructor(props: OperationWithStatusProps, id?: any) {
    super(
      id || new mongoose.Types.ObjectId().toString(),
      props
    );
  }

  public toDto() {
    return {
      id: this.id,
      operation: this.operation.toDto(),
      status: this.status
    };
  }
}

export default class VesselVisitExecution extends Entity<VesselVisitExecutionProps> {
  get id(): string { return this._id; }
  get relatedVVN(): string { return this.props.relatedVVN; }
  get operationsExecuted(): OperationWithStatus[] { return this.props.operationsExecuted; }
  get dateOpen(): Date | undefined { return this.props.dateOpen; }
  get dateClosed(): Date | undefined { return this.props.dateClosed; }
  get status() { return this.props.status; }
  get createdBy(): string { return this.props.createdBy; }
  get code(): string { return this.props.code; }

  set status(newStatus: VesselVisitExecutionStatus) {
    this.props.status = newStatus;
  }

  constructor(props: VesselVisitExecutionProps, id?: any) {
    super(
      id || new mongoose.Types.ObjectId().toString(),
      {
        ...props,
        operationsExecuted: props.operationsExecuted ?? [],
        status: props.status ?? 'Open'
      }
    );
  }

  public toDto(): VesselVisitExecutionDto {
        return {
            id: this.id,
            code: this.code,
            relatedVVN: this.relatedVVN,
            operationsExecuted: this.operationsExecuted.map(op => op.toDto()),
            dateOpen: this.dateOpen,
            dateClosed: this.dateClosed,
            status: this.status,
            createdBy: this.createdBy
        };
  }
}
