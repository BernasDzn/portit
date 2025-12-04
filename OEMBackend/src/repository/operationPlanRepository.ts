import { OperationPlans } from "../schemas/operationPlansSchema";
import { OperationPlan } from "../domain/operationPlans";
import { OperationPlanMapper } from "../domain/mappers/operationPlanMapper";
import { Pageable, Page } from "../domain/page";

export class OperationPlanRepository {

    async findAll(pageable: Pageable): Promise<Page<OperationPlan>> {
        // const data = await OperationPlans.find();
        // return data.map(doc => OperationPlanMapper.fromSchema(doc));

        const { pageNumber, pageSize } = pageable;
        const skip = (pageNumber - 1) * pageSize;
        const data = await OperationPlans.find()
            .skip(skip)
            .limit(pageSize);

        return {
            pageNumber,
            pageSize,
            pageCount: Math.ceil(await OperationPlans.countDocuments() / pageSize),
            items: data.map(doc => OperationPlanMapper.fromSchema(doc))
        };
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

    async findByDateRange(startDate: string, endDate: string, pageable: Pageable): Promise<Page<OperationPlan>> {
        // const data = await OperationPlans.find({
        //     date: {
        //         $gte: startDate,
        //         $lte: endDate
        //     }
        // });
        // return data.map(doc => OperationPlanMapper.fromSchema(doc));

        const data = await OperationPlans.find({
            date: {
                $gte: new Date(startDate),
                $lte: new Date(endDate)
            }
        })
        .skip((pageable.pageNumber - 1) * pageable.pageSize)
        .limit(pageable.pageSize);

        return {
            pageNumber: pageable.pageNumber,
            pageSize: pageable.pageSize,
            pageCount: Math.ceil(await OperationPlans.countDocuments({
                date: {
                    $gte: new Date(startDate),
                    $lte: new Date(endDate)
                }
            }) / pageable.pageSize),
            items: data.map(doc => OperationPlanMapper.fromSchema(doc))
        };
    }

    async savePlan(operationPlan: OperationPlan): Promise<any> {
        const doc = new OperationPlans(operationPlan);
        return await doc.save();
    }
}

export const operationPlanRepository = new OperationPlanRepository();