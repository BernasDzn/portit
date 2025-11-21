import type { VesselTypeDto } from './dto/VesselTypeDto';
import type { PhysicalCharacteristics } from './values/PhysicalCharacteristics';

export class VesselType {
    
    private _name: string;
    private _description: string;
    private _maxNumberOfRows: number;
    private _maxNumberOfBays: number;
    private _maxNumberOfTiers: number;
    private _physicalCharacteristics: PhysicalCharacteristics;

    constructor(params: {
        name: string;
        description: string;
        maxNumberOfRows: number;
        maxNumberOfBays: number;
        maxNumberOfTiers: number;
        capacity?: number;
        physicalCharacteristics: PhysicalCharacteristics;
    }) {
        if (!params.name) throw new Error('Name cannot be null or empty.');
        if (!params.description) throw new Error('Description cannot be null or empty.');
        if (!params.physicalCharacteristics) throw new Error('Physical characteristics cannot be null.');
        if (params.maxNumberOfBays === 0) throw new Error('Max number of bays must be greater than zero.');
        if (params.maxNumberOfRows === 0) throw new Error('Max number of rows must be greater than zero.');
        if (params.maxNumberOfTiers === 0) throw new Error('Max number of tiers must be greater than zero.');

        this._name = params.name;
        this._description = params.description;
        this._maxNumberOfRows = params.maxNumberOfRows;
        this._maxNumberOfBays = params.maxNumberOfBays;
        this._maxNumberOfTiers = params.maxNumberOfTiers;
        this._physicalCharacteristics = params.physicalCharacteristics;
    }

    get name(): string { return this._name; }
    get description(): string { return this._description; }
    get maxNumberOfRows(): number { return this._maxNumberOfRows; }
    get maxNumberOfBays(): number { return this._maxNumberOfBays; }
    get maxNumberOfTiers(): number { return this._maxNumberOfTiers; }
    get capacity(): number { return this._maxNumberOfRows * this._maxNumberOfBays * this._maxNumberOfTiers; }
    get physicalCharacteristics(): PhysicalCharacteristics { return this._physicalCharacteristics; }

    updateName(name: string): void {
        if (!name) throw new Error('Name cannot be null or empty.');
        this._name = name;
    }

    updateDescription(description: string): void {
        if (!description) throw new Error('Description cannot be null or empty.');
        this._description = description;
    }

    updateMaxNumberOfRows(maxNumberOfRows: number): void {
        if (maxNumberOfRows === 0) throw new Error('Max number of rows must be greater than zero.');
        this._maxNumberOfRows = maxNumberOfRows;
    }

    updateMaxNumberOfBays(maxNumberOfBays: number): void {
        if (maxNumberOfBays === 0) throw new Error('Max number of bays must be greater than zero.');
        this._maxNumberOfBays = maxNumberOfBays;
    }

    updateMaxNumberOfTiers(maxNumberOfTiers: number): void {
        if (maxNumberOfTiers === 0) throw new Error('Max number of tiers must be greater than zero.');
        this._maxNumberOfTiers = maxNumberOfTiers;
    }

    updatePhysicalCharacteristics(physicalCharacteristics: PhysicalCharacteristics): void {
        if (!physicalCharacteristics) throw new Error('Physical characteristics cannot be null.');
        this._physicalCharacteristics = physicalCharacteristics;
    }

    toDto(): VesselTypeDto {
        return {
            name: this._name,
            description: this._description,
            maxNumberOfRows: this._maxNumberOfRows,
            maxNumberOfBays: this._maxNumberOfBays,
            maxNumberOfTiers: this._maxNumberOfTiers,
            capacity: this.capacity,
            physicalCharacteristics: this._physicalCharacteristics,
        };
    }
}

