import type { Page } from "@/model/Page";
import type { Qualification } from "@/model/Qualifications";

export interface IQualificationService {
    getQualifications(filtering: Qualification): Promise<Page<Qualification>>;
    // create 
    // deactivate
    // update
    // filter
}