import { OperationPlans } from "../schemas/operationPlansSchema";
import { OperationPlan } from "../domain/operationPlans";
import { OperationPlanMapper } from "../domain/mappers/operationPlanMapper";

export class OperationPlanRepository {

    async findAll(): Promise<OperationPlan[]> {
        const data = await OperationPlans.find();
        return data.map(doc => OperationPlanMapper.fromSchema(doc));
    }

    async findById(id: string): Promise<OperationPlan | null> {
        const data = await OperationPlans.findById(id);
        
        if (data) return OperationPlanMapper.fromSchema(data);
        return null;
    }
}

export const operationPlanRepository = new OperationPlanRepository();