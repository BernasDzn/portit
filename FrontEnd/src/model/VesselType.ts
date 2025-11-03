import type { PhysicalCharacteristics } from "./PhysicalCharacteristics";

export interface VesselType {
    name: string;
    description: string;
    maxNumberOfRows: number;
    maxNumberOfBays: number;
    maxNumberOfTiers: number;
    //capacity: number;
    physicalCharacteristics: PhysicalCharacteristics;
}