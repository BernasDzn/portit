import type { StaffDto } from "@/model/dto/StaffDto";
import type { Filter, Page } from "@/model/Page";
import type { Staff } from "@/model/Staff";

export interface IStaffService {
	getStaffs(filtering?: Filter<Staff>): Promise<Page<Staff>>;
	getStaffByMechanographicNumber(mechanographicNumber: string): Promise<Staff>;
	
	createStaff(staff: Staff): Promise<Staff>;
	deactivateStaff(mechanographicNumber: string): Promise<void>;
	updateStaff(staff: Staff): Promise<Staff>;
	
	getNumberOfStaffs(): Promise<number>;
}