import type { Filter, Page } from "@/model/Page";
import type { Vessel } from "@/model/Vessels";

export interface IVesselService {
    getVessels(filtering: Filter<Vessel>): Promise<Page<Vessel>>;
    // create 
    // deactivate
    // update
    // filter
}