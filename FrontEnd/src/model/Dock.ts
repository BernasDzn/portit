import type { DockDto } from "./dto/DockDto";
import type { PhysicalCharacteristics } from "./values/PhysicalCharacteristics";
import type { VesselType } from "./VesselType";

export class Dock {
    readonly code: string;
    readonly name: string;
    readonly location: string;
    readonly physicalCharacteristics: PhysicalCharacteristics;
    readonly supportedVesselTypes: VesselType[];

    constructor(params: {
        code: string;
        name: string;
        location: string;
        physicalCharacteristics: PhysicalCharacteristics;
        supportedVesselTypes: VesselType[];
    }) {
        this.code = params.code;
        this.name = params.name;
        this.location = params.location;
        this.physicalCharacteristics = params.physicalCharacteristics;
        this.supportedVesselTypes = params.supportedVesselTypes;
    }

    toDto(): DockDto {
        return {
            code: this.code,
            name: this.name,
            location: this.location,
            physicalCharacteristics: this.physicalCharacteristics,
            supportedVesselTypes: this.supportedVesselTypes.map(vt => vt.name),
        };
    }
}
