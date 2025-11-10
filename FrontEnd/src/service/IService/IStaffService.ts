import type { Page } from "@/model/Page";
import { type Staff, type StaffCreate } from "../../model/Staff";

export interface IStaffService {
	getStaffs(): Promise<Page<Staff>>;
	getStaffByMechanographicNumber(mechanographicNumber: string): Promise<Staff>;
	createStaff(staff: StaffCreate): Promise<Staff>;
	deactivateStaff(mechanographicNumber: string): Promise<void>;
	updateStaff(mechanographicNumber: string, staff: StaffCreate): Promise<Staff>;
	getNumberOfStaffs(): Promise<number>;
}