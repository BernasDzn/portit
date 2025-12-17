import { Service } from "typedi";
import { OperationPlanRepository } from "../repository/operationPlanRepository";
import OperationPlan from "../domain/operationPlan";
import VesselVisitExecution, { OperationWithStatus } from "../domain/vesselVisitExecution";
import { VesselVisitExecutionDto } from "../dto/vesselVisitExecutionDto";
import { VesselVisitExecutionRepository } from "../repository/vesselVisitExecutionRepository";
import Operation from "../domain/value/operation";
import { OperationStartDto } from "../dto/value/operationDto";
import { TaskCategoryRepository } from "../repository/taskCategoryRepository";
import { Resource, ResourceType } from "../domain/value/resource";
import { PayloadValidator } from "./validators/operationPayloadValidator";
import mongoose from "mongoose";

@Service("vesselVisitExecutionService")
export class VesselVisitExecutionService {

    operationPlanRepository: OperationPlanRepository;
    vesselVisitExecutionRepository: VesselVisitExecutionRepository;
    taskCategoryRepository: TaskCategoryRepository;

    constructor() {
        this.operationPlanRepository = new OperationPlanRepository();
        this.vesselVisitExecutionRepository = new VesselVisitExecutionRepository();
        this.taskCategoryRepository = new TaskCategoryRepository();
    }

    async createVesselVisitExecution(relatedVVN: string, creatorUser: string): Promise<VesselVisitExecutionDto> {

        const existingVVE = await this.vesselVisitExecutionRepository.getByVVN(relatedVVN);
        if (existingVVE) {
            throw new Error(`Vessel Visit Execution with related VVN ${relatedVVN} was already opened before.`);
        }

        const plan: OperationPlan | null = await this.operationPlanRepository.getByVVN(relatedVVN);
        if (!plan) {
            throw new Error(`Operation Plan with VVN ${relatedVVN} not found.`);
        }

        // Create a VVE with the same details as the Operation Plan, but everything starts as pending
        const vve: VesselVisitExecution = new VesselVisitExecution({
            code: `VVE-PORTO-${(await this.vesselVisitExecutionRepository.count()) + 1}`,
            relatedVVN: plan.relatedVVN,
            operationsExecuted: plan.operationSchedule.toArray().map(op => {
                // console.log('Plan operation:', {
                //     id: op.id,
                //     operationType: op.operationType,
                //     operationTypeId: op.operationType?.id
                // });
                return new OperationWithStatus(
                {
                    operation: op,
                    status: 'Pending'
                }
            )}),
            status: 'Open',
            dateOpen: new Date(),
            dateClosed: undefined,
            createdBy: creatorUser,
        });

        const created = await this.vesselVisitExecutionRepository.createVesselVisitExecution(vve);
        return created.toDto();
    }

    async closeVesselVisitExecution(relatedVVN: string): Promise<VesselVisitExecutionDto> {

        const vve: VesselVisitExecution | null = await this.vesselVisitExecutionRepository.getByVVN(relatedVVN);
        if (!vve) {
            throw new Error(`Vessel Visit Execution with VVN ${relatedVVN} not found.`);
        }

        if (vve.status === 'Closed') {
            throw new Error(`Vessel Visit Execution with VVN ${relatedVVN} is already closed.`);
        }

        // Check if all operations are completed
        const allCompleted = vve.operationsExecuted.every(opWS => opWS.status === 'Completed');
        if (!allCompleted) {
            throw new Error(`Cannot close Vessel Visit Execution with VVN ${relatedVVN} because not all operations are completed.`);
        }

        vve.status = 'Closed';
        vve.props.dateClosed = new Date();

        const updated = await this.vesselVisitExecutionRepository.updateVesselVisitExecution(vve);
        return updated.toDto();
    }

    /**
{
  "type": "LOAD",
  "startTime": "2025-12-17T16:59:10.341Z",
  "endTime": "2025-12-17T16:59:10.341Z",
  "resources": [
    {
      "name": "John Doe",
      "startTime": "2025-12-17T16:59:10.341Z",
      "endTime": "2025-12-17T17:59:10.341Z"
    }
  ],
  "payload": {
    "containerId": "AAAAA",
    "storageLocation": "aaa"
  }
}
     */

    async startOperation(vveId: string, operation: OperationStartDto): Promise<VesselVisitExecutionDto> {    
        
        const vve = await this.vesselVisitExecutionRepository.getByVVN(vveId);
        if (!vve) {
            throw new Error(`Vessel Visit Execution with id ${vveId} not found.`);
        }
    
        const operationWS = vve.operationsExecuted.find(opWS => opWS.operation.id === operation.id);
        
        if (!operationWS) {
            const operationType = await this.taskCategoryRepository.getCategoryByCode(operation.type);
            if (!operationType) {
                throw new Error(`Operation type with code ${operation.type} not found.`);
            }

            // You cannot create a load or unload operation 
            if (operationType.category.getValue() === 'LOAD' || operationType.category.getValue() === 'UNLOAD') {
                throw new Error(`Operation type ${operation.type} cannot be started manually.`);
            }
    
            new PayloadValidator(operation.payload, operationType.category.getValue()).validatePayloadForType();
    
            const mappedResources = operation.resources.map((res, index) => {
                
                const resource = new Resource({
                    name: res.name,
                    type: ResourceType.Staff,
                    startTime: new Date(res.startTime),
                    endTime: new Date(res.endTime)
                });
                
                console.log(`Created Resource ${index}:`, resource);
                return resource;
            });
    
            const actualOperation = new Operation({
                id: new mongoose.Types.ObjectId().toString(),
                operationType: operationType,
                startTime: new Date(operation.startTime),
                endTime: new Date(operation.endTime),
                resources: mappedResources,
                payload: operation.payload
            });
    
            console.log('Operation:', {
                id: actualOperation.id,
                operationType: actualOperation.operationType,
                operationTypeId: actualOperation.operationType?.id,
                operationTypeFullObject: JSON.stringify(actualOperation.operationType)
            });
            
            vve.operationsExecuted.push(new OperationWithStatus({
                operation: actualOperation,
                status: 'InProgress'
            }));

        } else {
 
            if (operationWS.status !== 'Pending') {
                throw new Error(`Operation with id ${operation.id} cannot be started because it is in status ${operationWS.status}.`);
            }
 
            // If it exists, update the existing operation
            // Only update start date, resources, and payload, other fields remain the same
            operationWS.props.status = 'InProgress';
            operationWS.operation.startTime = new Date(operation.startTime);
            operationWS.operation.resources = operation.resources.map((res, index) => {
                
                const resource = new Resource({
                    name: res.name,
                    type: ResourceType.Staff,
                    startTime: new Date(res.startTime),
                    endTime: new Date(res.endTime)
                });
                
                console.log(`Updated Resource ${index}:`, resource);
                return resource;
            });

            const operationType = await this.taskCategoryRepository.getCategoryByCode(operation.type);
            if (!operationType) {
                throw new Error(`Operation type with code ${operation.type} not found.`);
            }

            new PayloadValidator(operation.payload, operationType?.category.getValue()).validatePayloadForType();
            operationWS.operation.payload = operation.payload;
        }
    
        const updated = await this.vesselVisitExecutionRepository.updateVesselVisitExecution(vve);
        return updated.toDto();
    }

    async completeOperation(vveId: string, operationId: string, endTime: Date): Promise<VesselVisitExecutionDto> {
        const vve = await this.vesselVisitExecutionRepository.getByVVN(vveId);
        if (!vve) {
            throw new Error(`Vessel Visit Execution with id ${vveId} not found.`);
        }

        const operationWS = vve.operationsExecuted.find(opWS => opWS.operation.id === operationId);
        if (!operationWS) {
            throw new Error(`Operation with id ${operationId} not found in Vessel Visit Execution ${vveId}.`);
        }

        if (operationWS.status !== 'InProgress') {
            throw new Error(`Operation with id ${operationId} cannot be completed because it is in status ${operationWS.status}.`);
        }

        operationWS.props.status = 'Completed';
        operationWS.operation.endTime = endTime;

        const updated = await this.vesselVisitExecutionRepository.updateVesselVisitExecution(vve);
        return updated.toDto();
    }
}