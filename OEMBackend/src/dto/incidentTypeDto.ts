export interface IncidentTypeDto {
	id: string;
	name: string;
	parentId?: string | undefined;
	childrenIds?: string[] | undefined;
}