import OperationPlan from "../domain/operationPlan";
import { OperationPlanDto } from "../dto/operationPlanDto";
import { OperationPlanMapper } from "../mappers/operationPlanMapper";
import { OperationPlanModel } from "../schemas/operationPlanSchema";
import { Page } from "../utils/page";
import config from "../config/config";
import { ContainerDto } from "../dto/container";
import { PlanFilter } from "../dto/filters/planFilter";
import { TaskCategoryRepository } from "./taskCategoryRepository";
import { Service } from "typedi";

@Service("operationPlanRepository")
export class OperationPlanRepository {

	async create(operationPlan: OperationPlan): Promise<OperationPlan> {
		const newOperationPlan = OperationPlanMapper.toSchema(operationPlan);
		const createdDoc = await OperationPlanModel.create(newOperationPlan);
		
        const plan = await OperationPlanMapper.fromSchema(createdDoc, new TaskCategoryRepository());
        return plan;
	}

	async getAll(pageable: PlanFilter): Promise<Page<OperationPlan>> {
		const { pageNumber, pageSize } = pageable;
		const skip = (pageNumber - 1) * pageSize;

        const startDate = pageable.startDate ? new Date(pageable.startDate) : null;
        const endDate = pageable.endDate ? new Date(pageable.endDate) : null;

		const data = await OperationPlanModel.find()
            .where(startDate ? { 'operationSchedule.0.startTime': { $gte: startDate } } : {})
            .where(endDate ? { 'operationSchedule.0.startTime': { $lte: endDate } } : {})
            .sort({ 'updatedAt': -1 })
			.skip(skip)
			.limit(pageSize);

		return {
			pageNumber,
			pageSize,
			pageCount: Math.ceil(await OperationPlanModel.countDocuments() / pageSize),
			items: await Promise.all(data.map(async (doc) => {
                const plan = await OperationPlanMapper.fromSchema(doc, new TaskCategoryRepository());
                return plan;
            }))
		};
	}

	async getById(id: string): Promise<OperationPlan | null> {
		const doc = await OperationPlanModel.findById(id);
		if (!doc) return null;
		const plan = await OperationPlanMapper.fromSchema(doc, new TaskCategoryRepository());
		return plan;
	}

    async getByVVN(vvnId: string): Promise<OperationPlan | null> {
        const doc = await OperationPlanModel.findOne({ relatedVVN: vvnId });
        if (!doc) return null;
        const plan = await OperationPlanMapper.fromSchema(doc, new TaskCategoryRepository());
        return plan;
    }

	async getByDateGrouped(): Promise<{ date: string; plans: OperationPlanDto[] }[]> {
		const allPlans = await OperationPlanModel.find();
		const plansByDate = new Map<string, OperationPlanDto[]>();

        for (const doc of allPlans) {
            const plan = await OperationPlanMapper.fromSchema(doc, new TaskCategoryRepository());
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

	async getNotificationsWithoutPlan(token: string): Promise<string[]> {
		const baseUrl = config.backendServer.replace('localhost', '127.0.0.1');
		const url = `${baseUrl}/VesselVisitNotification/getAllAcceptedVVNs`;
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

    async getContainersOfNotification(vvnId: string, token: string, isUnload: boolean): Promise<ContainerDto[]> {

        const url = `${config.backendServer}/VesselVisitNotification/${vvnId}`;
        console.log(`Fetching containers for VVN ${vvnId} from ${url}`);
        const res = await fetch(url, {
            credentials: "include",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (!res.ok) {
            throw new Error(`Failed to fetch VVN ${vvnId}: ${res.statusText}`);
        }

        const data = await res.json();
        let loadCargoManifest = data.loadCargoManifest || [];
        let unloadCargoManifest = data.unloadCargoManifest || [];

        return (isUnload ? [...unloadCargoManifest] : [...loadCargoManifest]).map((containerData: any) => {
                return {
                    position: containerData.position,
                    area: containerData.area.nameCode,
                    containerNumber: containerData.container.containerNumber,
                } as ContainerDto;
            }
        );
    }
}