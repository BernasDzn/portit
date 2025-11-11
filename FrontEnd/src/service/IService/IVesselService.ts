import type { Filter, Page } from "@/model/Page";
import type { Vessel } from "@/model/Vessel";

export interface IVesselService {
    getVessels(filtering: Filter<Vessel>): Promise<Page<Vessel>>;
    createVessel(vessel: Vessel): Promise<Vessel>;
    getVesselByIMO(imo: string): Promise<Vessel>;
    updateVessel(imo: string, vessel: Vessel): Promise<Vessel>;
    getNumberOfVessels(): Promise<number>;
}