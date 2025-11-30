import { OperationPlanDto } from "../domain/dto/operationPlansDto";
import { OperationPlan } from "../domain/operationPlans";
import { operationPlanRepository } from "../repository/operationPlanRepository";

export class OperationPlanService {
    async getAll(): Promise<OperationPlanDto[]> {
        console.log("OperationPlanService: Fetching all operation plans from repository...");
        let plans = await operationPlanRepository.findAll();
        console.log(`OperationPlanService: Retrieved ${plans.length} plans.`);
        return plans.map(plan => plan.toDto());
    }

    async getById(id: string): Promise<OperationPlanDto | undefined> {
        let plan = await operationPlanRepository.findById(id);
        return plan?.toDto();
    }
}

export const operationPlanService = new OperationPlanService();
