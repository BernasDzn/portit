import type { Page } from "@/model/Page";
import { type Staff } from "../../model/Staff";

export interface IStaffService {
	getStaffs(): Promise<Page<Staff>>;
	// create 
	// deactivate
	// update
	// filter
}