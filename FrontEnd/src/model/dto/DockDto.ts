import type { PhysicalCharacteristics } from "../values/PhysicalCharacteristics";

export interface DockDto {
    code: string;
    name: string;
    location: string;
    physicalCharacteristics: PhysicalCharacteristics;
    supportedVesselTypes: string[];
}

export interface DockFilter {
    dockName: string;
    location: string;
    vesselTypeName: string;
}