import type { VesselDto } from './dto/VesselDto';
import type { ShippingAgentOrganization } from './ShippingAgentOrganization';
import type { PhysicalCharacteristics } from './values/PhysicalCharacteristics';
import { VesselType } from './VesselType';

export class Vessel {
    
    readonly name: string;
    readonly imoNumber: string;
    readonly type: VesselType;
    readonly owner: ShippingAgentOrganization;
    readonly physicalCharacteristics: PhysicalCharacteristics;

    constructor(params: {
        name: string;
        imoNumber: string;
        type: VesselType;
        owner: ShippingAgentOrganization;
        physicalCharacteristics: PhysicalCharacteristics;
    }) {
        this.name = params.name;
        this.imoNumber = params.imoNumber;
        this.type = params.type;
        this.owner = params.owner;
        this.physicalCharacteristics = params.physicalCharacteristics;
    }

    toDto(): VesselDto {
        return {
            name: this.name,
            imoNumber: this.imoNumber,
            type: this.type.name,
            owner: this.owner.taxNumber,
            length: this.physicalCharacteristics.length,
            depth: this.physicalCharacteristics.depth,
            draft: this.physicalCharacteristics.draft,
        };
    }
}
