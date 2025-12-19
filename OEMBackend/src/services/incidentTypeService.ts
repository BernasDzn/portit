import { Service } from "typedi";
import { Page, Pageable } from "../utils/page";
import { IncidentTypeRepository } from "../repository/incidentTypeRepository";
import IncidentType, { Severity } from "../domain/incidentType";
import { IncidentTypeCreateDto, IncidentTypeDto } from "../dto/incidentTypeDto";
import { PageMapper } from "../mappers/pageMapper";
import { IncidentTypeMapper } from "../mappers/incidentTypeMapper";

@Service("incidentTypeService")
export class IncidentTypeService {

	private mapper = new IncidentTypeMapper();
	private incidentTypeRepository: IncidentTypeRepository;

	constructor() {
		this.incidentTypeRepository = new IncidentTypeRepository();
	}

	async count(): Promise<number> {
		return await this.incidentTypeRepository.count();
	}

	async getPaged(pageable: Pageable): Promise<Page<IncidentTypeDto>> {
		let domainDataPage = await this.incidentTypeRepository.getPaged(pageable);
		return PageMapper.itemsToDto<IncidentType, IncidentTypeDto>(domainDataPage, this.mapper);
	}

	async getById(id: string): Promise<IncidentTypeDto> {
		let domainData = await this.incidentTypeRepository.getById(id);
		return this.mapper.toDto(domainData);
	}

	async create(incidentType: IncidentTypeDto): Promise<IncidentTypeDto> {

		let parentType: IncidentType | undefined = undefined;
		if(incidentType.subtypeOf != undefined){
			parentType = await this.incidentTypeRepository.getById(incidentType.subtypeOf);
			if(!parentType)
				throw new Error("Parent Incident Type with id " + incidentType.subtypeOf + " not found.");
		}

		let newIncidentType = new IncidentType({
			name: incidentType.name,
			description: incidentType.description,
			severity: incidentType.severity as Severity,
			subtypeOf: parentType,
			subtypes: undefined
		}); // Will throw error if trying to assing self as parent

		let created =  await this.incidentTypeRepository.save(newIncidentType);
		return this.mapper.toDto(created);
	}

	async update(id : string, newIncidentTypeData: IncidentTypeCreateDto): Promise<IncidentTypeDto> {

		let incidentTypeDto = await this.getById(id);
		if(!incidentTypeDto)
			throw new Error("Incident Type with id " + id + " not found.");

		let parentType: IncidentType | undefined = undefined;
		if(newIncidentTypeData.subtypeOf != undefined){
			parentType = await this.incidentTypeRepository.getById(newIncidentTypeData.subtypeOf);
			if(!parentType)
				throw new Error("Parent Incident Type with id " + newIncidentTypeData.subtypeOf + " not found.");
		}

		let incidentType = new IncidentType({
			name: newIncidentTypeData.name,
			description: newIncidentTypeData.description,
			severity: newIncidentTypeData.severity as Severity,
			subtypeOf: parentType,
			subtypes: undefined
		}, incidentTypeDto.id);
		
		let updated = await this.incidentTypeRepository.update(incidentType);
		return this.mapper.toDto(updated);
	}


}