import type { Page } from "@/model/Page";
import { type Staff } from "../../model/Staff";

export interface IStaffService {
	getStaffs(): Promise<Page<Staff>>;
	getStaffByMechanographicNumber(mechanographicNumber: string): Promise<Staff | undefined>;
	createStaff(staff: Staff): Promise<Staff>;
	deactivateStaff(mechanographicNumber: string): Promise<void>;
	updateStaff(mechanographicNumber: string, staff: Staff): Promise<Staff>;
}