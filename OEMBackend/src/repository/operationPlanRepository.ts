import { OperationPlan } from "../domain/operationPlan";
import { OperationPlanDto } from "../dto/operationPlanDto";
import { OperationPlanMapper } from "../mappers/operationPlanMapper";
import { OperationPlanModel } from "../schemas/operationPlanSchema";
import { Page, Pageable } from "../utils/page";


export class OperationPlanRepository {

	async create(operationPlan: OperationPlan): Promise<OperationPlanDto> {
		const newOperationPlan = OperationPlanMapper.toSchema(operationPlan);
		const createdDoc = await OperationPlanModel.create(newOperationPlan);

		return OperationPlanMapper.fromSchema(createdDoc).toDto();
	}

	async getAll(pageable: Pageable): Promise<Page<OperationPlanDto>> {
		const { pageNumber, pageSize } = pageable;
		const skip = (pageNumber - 1) * pageSize;
		const data = await OperationPlanModel.find()
			.skip(skip)
			.limit(pageSize);

		return {
			pageNumber,
			pageSize,
			pageCount: Math.ceil(await OperationPlanModel.countDocuments() / pageSize),
			items: data.map(doc => OperationPlanMapper.fromSchema(doc).toDto())
		};
	}

	async getById(id: string): Promise<OperationPlanDto | null> {
		const plan = await OperationPlanModel.findById(id);
		if (!plan) return null;
		return OperationPlanMapper.fromSchema(plan).toDto();
	}

	async getByDateGrouped(): Promise<{ date: string; plans: OperationPlanDto[] }[]> {
		const allPlans = await OperationPlanModel.find();
		const plansByDate = new Map<string, OperationPlanDto[]>();

        for (const doc of allPlans) {
            const plan = OperationPlanMapper.fromSchema(doc);
            const planDto = plan.toDto();
            
            // Extract date from the first operation's start time
            if (planDto.operationSchedule && planDto.operationSchedule.length > 0) {
                const firstOperation = planDto.operationSchedule[0];
                if (firstOperation && firstOperation.startTime) {
                    const startTime = new Date(firstOperation.startTime);
                    const dateKey = startTime.toISOString().split('T')[0] as string;
                    
                    if (!plansByDate.has(dateKey)) {
                        plansByDate.set(dateKey, []);
                    }
                    plansByDate.get(dateKey)!.push(planDto);
                }
            }
        }		
        // Convert map to array and sort by date
		return Array.from(plansByDate.entries())
			.map(([date, plans]) => ({ date, plans }))
			.sort((a, b) => a.date.localeCompare(b.date));
	}

}