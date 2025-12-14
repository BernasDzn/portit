export type Severity = 'Minor' | 'Major' | 'Critical';

export interface IncidentTypeDto {
    id: string;
    name: string;
    description: string;
    severity: Severity;
    subtypeOfId?: string;
    subtypesIds?: string[];
}

export class IncidentType {
    private _id: string;
    private _name: string;
    private _description: string;
    private _severity: Severity;
    private _subtypeOfId?: string;
    private _subtypesIds?: string[];

    constructor(params: {
        id?: string;
        name: string;
        description: string;
        severity: Severity;
        subtypeOfId?: string;
        subtypesIds?: string[];
    }) {
        if (!params.name) throw new Error('Name cannot be null or empty.');
        if (!params.description) throw new Error('Description cannot be null or empty.');
        if (!params.severity) throw new Error('Severity cannot be null or empty.');

        this._id = params.id || '';
        this._name = params.name;
        this._description = params.description;
        this._severity = params.severity;
        this._subtypeOfId = params.subtypeOfId;
        this._subtypesIds = params.subtypesIds;
    }

    get id(): string { return this._id; }
    get name(): string { return this._name; }
    get description(): string { return this._description; }
    get severity(): Severity { return this._severity; }
    get subtypeOfId(): string | undefined { return this._subtypeOfId; }
    get subtypesIds(): string[] | undefined { return this._subtypesIds; }

    toDto(): IncidentTypeDto {
        return {
            id: this._id,
            name: this._name,
            description: this._description,
            severity: this._severity,
            subtypeOfId: this._subtypeOfId,
            subtypesIds: this._subtypesIds
        };
    }

    static fromDto(dto: IncidentTypeDto): IncidentType {
        return new IncidentType({
            id: dto.id,
            name: dto.name,
            description: dto.description,
            severity: dto.severity,
            subtypeOfId: dto.subtypeOfId,
            subtypesIds: dto.subtypesIds
        });
    }
}
