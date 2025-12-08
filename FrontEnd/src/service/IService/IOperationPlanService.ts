import type { Filter, Page } from '@/model/Page';

export interface IOperationPlanService {

    getAllOperationPlans(filtering?: Filter<null>): Promise<Page<any>>; // TODO usar modelo!
    groupOperationPlansByDate(): Promise<{
        date: string;
        plans: any[];
    }[]>;
}