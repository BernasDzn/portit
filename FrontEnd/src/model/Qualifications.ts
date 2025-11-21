import type { QualificationDto } from './dto/QualificationDto';

export class Qualification {
    private _idCode: string;
    private _qualificationName: string;

    constructor(params: { idCode: string; qualificationName: string; }) {
        if (!params.idCode) throw new Error('ID code cannot be null or empty.');
        if (!params.qualificationName) throw new Error('Qualification name cannot be null or empty.');

        this._idCode = params.idCode;
        this._qualificationName = params.qualificationName;
    }

    get idCode(): string { return this._idCode; }
    get qualificationName(): string { return this._qualificationName; }

    get code(): string {
        return this._idCode;
    }

    get name(): string {
        return this._qualificationName;
    }

    get displayName(): string {
        return `${this.code} - ${this.name}`;
    }

    updateQualificationName(qualificationName: string): void {
        if (!qualificationName) throw new Error('Qualification name cannot be null or empty.');
        this._qualificationName = qualificationName;
    }

    toDto(): QualificationDto {
        return {
            idCode: this._idCode,
            qualificationName: this._qualificationName,
        };
    }
}