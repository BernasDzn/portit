export class IncidentTypeDto {
	bid!: string;
	name!: string;
	description!: string;
	severity!: string;
	subtypeOf?: string;
	subtypes?: string[];
}

// Create and Update DTO
export class PartialIncidentTypeDto {
	name!: string;
	description!: string;
	severity!: string;
	subtypeOf?: string;
}