import { Service } from "typedi";
import { OperationPlanRepository } from "../repository/operationPlanRepository";
import OperationPlan from "../domain/operationPlan";
import VesselVisitExecution, { OperationWithStatus } from "../domain/vesselVisitExecution";
import { VesselVisitExecutionDto } from "../dto/vesselVisitExecutionDto";
import { VesselVisitExecutionRepository } from "../repository/vesselVisitExecutionRepository";

@Service("vesselVisitExecutionService")
export class VesselVisitExecutionService {

    operationPlanRepository: OperationPlanRepository;
    vesselVisitExecutionRepository: VesselVisitExecutionRepository;

    constructor() {
        this.operationPlanRepository = new OperationPlanRepository();
        this.vesselVisitExecutionRepository = new VesselVisitExecutionRepository();
    }

    async createVesselVisitExecution(relatedVVN: string): Promise<VesselVisitExecutionDto> {

        const existingVVE = await this.vesselVisitExecutionRepository.getById(relatedVVN);
        if (existingVVE) {
            throw new Error(`Vessel Visit Execution with related VVN ${relatedVVN} was already open.`);
        }

        const plan: OperationPlan | null = await this.operationPlanRepository.getByVVN(relatedVVN);
        if (!plan) {
            throw new Error(`Operation Plan with VVN ${relatedVVN} not found.`);
        }

        // Create a VVE with the same details as the Operation Plan, but everything starts as pending
        const vve: VesselVisitExecution = new VesselVisitExecution({
            dock: plan.dock,
            relatedVVN: plan.relatedVVN,
            operationsExecuted: plan.operationSchedule.toArray().map(op => new OperationWithStatus(
                {
                    operation: op,
                    status: 'Pending'
                }
            )),
            status: 'Open',
            dateOpen: new Date(),
            dateClosed: undefined
        });

        const created = await this.vesselVisitExecutionRepository.createVesselVisitExecution(vve);
        return created.toDto();
    }
}