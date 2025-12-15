import { Pageable } from "../../utils/page";

export interface TaskCategoryFilter extends Pageable {
    name?: string;    
}