import { BaseMapper } from "../core/infra/baseMapper";
import Incident from "../domain/incident";
import { IncidentDto } from "../dto/incidentDto";
import { IncidentTypeRepository } from "../repository/incidentTypeRepository";
import { VesselVisitExecutionRepository } from "../repository/vesselVisitExecutionRepository";
import { IncidentModel } from "../schemas/incidentSchema";
import { IncidentTypeMapper } from "./incidentTypeMapper";
import { VesselVisitExecutionMapper } from "./vesselVisitExecutionMapper";


export class IncidentMapper extends BaseMapper<Incident, IncidentDto> {

    fromSchema(schema: any): Incident {
        throw new Error("Method not implemented.");
    }

	toDto(incident: Incident): IncidentDto {
        console.log(incident.affectedVVECodes);
		const dto: IncidentDto = {
			bid: incident.bid,
			type: new IncidentTypeMapper().toDto(incident.type),
			startTime: incident.startTime.toISOString(),
			endTime: incident.endTime ? incident.endTime.toISOString() : undefined,
			severity: incident.severity,
			description: incident.description,
			createdBy: incident.createdBy,
			affectedVVECodes: incident.affectedVVECodes?.map(v => v.toDto())
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
			affectedVVECodes: incident.affectedVVECodes?.map(v => v.code)
		};

		return new IncidentModel(data);
	}

	async fromSchemaAsync(schema: any): Promise<Incident> {

        const incidentTypeRepo = new IncidentTypeRepository();
        const type = await incidentTypeRepo.getByObjectId(schema.type);

        const vveRepo = new VesselVisitExecutionRepository();

		const incident = new Incident(
			{
				bid: schema.bid,
				type: type,
				startTime: schema.startTime,
				endTime: schema.endTime,
				severity: schema.severity,
				description: schema.description,
				createdBy: schema.createdBy,
				affectedVVECodes: await Promise.all(schema.affectedVVECodes.map(
                    async (i: string) => vveRepo.getByCode(i)
                ))
			},
			schema._id
		);
		return incident;
	}

}