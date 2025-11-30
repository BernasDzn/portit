import type { Qualification } from '@/model/Qualifications';
import type { Filter, Page } from '@/model/Page';

export interface IOprationPlanService {
    getQualifications(filtering?: Filter<Qualification>): Promise<Page<Qualification>>;
    getQualificationById(id: string): Promise<Qualification>;
}