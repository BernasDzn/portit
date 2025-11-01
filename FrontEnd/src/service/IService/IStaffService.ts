import { type Staff } from "../../model/Staff";

export interface IStaffService {
	getStaffs(): Promise<Staff[]>;
	// create 
	// deactivate
	// update
	// filter
}