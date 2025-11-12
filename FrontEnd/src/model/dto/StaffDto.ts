import type { OperationalWindow } from "../values/OperationalWindow";

export interface StaffDto {
    mechanographicNumber: string;
    name: string;
    email: string;
    phoneNumber: string;
    status: number;
    operationalWindow: OperationalWindow;
    qualificationsCodes?: string[];
}