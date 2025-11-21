import type { DockDto } from "./dto/DockDto";
import type { PhysicalCharacteristics } from "./values/PhysicalCharacteristics";
import type { VesselType } from "./VesselType";

export class Dock {
    private _code: string;
    private _name: string;
    private _location: string;
    private _physicalCharacteristics: PhysicalCharacteristics;
    private _supportedVesselTypes: VesselType[];

    constructor(params: {
        code: string;
        name: string;
        location: string;
        physicalCharacteristics: PhysicalCharacteristics;
        supportedVesselTypes: VesselType[];
    }) {
        this.validatePhysicalCharacteristics(params.physicalCharacteristics, params.supportedVesselTypes);

        if (!params.code) throw new Error('Code cannot be null or empty.');
        if (!params.name) throw new Error('Name cannot be null or empty.');
        if (!params.location) throw new Error('Location cannot be null or empty.');
        if (!params.physicalCharacteristics) throw new Error('Physical characteristics cannot be null.');
        if (!params.supportedVesselTypes || params.supportedVesselTypes.length === 0) {
            throw new Error('Invalid vessel types - at least one vessel type must be supported.');
        }

        this._code = params.code;
        this._name = params.name;
        this._location = params.location;
        this._physicalCharacteristics = params.physicalCharacteristics;
        this._supportedVesselTypes = params.supportedVesselTypes;
    }

    get code(): string { return this._code; }
    get name(): string { return this._name; }
    get location(): string { return this._location; }
    get physicalCharacteristics(): PhysicalCharacteristics { return this._physicalCharacteristics; }
    get supportedVesselTypes(): VesselType[] { return this._supportedVesselTypes; }

    updateName(newName: string): void {
        if (!newName) throw new Error('Name cannot be null or empty.');
        this._name = newName;
    }

    updateLocation(newLocation: string): void {
        if (!newLocation) throw new Error('Location cannot be null or empty.');
        this._location = newLocation;
    }

    updatePhysicalCharacteristics(physicalCharacteristics: PhysicalCharacteristics): void {
        this.validatePhysicalCharacteristics(physicalCharacteristics, this._supportedVesselTypes);
        this._physicalCharacteristics = physicalCharacteristics;
    }

    updateVesselTypes(newVesselTypes: VesselType[]): void {
        this.validatePhysicalCharacteristics(this._physicalCharacteristics, newVesselTypes);
        this._supportedVesselTypes = newVesselTypes;
    }

    private validatePhysicalCharacteristics(physicalCharacteristics: PhysicalCharacteristics, supportedVesselTypes: VesselType[]): void {
        if (!physicalCharacteristics) {
            throw new Error('Physical characteristics cannot be null.');
        }
        if (!supportedVesselTypes || supportedVesselTypes.length === 0) {
            throw new Error('Invalid vessel types - at least one vessel type must be supported.');
        }

        for (const vt of supportedVesselTypes) {
            if (physicalCharacteristics.length < vt.physicalCharacteristics.length) {
                throw new Error(`The dock's length is insufficient for the vessel type ${vt.name}.`);
            }
            if (physicalCharacteristics.depth < vt.physicalCharacteristics.depth) {
                throw new Error(`The dock's depth is insufficient for the vessel type ${vt.name}.`);
            }
            if (physicalCharacteristics.draft < vt.physicalCharacteristics.draft) {
                throw new Error(`The dock's max draft is insufficient for the vessel type ${vt.name}.`);
            }
        }
    }

    toDto(): DockDto {
        return {
            code: this._code,
            name: this._name,
            location: this._location,
            physicalCharacteristics: this._physicalCharacteristics,
            supportedVesselTypes: this._supportedVesselTypes.map(vt => vt.name),
        };
    }
}
