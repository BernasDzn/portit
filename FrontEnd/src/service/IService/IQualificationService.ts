import type { Filter, Page } from "@/model/Page";
import type { Qualification } from "@/model/Qualifications";

export interface IQualificationService {
    getQualifications(filtering: Filter<Qualification>): Promise<Page<Qualification>>;
    getQualificationById(id: string): Promise<Qualification>;
    addQualification(value: Qualification): Promise<Qualification>;
    updateQualification(id: string, value: Qualification): Promise<Qualification>;
    getNumberOfQualifications(): Promise<number>;
}