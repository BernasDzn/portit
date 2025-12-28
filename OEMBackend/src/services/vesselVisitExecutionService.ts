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

    async startOperation(vveId: string, operation: OperationStartDto): Promise<VesselVisitExecutionDto> {    
        
        const vve = await this.vesselVisitExecutionRepository.getByVVN(vveId);
        if (!vve) {
            throw new Error(`Vessel Visit Execution with id ${vveId} not found.`);
        }

        const operationWS = vve.operationsExecuted.find(opWS => opWS.operation.id === operation.id);

        if (!operationWS) {
            throw new Error(`Operation with id ${operation.id} not found in Vessel Visit Execution ${vveId}. Only existing operations can be started.`);
        }

        const newStartTime = new Date(operation.startTime);
        let delayMs = 0;
        if(newStartTime > operationWS.operation.startTime){
            delayMs = newStartTime.getTime() - operationWS.operation.startTime.getTime();
        }

        let expectedNewEndTime = operationWS.operation.endTime;
        const prevExpectedStart = operationWS.operation.startTime;

        // If the new start time is after the previous expected start time, delay subsequent pending operations and add delay to their times
        if (delayMs > 0) {
            for (const opWS of vve.operationsExecuted) {
                if(opWS.id === operationWS.id) continue;
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
            expectedNewEndTime = new Date(operationWS.operation.endTime.getTime() + delayMs);
        }
        
        operationWS.props.status = 'Started';
        operationWS.operation.startTime = newStartTime;
        operationWS.operation.endTime = expectedNewEndTime;

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

        const updated = await this.vesselVisitExecutionRepository.updateVesselVisitExecution(vve);
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

    async count(): Promise<number> {
        return this.vesselVisitExecutionRepository.count();
    }
}