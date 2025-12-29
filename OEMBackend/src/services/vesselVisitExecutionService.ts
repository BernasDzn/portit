import { Service } from "typedi";
import { OperationPlanRepository } from "../repository/operationPlanRepository";
import OperationPlan from "../domain/operationPlan";
import VesselVisitExecution, { OperationWithStatus } from "../domain/vesselVisitExecution";
import { VesselVisitExecutionDto } from "../dto/vesselVisitExecutionDto";
import { VesselVisitExecutionRepository } from "../repository/vesselVisitExecutionRepository";
import Operation from "../domain/value/operation";
import { OperationDto, OperationStartDto } from "../dto/value/operationDto";
import { TaskCategoryRepository } from "../repository/taskCategoryRepository";
import { Resource, ResourceType } from "../domain/value/resource";
import { PayloadValidator } from "./validators/operationPayloadValidator";
import mongoose from "mongoose";
import { Page } from "../utils/page";
import { VesselVisitExecutionFilter } from "../dto/filters/vesselVisitExecutionFilter";

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
                    status: 'Pending',
                    impactedOperations: []
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

    async startOperation(vveId: string, operation: OperationStartDto): Promise<VesselVisitExecutionDto> {    
        
        const vve = await this.vesselVisitExecutionRepository.getByVVN(vveId);
        if (!vve) {
            throw new Error(`Vessel Visit Execution with id ${vveId} not found.`);
        }

        let operationWS = vve.operationsExecuted.find(opWS => opWS.operation.id === operation.id);

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
                status: 'Started',
                impactedOperations: operation.impactedOperations || []
            }));

            operationWS = vve.operationsExecuted[vve.operationsExecuted.length - 1];

            // When starting a complementary task that impacts other operations,
            // change their status to Delayed if they are Started or Pending
            if(operationWS!.impactedOperations.length > 0){
                console.log('New complementary task impacts operations:', operationWS!.impactedOperations);
                for(const opWS of vve.operationsExecuted){
                    if(operationWS!.impactedOperations.includes(opWS.operation.id!)){
                        console.log(`  Checking operation ${opWS.operation.id}, status: ${opWS.status}`);
                        if(opWS.status === 'Started' || opWS.status === 'Pending'){
                            console.log(`    Changing from ${opWS.status} to Delayed`);
                            opWS.props.status = 'Delayed';
                        }
                    }
                }
            }
        } else {
 
            if (operationWS.status !== 'Pending' && operationWS.status !== 'Delayed') {
                throw new Error(`Operation with id ${operation.id} cannot be started because it is in status ${operationWS.status}.`);
            }
            
            // Check if this operation is being blocked by any active complementary tasks
            if (operationWS.status === 'Delayed') {
                const blockingTasks = vve.operationsExecuted.filter(opWS => 
                    (opWS.status === 'Started' || opWS.status === 'Delayed') &&
                    opWS.impactedOperations.includes(operationWS!.operation.id!)
                );
                if (blockingTasks.length > 0) {
                    const taskNames = blockingTasks.map(t => t.operation.operationType?.category.getValue() || 'Unknown').join(', ');
                    throw new Error(`Operation with id ${operation.id} is currently blocked by the following tasks: ${taskNames}. Complete those tasks first.`);
                }
            }
 
            // If it exists, update the existing operation
            // Only update start date, resources, and payload, other fields remain the same
            operationWS.props.status = 'Started';
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

        const newStartTime = new Date(operation.startTime);
        let delayMs = 0;
        if(newStartTime > operationWS!.operation.startTime){
            delayMs = newStartTime.getTime() - operationWS!.operation.startTime.getTime();
        }

        let expectedNewEndTime = operationWS!.operation.endTime;
        const prevExpectedStart = operationWS!.operation.startTime;

        // If the new start time is after the previous expected start time, delay subsequent pending operations and add delay to their times
        if (delayMs > 0) {
            for (const opWS of vve.operationsExecuted) {
                if(opWS.id === operationWS!.id) continue;
                if (opWS.operation.startTime >= prevExpectedStart) {
                    if (opWS.status === 'Pending' || opWS.status === 'Delayed') {
                        opWS.props.status = 'Delayed';
                        if (opWS.operation.startTime) {
                            opWS.operation.startTime = new Date(new Date(opWS.operation.startTime).getTime() + delayMs);
                        }
                        if (opWS.operation.endTime) {
                            opWS.operation.endTime = new Date(new Date(opWS.operation.endTime).getTime() + delayMs);
                        }
                    }
                }
            }
            expectedNewEndTime = new Date(operationWS!.operation.endTime.getTime() + delayMs);
        }
        
        operationWS!.props.status = 'Started';
        operationWS!.operation.startTime = newStartTime;
        operationWS!.operation.endTime = expectedNewEndTime;

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

        if (operationWS.status !== 'Started' && operationWS.status !== 'Delayed') {
            throw new Error(`Operation with id ${operationId} cannot be completed because it is in status ${operationWS.status}.`);
        }

        const resources = operationWS.operation.resources.map(r => r.name);
        for (const otherOp of vve.operationsExecuted) {
            if (otherOp.operation.id === operationId) continue;
            if (otherOp.status === 'Completed') continue;
            for (const otherOpres of otherOp.operation.resources) {
                if (resources.includes(otherOpres.name) && otherOp.status === 'Started') {
                    const otherStart = new Date(otherOp.operation.startTime);
                    if (otherStart < endTime) {
                        throw new Error(`Resource '${otherOpres.name}' is scheduled for another operation starting at ${otherStart.toLocaleString()} before this operation's end time.`);
                    }
                }
            }
        }

        operationWS.props.status = 'Completed';
        operationWS.operation.endTime = endTime;

        console.log('Completing operation:', operationId);
        console.log('This operation impacts:', operationWS.impactedOperations);

        // When completing an operation that impacts others, check if those operations
        // can return to their previous status. They should only return if
        // there are NO other active (Started/Delayed) operations impacting them.
        if(operationWS!.impactedOperations.length > 0){
            for(const opWS of vve.operationsExecuted){
                const opId = opWS.operation.id!;
                const isImpacted = operationWS!.impactedOperations.includes(opId);
                
                if(isImpacted){
                    if(opWS.status === 'Delayed'){
                        // Check if any other active operations are still impacting this operation
                        const stillBlocked = vve.operationsExecuted.some(otherOp => {
                            const isBlocking = otherOp.operation.id !== operationWS!.operation.id && // Not the current operation being completed
                                (otherOp.status === 'Started' || otherOp.status === 'Delayed') && // Still active
                                otherOp.impactedOperations.includes(opWS.operation.id!); // Impacts this operation
                            return isBlocking;
                        });
                        // Only change back to Pending if no other operations are blocking it
                        // (operations that were Pending before being Delayed should return to Pending, not Started)
                        if (!stillBlocked) {
                            opWS.props.status = 'Pending';
                        }
                    }
                }
            }
        }

        const updated = await this.vesselVisitExecutionRepository.updateVesselVisitExecution(vve);

        const allCompleted = vve.operationsExecuted.every(opWS => opWS.props.status === 'Completed');
        if (allCompleted) {
            await this.closeVesselVisitExecution(vve.relatedVVN);
        }

        return updated.toDto();
    }

    async updateBerthDetails(relatedVVN: string, dock: string, berthTime: Date): Promise<VesselVisitExecutionDto> {
        const vve = await this.vesselVisitExecutionRepository.getByVVN(relatedVVN);
        if (!vve) {
            throw new Error(`Vessel Visit Execution with VVN ${relatedVVN} not found.`);
        }

        if (vve.status === 'Closed') {
            throw new Error(`Cannot update berth details for closed Vessel Visit Execution ${relatedVVN}.`);
        }

        vve.props.dock = dock;
        vve.props.berthTime = berthTime;

        const updated = await this.vesselVisitExecutionRepository.updateVesselVisitExecution(vve);
        console.log('Updated VVE with berth details:', updated);
        return updated.toDto();
    }

    async getVesselVisitExecutionByVVN(relatedVVN: string): Promise<VesselVisitExecutionDto> {
        const vve = await this.vesselVisitExecutionRepository.getByVVN(relatedVVN);
        if (!vve) {
            throw new Error(`Vessel Visit Execution with VVN ${relatedVVN} not found.`);
        }
        return vve.toDto();
    }

    async getAllVesselVisitExecutions(filter: VesselVisitExecutionFilter): Promise<Page<VesselVisitExecutionDto>> {
        const vvePage = await this.vesselVisitExecutionRepository.getAllVesselVisitExecutions(filter);
        return {
            items: vvePage.items.map(vve => vve.toDto()),
            pageSize: vvePage.pageSize,
            pageNumber: vvePage.pageNumber,
            pageCount: vvePage.pageCount
        };
    }

    async getVesselVisitExecutionByCode(code: string): Promise<VesselVisitExecutionDto> {
        const vve = await this.vesselVisitExecutionRepository.getByCode(code);
        if (!vve) {
            throw new Error(`Vessel Visit Execution with code ${code} not found.`);
        }
        return vve.toDto();
    }
    
    async count(): Promise<number> {
        return this.vesselVisitExecutionRepository.count();
    }
}