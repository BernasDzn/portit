import { Pageable } from "../../utils/page";

export interface VesselVisitExecutionFilter extends Pageable {
    startDate?: string;
    endDate?: string;
    relatedVVN?: string;
    status?: string;
}
