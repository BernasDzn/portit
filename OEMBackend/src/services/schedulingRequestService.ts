import config from "../config/config";
import { OperationPlanDto } from "../domain/dto/operationPlansDto";
import { OperationPlan } from "../domain/operationPlans";
import { operationPlanRepository } from "../repository/operationPlanRepository";

export class SchedulingRequestService {

    async scheduleRequest(day: string, alg: string, daysAhead: number = 2): Promise<OperationPlanDto> {
        const url = `${config.schedulingServer}/schedule?day=${day}&alg=${alg}&daysAhead=${daysAhead}`;

        const res = await fetch(url);
        if (!res.ok) {
            throw new Error(`Failed to fetch schedule: ${res.statusText}`);
        }

        const data = await res.json();
        return data;
    }
}

export const schedulingRequestService = new SchedulingRequestService();
