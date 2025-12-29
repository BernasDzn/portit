import { BaseMapper } from "../core/infra/baseMapper";
import Incident from "../domain/incident";
import { IncidentDto } from "../dto/incidentDto";
import { IncidentModel } from "../schemas/incidentSchema";


export class IncidentMapper extends BaseMapper<Incident, IncidentDto> {

	toDto(incident: Incident): IncidentDto {
		const dto: IncidentDto = {
			bid: incident.bid,
			type: incident.type.bid,
			startTime: incident.startTime.toISOString(),
			endTime: incident.endTime ? incident.endTime.toISOString() : undefined,
			severity: incident.severity,
			description: incident.description,
			createdBy: incident.createdBy,
			affectedVVECodes: incident.affectedVVECodes
		};
		return dto;
	}

	toPersistence(incident: Incident) {
		const data: any = {
			_id: incident._id,
			bid: incident.bid,
			type: incident.type._id,
			startTime: incident.startTime,
			endTime: incident.endTime,
			severity: incident.severity,
			description: incident.description,
			createdBy: incident.createdBy,
			affectedVVECodes: incident.affectedVVECodes
		};

		return new IncidentModel(data);
	}

	fromSchema(schema: any): Incident {
		const incident = new Incident(
			{
				bid: schema.bid,
				type: schema.type,
				startTime: schema.startTime,
				endTime: schema.endTime,
				severity: schema.severity,
				description: schema.description,
				createdBy: schema.createdBy,
				affectedVVECodes: schema.affectedVVECodes
			},
			schema._id
		);
		return incident;
	}

}