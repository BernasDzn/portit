import { Pageable } from "../../utils/page";

export interface incidentFilter extends Pageable {
	vveCode?: string;
	filterStartTime?: string;
	filterEndTime?: string;
	severity?: string;
	isResolved?: boolean;
}