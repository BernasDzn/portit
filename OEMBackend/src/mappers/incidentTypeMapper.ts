import { BaseMapper } from "../core/infra/baseMapper";
import IncidentType from "../domain/incidentType";
import { IncidentTypeDto } from "../dto/incidentTypeDto";
import { IncidentTypeModel } from "../schemas/incidentTypeSchema";

export class IncidentTypeMapper extends BaseMapper<IncidentType, IncidentTypeDto> {

	toDto(incidentType: IncidentType): IncidentTypeDto {

		let dto : IncidentTypeDto = {
			id: incidentType.id,
			name: incidentType.name,
			description: incidentType.description,
			severity: incidentType.severity,
			subtypeOf: incidentType.subtypeOf ? incidentType.subtypeOf.id : undefined,
			subtypes: incidentType.subtypes ? incidentType.subtypes.map(subtype => subtype.id) : undefined
		};

		return dto;
	}

	toPersistence(incidentType: IncidentType) {
		return new IncidentTypeModel({
			id: incidentType.id,
			name: incidentType.name,
			description: incidentType.description,
			severity: incidentType.severity,
			subtypeOf: incidentType.subtypeOf ? incidentType.subtypeOf.id : null,
			subtypes: incidentType.subtypes ? incidentType.subtypes.map(subtype => subtype.id) : []
		});
	}

	fromSchema(schema: any): IncidentType {
		
		let subtypeOf: IncidentType | undefined = undefined;
		let subtypes: IncidentType[] | undefined = undefined;

		// Lazy load of parent subtype, probably already done by the DB but just in case
		if(schema.subtypeOf) subtypeOf = this.fromSchema(schema.subtypeOf);


		if(schema.subtypes) subtypes = schema.subtypes.map(
			(subtypeSchema: any) => this.fromSchema(subtypeSchema)
		);

		const incidentType = new IncidentType(
			{
				name: schema.name,
				description: schema.description,
				severity: schema.severity,
				subtypeOf: subtypeOf ? subtypeOf : undefined,
				subtypes: subtypes ? subtypes : undefined
			},
			schema.id
		);
		return incidentType;
	}

}