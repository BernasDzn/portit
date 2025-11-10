import type { PhysicalCharacteristics } from "./PhysicalCharacteristics";

export interface Dock {
    code: string;
    name: string;
    location: string;
    physicalCharacteristics: PhysicalCharacteristics;
    supportedVesselTypes: string[];
}

export interface DockFilter{
    dockName: string;
    location: string;
    vesselTypeName: string;
}