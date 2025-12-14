import { Pageable } from "../../utils/page";

export interface PlanFilter extends Pageable {
    
    startDate?: string;
    endDate?: string;
}