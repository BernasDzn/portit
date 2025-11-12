import type { Qualification } from '@/model/Qualifications';
import type { Filter, Page } from '@/model/Page';
import type { QualificationDto } from '@/model/dto/QualificationDto';

export interface IQualificationService {
    getQualifications(filtering?: Filter<Qualification>): Promise<Page<Qualification>>;
    getQualificationById(id: string): Promise<Qualification>;

    addQualification(value: QualificationDto): Promise<Qualification>;
    updateQualification(value: QualificationDto): Promise<Qualification>;
    
    getNumberOfQualifications(): Promise<number>;
}