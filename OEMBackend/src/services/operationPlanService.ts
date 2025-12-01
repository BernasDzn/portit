import { OperationPlanDto } from "../domain/dto/operationPlansDto";
import { OperationPlan } from "../domain/operationPlans";
import { operationPlanRepository } from "../repository/operationPlanRepository";

export class OperationPlanService {
    async getAll(): Promise<OperationPlanDto[]> {
        let plans = await operationPlanRepository.findAll();
        return plans.map(plan => plan.toDto());
    }

    async getById(id: string): Promise<OperationPlanDto | undefined> {
        let plan = await operationPlanRepository.findById(id);
        return plan?.toDto();
    }

    async groupByDate(): Promise<{
        date: string;
        count: number;
    }[]> {
        return await operationPlanRepository.groupBydate();
    }
}

export const operationPlanService = new OperationPlanService();
