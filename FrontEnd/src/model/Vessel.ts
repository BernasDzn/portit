import type { VesselDto } from './dto/VesselDto';
import type { ShippingAgentOrganization } from './ShippingAgentOrganization';
import type { PhysicalCharacteristics } from './values/PhysicalCharacteristics';
import { VesselType } from './VesselType';

export class Vessel {
    
    private _name: string;
    private _imoNumber: string;
    private _type: VesselType;
    private _owner: ShippingAgentOrganization;
    private _physicalCharacteristics: PhysicalCharacteristics;

    constructor(params: {
        name: string;
        imoNumber: string;
        type: VesselType;
        owner: ShippingAgentOrganization;
        physicalCharacteristics: PhysicalCharacteristics;
    }) {
        if (!params.name) throw new Error('Name cannot be null or empty.');
        if (!params.imoNumber) throw new Error('IMO Number cannot be null or empty.');
        if (!params.type) throw new Error('Vessel Type cannot be null.');
        if (!params.owner) throw new Error('Owner cannot be null.');
        if (!params.physicalCharacteristics) throw new Error('Physical Characteristics cannot be null.');

        this._name = params.name;
        this._imoNumber = params.imoNumber;
        this._type = params.type;
        this._owner = params.owner;
        this._physicalCharacteristics = params.physicalCharacteristics;

        this.validatePhysicalCharacteristics();
    }

    get name(): string { return this._name; }
    get imoNumber(): string { return this._imoNumber; }
    get type(): VesselType { return this._type; }
    get owner(): ShippingAgentOrganization { return this._owner; }
    get physicalCharacteristics(): PhysicalCharacteristics { return this._physicalCharacteristics; }

    updateName(name: string): void {
        if (!name) throw new Error('Name cannot be null or empty.');
        this._name = name;
    }

    updateImoNumber(imoNumber: string): void {
        if (!imoNumber) throw new Error('IMO Number cannot be null or empty.');
        this._imoNumber = imoNumber;
    }

    updateVesselType(vesselType: VesselType): void {
        if (!vesselType) throw new Error('Vessel Type cannot be null.');
        this._type = vesselType;
        this.validatePhysicalCharacteristics();
    }

    updateOwner(owner: ShippingAgentOrganization): void {
        if (!owner) throw new Error('Owner cannot be null.');
        this._owner = owner;
    }

    updatePhysicalCharacteristics(physicalCharacteristics: PhysicalCharacteristics): void {
        if (!physicalCharacteristics) throw new Error('Physical Characteristics cannot be null.');
        this._physicalCharacteristics = physicalCharacteristics;
        this.validatePhysicalCharacteristics();
    }

    private validatePhysicalCharacteristics(): void {
        if (this._physicalCharacteristics.length > this._type.physicalCharacteristics.length) {
            throw new Error(`The vessel's length exceeds the maximum length for the vessel type ${this._type.name}.`);
        }
        if (this._physicalCharacteristics.depth > this._type.physicalCharacteristics.depth) {
            throw new Error(`The vessel's depth exceeds the maximum depth for the vessel type ${this._type.name}.`);
        }
        if (this._physicalCharacteristics.draft > this._type.physicalCharacteristics.draft) {
            throw new Error(`The vessel's max draft exceeds the maximum draft for the vessel type ${this._type.name}.`);
        }
    }

    toDto(): VesselDto {
        return {
            name: this._name,
            imoNumber: this._imoNumber,
            type: this._type.name,
            owner: this._owner.taxNumber,
            length: this._physicalCharacteristics.length,
            depth: this._physicalCharacteristics.depth,
            draft: this._physicalCharacteristics.draft,
        };
    }
}
