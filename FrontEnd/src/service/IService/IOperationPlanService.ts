import type { Qualification } from '@/model/Qualifications';
import type { Filter, Page } from '@/model/Page';

export interface IOperationPlanService {

    getAllOperationPlans(): Promise<any>; // Francisco apaga isto
}