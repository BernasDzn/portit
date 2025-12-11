import { OperationPlan } from "../domain/operationPlan";
import { OperationPlanDto } from "../dto/operationPlanDto";
import { OperationPlanMapper } from "../mappers/operationPlanMapper";
import { OperationPlanModel } from "../schemas/operationPlanSchema";
import { Page, Pageable } from "../utils/page";
import config from "../config/config";


export class OperationPlanRepository {

	async create(operationPlan: OperationPlan): Promise<OperationPlanDto> {
		const newOperationPlan = OperationPlanMapper.toSchema(operationPlan);
		const createdDoc = await OperationPlanModel.create(newOperationPlan);

		return (await OperationPlanMapper.fromSchema(createdDoc)).toDto();
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
			items: await Promise.all(data.map(async (doc) => {
                const plan = await OperationPlanMapper.fromSchema(doc);
                return plan.toDto();
            }))
		};
	}

	async getById(id: string): Promise<OperationPlanDto | null> {
		const plan = await OperationPlanModel.findById(id);
		if (!plan) return null;
		return (await OperationPlanMapper.fromSchema(plan)).toDto();
	}

	async getByDateGrouped(): Promise<{ date: string; plans: OperationPlanDto[] }[]> {
		const allPlans = await OperationPlanModel.find();
		const plansByDate = new Map<string, OperationPlanDto[]>();

        for (const doc of allPlans) {
            const plan = OperationPlanMapper.fromSchema(doc);
            const planDto = (await plan).toDto();
            
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

	async getNotificationsWithoutPlan(token: string): Promise<string[]> {
		const url = `${config.backendServer}/VesselVisitNotification/getAllIds`;
		const res = await fetch(url, {
			credentials: "include",
			headers: {
				"Authorization": `Bearer ${token}`
			}
		});
		
		if (!res.ok) {
			throw new Error(`Failed to fetch all VVNs: ${res.statusText}`);
		}

		const data = await res.json();
		const allVvnIds: string[] = data;

		const allPlans = await OperationPlanModel.find();
		const plannedVvnIds = allPlans.map(doc => doc.relatedVVN);

		const unplannedVvnIds = allVvnIds.filter(id => !plannedVvnIds.includes(id));

		return unplannedVvnIds;
	}

	async deleteById(id: string): Promise<void> {
		await OperationPlanModel.findByIdAndDelete(id);
	}

}