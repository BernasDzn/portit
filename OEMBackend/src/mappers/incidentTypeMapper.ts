import IncidentType from "../domain/incidentType";

export class IncidentTypeMapper {

	static toSchema(incidentType: IncidentType): any {
		return {
			id: incidentType.id,
			name: incidentType.name,
			description: incidentType.description,
			severity: incidentType.severity,
			subtypeOf: incidentType.subtypeOf ? incidentType.subtypeOf.id : null,
			subtypes: incidentType.subtypes ? incidentType.subtypes.map(subtype => subtype.id) : []
		};
	}

	static fromSchema(schema: any): IncidentType {
		const incidentType = new IncidentType(
			{
				name: schema.name,
				description: schema.description,
				severity: schema.severity
			},
			schema.id
		);
		return incidentType;
	}

}