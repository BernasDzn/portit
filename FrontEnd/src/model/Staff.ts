import type { Qualification } from "./Qualifications";

export interface Staff {
	mechanographicNumber: string;
	name: string;
	email?: string;
	phoneNumber?: string;
	status?: number;
	active?: boolean;
    qualifications?: Qualification[];
}