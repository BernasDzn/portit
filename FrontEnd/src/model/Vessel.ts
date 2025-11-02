import type { VesselType } from "./VesselType";

export interface Vessel {
    name: string;
    imoNumber: string;
    type: VesselType;
    owner: string;
    length: number;
    depth: number;
    draft: number;
}