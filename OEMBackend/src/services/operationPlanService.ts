import { OperationPlanDto } from "../domain/dto/operationPlansDto";
import { operationPlanRepository } from "../repository/operationPlanRepository";
import config from "../config/config";
import { OperationPlan } from "../domain/operationPlans";

export class OperationPlanService {
    async getAll(): Promise<OperationPlanDto[]> {
        let plans = await operationPlanRepository.findAll();
        return plans.map(plan => plan.toDto());
    }

    async getById(id: string): Promise<OperationPlanDto | undefined> {
        let plan = await operationPlanRepository.findById(id);
        return plan?.toDto();
    }

    async findByDateRange(startDate: string, endDate: string): Promise<OperationPlanDto[]> {
        let plans = await operationPlanRepository.findByDateRange(startDate, endDate);
        return plans.map(plan => plan.toDto());
    }

    async groupByDate(): Promise<{
        date: string;
        count: number;
    }[]> {
        return await operationPlanRepository.groupBydate();
    }

    async getNotificationsWithoutPlan(token: string): Promise<string[]> {
        const url = `${config.backendServer}/VesselVisitNotification/getAllIds`;
        const res = await fetch(url,
            {
                credentials: "include",
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            }
        );
        if (!res.ok) {
            throw new Error(`Failed to fetch all VVNs: ${res.statusText}`);
        }

        const data = await res.json();
        const allVvnIds: string[] = data;

        const plans = await this.getAll();
        const plannedVvnIds = plans.map(plan => plan.id);

        const unplannedVvnIds = allVvnIds.filter(id => !plannedVvnIds.includes(id));

        return unplannedVvnIds;
    }

    async savePlan(planData: any): Promise<any> {
        console.log("Saving operation plan...", planData);
        const plan = new OperationPlan(planData);
        return await operationPlanRepository.savePlan(plan);
    }
}

export const operationPlanService = new OperationPlanService();
