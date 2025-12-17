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
            operationsExecuted: plan.operationSchedule.toArray().map(op => new OperationWithStatus(
                {
                    operation: op,
                    status: 'Pending'
                }
            )),
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

    async startOperation(vveId: string, operation: OperationStartDto): Promise<VesselVisitExecutionDto | null> {    

        const vve = await this.vesselVisitExecutionRepository.getById(vveId);
        if (!vve) {
            throw new Error(`Vessel Visit Execution with id ${vveId} not found.`);
        }

        const operationWS = vve.operationsExecuted.find(opWS => opWS.operation.id === operation.id);
        if (!operationWS) {
            
            const operationType = await this.taskCategoryRepository.getCategoryByCode(operation.type);
            if (!operationType) {
                throw new Error(`Operation type with code ${operation.type} not found.`);
            }

            const actualOperation = new Operation({
                operationType: operationType!,
                startTime: new Date(operation.startTime),
                endTime: new Date(operation.endTime),
                resources: operation.resources.map(res => ({
                    name: res.name,
                    type: res.type as ResourceType,
                    startTime: res.startTime,
                    endTime: res.endTime
                } as Resource)),
                payload: operation.payload
            });

            // If not existing, make new
            vve.operationsExecuted.push(new OperationWithStatus(
                {
                    operation: actualOperation,
                    status: 'InProgress'
                }
            ));
        } else {
            // Update existing
            operationWS.props.status = 'InProgress';
            vve.props.operationsExecuted = vve.operationsExecuted.map(opWS => 
                opWS.operation.id === operation.id ? operationWS : opWS
            );
        }

        const updated = await this.vesselVisitExecutionRepository.updateVesselVisitExecution(vve);
        return updated.toDto();
    }
}