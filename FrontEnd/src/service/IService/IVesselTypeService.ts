import type { Filter, Page } from "@/model/Page";
import { type VesselType } from "../../model/VesselType";
import type { VesselTypeDto } from "@/model/dto/VesselTypeDto";

export interface IVesselTypeService {
    getVesselTypes(filtering?: Filter<VesselType>): Promise<Page<VesselType>>
    getVesselTypeByName(name: string): Promise<VesselType | undefined>;
    
    createVesselType(vesselType: VesselTypeDto): Promise<VesselType>;
    updateVesselType(vesselType: VesselTypeDto): Promise<VesselType>;

    getNumberOfVesselTypes(): Promise<number>;
}