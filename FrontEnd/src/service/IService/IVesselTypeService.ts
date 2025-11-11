import type { Filter, Page } from "@/model/Page";
import { type VesselType } from "../../model/VesselType";

export interface IVesselTypeService {
    getVesselTypes(filtering?: Filter<VesselType>): Promise<Page<VesselType>>
    getVesselTypeByName(name: string): Promise<VesselType | undefined>;
    createVesselType(vesselType: VesselType): Promise<VesselType>;
    updateVesselType(name: string, vesselType: VesselType): Promise<VesselType>;
    getNumberOfVesselTypes(): Promise<number>;
    
}