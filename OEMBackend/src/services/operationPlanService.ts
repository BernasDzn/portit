import { OperationPlanDto } from "../domain/dto/operationPlansDto";
import { operationPlanRepository } from "../repository/operationPlanRepository";
import config from "../config/config";
import { OperationPlan } from "../domain/operationPlans";
import {Pageable, Page, mapPageItems} from "../domain/page";

export class OperationPlanService {
    async getAll(pageable: Pageable): Promise<Page<OperationPlanDto>> {
        let plans = await operationPlanRepository.findAll(pageable);
        return mapPageItems(plans, plan => plan.toDto());
    }

    async getById(id: string): Promise<OperationPlanDto | undefined> {
        let plan = await operationPlanRepository.findById(id);
        return plan?.toDto();
    }

    async findByDateRange(startDate: string, endDate: string, pageable: Pageable): Promise<Page<OperationPlanDto>> {
        let plans = await operationPlanRepository.findByDateRange(startDate, endDate, pageable);
        return mapPageItems(plans, plan => plan.toDto());
    }

    async groupByDate(): Promise<{
        date: string;
        count: number;
    }[]> {
        return await operationPlanRepository.groupBydate();
    }

    async getNotificationsWithoutPlan(token: string): Promise<string[]> {
        const url = `${config.backendServer}/VesselVisitNotification/getAllIds`;
        console.log("Fetching all VVN IDs from external service..." + url);
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

        console.log("Fetched all VVN IDs from external service.");

        const data = await res.json();
        const allVvnIds: string[] = data;

        const plans = await this.getAll({
            pageNumber: 1,
            pageSize: Number.MAX_SAFE_INTEGER
        });
        
        const plannedVvnIds = plans.items.map(plan => plan.id);
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
