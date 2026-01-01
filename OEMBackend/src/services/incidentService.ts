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
import { VesselVisitExecutionRepository } from "../repository/vesselVisitExecutionRepository";

@Service("incidentService")
export class IncidentService {

	private mapper = new IncidentMapper();
	private incidentRepository : IncidentRepository;
	private incidentTypeRepository : IncidentTypeRepository;
    private vveRepository: VesselVisitExecutionRepository;

	constructor() {
        this.mapper = new IncidentMapper();
		this.incidentRepository = new IncidentRepository();
		this.incidentTypeRepository = new IncidentTypeRepository();
        this.vveRepository = new VesselVisitExecutionRepository();
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

		if (UpdateIncidentDto.type){

            const type = await this.incidentTypeRepository.getById(UpdateIncidentDto.type);
            if (!type) throw new Error("Type " + type + " was not found");
            
			domainIncident.type = type;
        }

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

		if (UpdateIncidentDto.affectedVVECodes){
            
            let vves = [];
            for (let index = 0; index < UpdateIncidentDto.affectedVVECodes.length; index++) {
                const element = UpdateIncidentDto.affectedVVECodes[index];
                
                if (!element) throw new Error("Invalid vve code");
                const vve = await this.vveRepository.getByCode(element);

                if (!vve) throw new Error("VVE of code " + element + " not found");
                if (vve.status == "Closed") throw new Error("VVE of code " + element + " was already closed");
                vves.push(vve);
            }

            domainIncident.affectedVVECodes = vves;
        }
		let UpdatedIncident = await this.incidentRepository.update(domainIncident);
        if (!UpdateIncidentDto)
            throw new Error("Failed to update incident");

		return this.mapper.toDto(UpdatedIncident);
	}
}