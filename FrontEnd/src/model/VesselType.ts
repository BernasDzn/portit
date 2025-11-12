import type { VesselTypeDto } from './dto/VesselTypeDto';
import type { PhysicalCharacteristics } from './values/PhysicalCharacteristics';

export class VesselType {
    
    readonly name: string;
    readonly description: string;
    readonly maxNumberOfRows: number;
    readonly maxNumberOfBays: number;
    readonly maxNumberOfTiers: number;
    readonly capacity?: number;
    readonly physicalCharacteristics: PhysicalCharacteristics;

    constructor(params: {
        name: string;
        description: string;
        maxNumberOfRows: number;
        maxNumberOfBays: number;
        maxNumberOfTiers: number;
        capacity?: number;
        physicalCharacteristics: PhysicalCharacteristics;
    }) {
        this.name = params.name;
        this.description = params.description;
        this.maxNumberOfRows = params.maxNumberOfRows;
        this.maxNumberOfBays = params.maxNumberOfBays;
        this.maxNumberOfTiers = params.maxNumberOfTiers;
        this.capacity = params.capacity;
        this.physicalCharacteristics = params.physicalCharacteristics;
    }

    toDto(): VesselTypeDto {
        return {
            name: this.name,
            description: this.description,
            maxNumberOfRows: this.maxNumberOfRows,
            maxNumberOfBays: this.maxNumberOfBays,
            maxNumberOfTiers: this.maxNumberOfTiers,
            capacity: this.capacity,
            physicalCharacteristics: this.physicalCharacteristics,
        };
    }
}

