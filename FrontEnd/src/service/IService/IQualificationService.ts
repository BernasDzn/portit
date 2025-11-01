import type { Qualification } from "@/model/Qualifications";

export interface IQualificationService {
    getQualifications(): Promise<Qualification[]>;
    // create 
    // deactivate
    // update
    // filter
}