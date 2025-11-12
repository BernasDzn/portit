import type { QualificationDto } from './dto/QualificationDto';

export class Qualification {
    readonly idCode: string;
    readonly qualificationName: string;

    constructor(params: { idCode: string; qualificationName: string; }) {
        this.idCode = params.idCode;
        this.qualificationName = params.qualificationName;
    }

    get code(): string {
        return this.idCode;
    }

    get name(): string {
        return this.qualificationName;
    }

    get displayName(): string {
        return `${this.code} - ${this.name}`;
    }

    toDto(): QualificationDto {
        return {
            idCode: this.idCode,
            qualificationName: this.qualificationName,
        };
    }
}