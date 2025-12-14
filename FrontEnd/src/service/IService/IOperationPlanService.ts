import type { Filter, Page } from '@/model/Page';
import type { OperationPlanDto, OperationPlanFilter } from '@/model/dto/OperationPlanDto';

export interface IOperationPlanService {

    getAllOperationPlans(filtering?: Filter<OperationPlanFilter>): Promise<Page<OperationPlanDto>>;
    getOperationPlanById(id: string): Promise<OperationPlanDto>;
    groupOperationPlansByDate(): Promise<{
        date: string;
        plans: OperationPlanDto[];
    }[]>;
}