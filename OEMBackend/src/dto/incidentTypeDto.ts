export class IncidentTypeDto {
	bid!: string;
	name!: string;
	description!: string;
	severity!: string;
	subtypeOf?: string | null;
	subtypes?: string[];
}

// Create and Update DTO
export class PartialIncidentTypeDto {
	name!: string;
	description!: string;
	severity!: string;
	subtypeOf?: string | null;
}