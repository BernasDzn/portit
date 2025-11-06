import type { Qualification } from "./Qualifications";
import type { OperationalWindow } from "./OperationalWindow";

export interface Staff {
	mechanographicNumber: string;
	name: string;
	email: string;
	phoneNumber: string;
	status: number;
	active: boolean;
	operationalWindow : OperationalWindow;
    qualifications?: Qualification[];
}

export interface StaffCreate {
	mechanographicNumber: string;
	name: string;
	email: string;
	phoneNumber: string;
	status: number;
	operationalWindow: OperationalWindow;
	qualificationsCodes?: string[];
}