import { Pageable } from "../../utils/page";

export interface IncidentTypeFilter extends Pageable {
	name?: string;
	severity?: string;
}