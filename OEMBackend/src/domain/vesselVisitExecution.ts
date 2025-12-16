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
  dock: string;
  relatedVVN: string;
  operationsExecuted: OperationWithStatus[];
  dateOpen?: Date;
  dateClosed?: Date;
  status: VesselVisitExecutionStatus;
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
  get dock(): string { return this.props.dock; }
  get relatedVVN(): string { return this.props.relatedVVN; }
  get operationsExecuted(): OperationWithStatus[] { return this.props.operationsExecuted; }
  get dateOpen(): Date | undefined { return this.props.dateOpen; }
  get dateClosed(): Date | undefined { return this.props.dateClosed; }
  get status() { return this.props.status; }

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
      dock: this.dock,
      relatedVVN: this.relatedVVN,
      operationsExecuted: this.operationsExecuted.map(op => op.toDto()),
      dateOpen: this.dateOpen,
      dateClosed: this.dateClosed,
      status: this.status
    };
  }
}
