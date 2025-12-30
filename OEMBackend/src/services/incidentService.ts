import { Service } from "typedi";
import { IncidentRepository } from "../repository/incidentRepository";
import { CreateIncidentDto, IncidentDto, UpdateIncidentDto } from "../dto/incidentDto";
import { IncidentMapper } from "../mappers/incidentMapper";
import { incidentFilter } from "../dto/filters/incidentFilter";
import { Page } from "../utils/page";
import { PageMapper } from "../mappers/pageMapper";
import Incident, { IncidentSeverity } from "../domain/incident";
import IncidentType from "../domain/incidentType";
import { IncidentTypeRepository } from "../repository/incidentTypeRepository";

@Service("incidentService")
export class IncidentService {

	private mapper = new IncidentMapper();
	private incidentRepository : IncidentRepository;
	private incidentTypeRepository : IncidentTypeRepository;

	constructor() {
		this.incidentRepository = new IncidentRepository();
		this.incidentTypeRepository = new IncidentTypeRepository();
	}

	async count(): Promise<number> {
		return await this.incidentRepository.count();
	}

	async getByBid(bid: string) : Promise<IncidentDto> {
		return this.mapper.toDto(await this.incidentRepository.getByBid(bid));
	}

	async getPaged(filter : incidentFilter) : Promise<Page<IncidentDto>> {
		let domainDataPage = await this.incidentRepository.getPaged(filter);
		return PageMapper.itemsToDto(domainDataPage, this.mapper);
	}

	async create(newIncident : CreateIncidentDto, createdBy: string) : Promise<IncidentDto> {
		let incidentType : IncidentType = await this.incidentTypeRepository.getById(newIncident.type);
		let incidentSeverity = newIncident.severity as IncidentSeverity;
		
		let domainIncident = new Incident(
			{
				type: incidentType,
				startTime: new Date(newIncident.startTime),
				severity: incidentSeverity,
				description: newIncident.description,
				createdBy: createdBy
			}
		);

		let savedIncident = await this.incidentRepository.save(domainIncident);
		return this.mapper.toDto(savedIncident);
	}

	async update(bid: string, UpdateIncidentDto: Partial<UpdateIncidentDto>) : Promise<IncidentDto> {
		let domainIncident = await this.incidentRepository.getByBid(bid);

		if (UpdateIncidentDto.type)
			domainIncident.type = await this.incidentTypeRepository.getById(UpdateIncidentDto.type);

		if (UpdateIncidentDto.startTime){

            if (domainIncident.endTime)
                if (new Date(UpdateIncidentDto.startTime) > domainIncident.endTime!) 
                    throw new Error("Start time can't be after end time");
			domainIncident.startTime = new Date(UpdateIncidentDto.startTime);
        }

		if (UpdateIncidentDto.endTime){
            if (domainIncident.startTime > new Date(UpdateIncidentDto.endTime)) 
                throw new Error("Start time can't be after end time");
			domainIncident.endTime = new Date(UpdateIncidentDto.endTime);
        }

		if (UpdateIncidentDto.severity)
			domainIncident.severity = UpdateIncidentDto.severity as IncidentSeverity;

		if (UpdateIncidentDto.description)
			domainIncident.description = UpdateIncidentDto.description;

		if (UpdateIncidentDto.affectedVVECodes)
			domainIncident.affectedVVECodes = UpdateIncidentDto.affectedVVECodes;

		let UpdatedIncident = await this.incidentRepository.update(domainIncident);
		return this.mapper.toDto(UpdatedIncident);
	}




}