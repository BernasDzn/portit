import { BaseMapper } from "../core/infra/baseMapper";
import IncidentType from "../domain/incidentType";
import { IncidentTypeDto } from "../dto/incidentTypeDto";
import { IncidentTypeModel } from "../schemas/incidentTypeSchema";

export class IncidentTypeMapper extends BaseMapper<IncidentType, IncidentTypeDto> {

	toDto(incidentType: IncidentType): IncidentTypeDto {

		let parentBid : string | undefined = undefined;
		if(incidentType.subtypeOf) parentBid = incidentType.subtypeOf.bid;

		let subtypesBids : string[] | undefined = undefined;
		if(incidentType.subtypes){
			if(incidentType.subtypes.length > 0){
				subtypesBids = incidentType.subtypes.map(subtype => subtype.bid);
			}
		}

		let dto : IncidentTypeDto = {
			bid: incidentType.bid,
			name: incidentType.name,
			description: incidentType.description,
			severity: incidentType.severity,
			subtypeOf: parentBid,
			subtypes: subtypesBids
		};
		return dto;
	}

	toPersistence(incidentType: IncidentType) {
		const data: any = {
			_id: incidentType._id,
			bid: incidentType.bid,
			name: incidentType.name,
			description: incidentType.description,
			severity: incidentType.severity,
			// IMPORTANT: References must store ObjectId values not full objects/strings
			// This shit gave me an headache cuz of type conversion, do it like this 👇
			subtypeOf: incidentType.subtypeOf ? incidentType.subtypeOf._id : undefined,
			subtypes: incidentType.subtypes ? incidentType.subtypes.map(subtype => subtype._id) : undefined
		};

		return new IncidentTypeModel(data);
	}

	fromSchema(schema: any): IncidentType {
		const incidentType = new IncidentType(
			{
				bid: schema.bid,
				name: schema.name,
				description: schema.description,
				severity: schema.severity,
				subtypeOf: schema.subtypeOf ? schema.subtypeOf : undefined,
				subtypes: schema.subtypes ? schema.subtypes : undefined
			},
			schema._id
		);
		return incidentType;
	}

}