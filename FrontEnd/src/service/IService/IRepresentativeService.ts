import type { Filter, Page } from "@/model/Page";
import type { Representative } from "@/model/Representative";

export interface IRepresentativeService {
    getAll(): Promise<Page<Representative>>
    getByEmail(emailAddress: string): Promise<Representative>
    getByCitizenId(citizenId: string): Promise<Representative>
}