import { OperationDto } from './value/operationDto';

export type OperationStatusDto =
  | 'Pending'
  | 'InProgress'
  | 'Completed'
  | 'Failed';

export interface OperationWithStatusDto {
  id: string;
  operation: OperationDto;
  status: OperationStatusDto;
}

export type VesselVisitExecutionStatusDto = 'Open' | 'Closed';

export interface VesselVisitExecutionDto {
  id: string;
  code: string;
  relatedVVN: string;
  operationsExecuted: OperationWithStatusDto[];
  dateOpen?: Date;
  dateClosed?: Date;
  status: VesselVisitExecutionStatusDto;
  dock?: string;
  berthTime?: Date;
  createdBy: string;
}