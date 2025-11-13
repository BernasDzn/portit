import type { VesselDto } from "@/model/dto/VesselDto";
import type { Filter, Page } from "@/model/Page";
import type { Vessel } from "@/model/Vessel";

export interface IVesselService {
    getVessels(filtering: Filter<Vessel>): Promise<Page<Vessel>>;
    getVesselByIMO(imo: string): Promise<Vessel>;
    getVesselByOwner(email: string): Promise<Vessel[]>;

    createVessel(vessel: VesselDto): Promise<Vessel>;
    updateVessel(vessel: VesselDto): Promise<Vessel>;

    getNumberOfVessels(): Promise<number>;
}