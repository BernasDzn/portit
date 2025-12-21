import { Service } from "typedi";
import { Page } from "../utils/page";
import { IncidentTypeRepository } from "../repository/incidentTypeRepository";
import IncidentType, { Severity } from "../domain/incidentType";
import { IncidentTypeDto, PartialIncidentTypeDto } from "../dto/incidentTypeDto";
import { PageMapper } from "../mappers/pageMapper";
import { IncidentTypeMapper } from "../mappers/incidentTypeMapper";
import { NotFoundError } from "../core/infra/extraErrors";
import { IncidentTypeFilter } from "../dto/filters/incidentTypeFilter";

@Service("incidentTypeService")
export class IncidentTypeService {

	private mapper = new IncidentTypeMapper();
	private incidentTypeRepository: IncidentTypeRepository;

	constructor() {
		this.incidentTypeRepository = new IncidentTypeRepository();
	}

	/**
	 * Counts all Incident Types in the system
	 * @returns Number of Incident Types
	 */
	async count(): Promise<number> {
		return await this.incidentTypeRepository.count();
	}

	/**
	 * Gets a paged list of Incident Types
	 * @param pageable Pagination information
	 * @returns Paged list of Incident Types
	 */
	async getPaged(filter : IncidentTypeFilter): Promise<Page<IncidentTypeDto>> {
		let domainDataPage = await this.incidentTypeRepository.getPaged(filter);
		return PageMapper.itemsToDto<IncidentType, IncidentTypeDto>(domainDataPage, this.mapper);
	}

	/**
	 * Gets an Incident Type by its id
	 * @param id Incident Type id
	 * @returns Incident Type Dto of the requested Incident Type
	 */
	async getById(id: string): Promise<IncidentTypeDto> {
		let domainData = await this.incidentTypeRepository.getById(id);
		return this.mapper.toDto(domainData);
	}

	/**
	 * Creates a new Incident Type
	 * @param incidentType Partial Incident Type data (no id)
	 * @returns Created Incident Type Dto
	 */
	async create(incidentType: PartialIncidentTypeDto): Promise<IncidentTypeDto> {

		let parentType: IncidentType | undefined = undefined;
		if(incidentType.subtypeOf != undefined){
			parentType = await this.incidentTypeRepository.getById(incidentType.subtypeOf);
			if(!parentType) throw new Error("Parent Incident Type with id " + incidentType.subtypeOf + " not found.");
		}

		let newIncidentType = new IncidentType({
			name: incidentType.name,
			description: incidentType.description,
			severity: incidentType.severity as Severity,
			subtypeOf: parentType
		}); // Will throw error if trying to assing self as parent

		if(parentType){
			parentType.addSubtype(newIncidentType)
			let parentUpdated = await this.incidentTypeRepository.update(parentType);
			if(!parentUpdated) throw new Error("There was an error adding " + newIncidentType.bid + " as subtype to " + parentType.bid);
		};

		let created =  await this.incidentTypeRepository.save(newIncidentType);
		console.log(created);
		return this.mapper.toDto(created);
	}

	/**
	 * Updates an existing Incident Type
	 * @param bid Incident Type id
	 * @param newIncidentTypeData Partial Incident Type data (no id)
	 * @returns Updated Incident Type Dto
	 */
	async update(bid : string, newIncidentTypeData: PartialIncidentTypeDto): Promise<IncidentTypeDto> {

		let existing = await this.incidentTypeRepository.getById(bid);
		if(!existing) throw new NotFoundError("Incident Type with id " + bid + " not found.");

		let parentType: IncidentType | undefined = undefined;
		if(newIncidentTypeData.subtypeOf != undefined){
			parentType = await this.incidentTypeRepository.getById(newIncidentTypeData.subtypeOf);
			if(!parentType) throw new NotFoundError("Parent Incident Type with id " + newIncidentTypeData.subtypeOf + " not found.");
		}

		// we could be updating a type that has subtypes, so we need to preserve them
		// since this is a put and not a patch.
		let subtypes : IncidentType[] = [];
		if(existing.subtypes && existing.subtypes.length > 0){
			for(let subtypeId of existing.subtypes){
				let subtype = await this.incidentTypeRepository.getById(subtypeId.bid);
				if(subtype) subtypes.push(subtype);
			}
		}

		let incidentType = new IncidentType({
			bid: existing.bid,
			name: newIncidentTypeData.name,
			description: newIncidentTypeData.description,
			severity: newIncidentTypeData.severity as Severity,
			subtypeOf: parentType,
			subtypes: subtypes
		}, existing._id); // Will throw error if trying to assing self as parent
		
		let updated = await this.incidentTypeRepository.update(incidentType);
		return this.mapper.toDto(updated);
	}


}