import IncidentType from "../domain/incidentType";

export class IncidentTypeMapper {

	static toSchema(incidentType: IncidentType): any {
		return {
			id: incidentType.id,
			name: incidentType.name,
			parent: incidentType.parent ? incidentType.parent.id : null,
			children: incidentType.children ? incidentType.children.map(child => child.id) : []
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