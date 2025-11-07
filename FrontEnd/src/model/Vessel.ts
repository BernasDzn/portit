import type { VesselType } from "./VesselType";

export interface Vessel {
    name: string;
    imoNumber: string;
    type: string;
    owner: string;
    length: number;
    depth: number;
    draft: number;
}

export interface VesselFilter {
    name?: string;
    imoNumber?: string;
    taxNumber?: string;
}