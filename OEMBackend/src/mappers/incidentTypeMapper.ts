import IncidentType from "../domain/incidentType";

export class IncidentTypeMapper {

	static toSchema(incidentType: IncidentType): any {
		return {
			id: incidentType.id,
			name: incidentType.name,
			subtypeOf: incidentType.subtypeOf ? incidentType.subtypeOf.id : null,
			subtypes: incidentType.subtypes ? incidentType.subtypes.map(subtype => subtype.id) : []
		};
	}

	static fromSchema(schema: any): IncidentType {
		const incidentType = new IncidentType(
			{
				name: schema.name
			},
			schema.id
		);
		return incidentType;
	}

}