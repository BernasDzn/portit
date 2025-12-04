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

    async groupBydate(): Promise<{
        date: string;
        count: number;
    }[]> {
        const data = await OperationPlans.aggregate([
            {
                $group: {
                    _id: "$date",
                    count: { $sum: 1 }
                }
            },
            {
                $project: {
                    _id: 0,
                    date: "$_id",
                    count: 1
                }
            }
        ]);

        return data;
    }

    async findByDateRange(startDate: string, endDate: string): Promise<OperationPlan[]> {
        const data = await OperationPlans.find({
            date: {
                $gte: startDate,
                $lte: endDate
            }
        });

        return data.map(doc => OperationPlanMapper.fromSchema(doc));
    }

    async savePlan(operationPlan: OperationPlan): Promise<any> {
        const doc = new OperationPlans(operationPlan);
        return await doc.save();
    }
}

export const operationPlanRepository = new OperationPlanRepository();