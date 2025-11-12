import type { PhysicalCharacteristics } from "../values/PhysicalCharacteristics";

export interface VesselTypeDto {
    name: string;
    description: string;
    maxNumberOfRows: number;
    maxNumberOfBays: number;
    maxNumberOfTiers: number;
    capacity?: number;
    physicalCharacteristics: PhysicalCharacteristics;
}