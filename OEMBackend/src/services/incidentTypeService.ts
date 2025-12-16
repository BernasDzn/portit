import { Service } from "typedi";
import { IncidentTypeRepository } from "../repository/incidentTypeRepository";
import IncidentType, { Severity } from "../domain/incidentType";
import { IncidentTypeDto } from "../dto/incidentTypeDto";
import { Page, Pageable } from "../utils/page";
import { IncidentTypeValidator } from "./validators/incidentTypeValidator";

@Service("incidentTypeService")
export class IncidentTypeService {

	incidentTypeRepository: IncidentTypeRepository;
	incidentTypeValidator : IncidentTypeValidator;

	constructor() {
		this.incidentTypeRepository = new IncidentTypeRepository();
		this.incidentTypeValidator = new IncidentTypeValidator(this.incidentTypeRepository);
	}

	async createIncidentType(name: string, severity: Severity, description: string, subtypeOfId?: string, subtypesIds?: string[]): Promise<IncidentTypeDto> {
		const incidentType = new IncidentType({ name, description, severity });
		
		await this.incidentTypeValidator.validateHierarchy(incidentType.id, subtypeOfId, subtypesIds);
		
		return await this.incidentTypeRepository.create(incidentType, subtypeOfId, subtypesIds);
	}

	async getIncidentTypeById(id: string): Promise<IncidentTypeDto | null> {
		return await this.incidentTypeRepository.getById(id);
	}

	async getAllIncidentTypes(pageable: Pageable): Promise<Page<IncidentTypeDto>> {
		return await this.incidentTypeRepository.getAll(pageable);
	}

	async updateIncidentType(id: string, name?: string, description?: string, severity?: Severity, subtypesIds?: string[]): Promise<IncidentTypeDto | null> {

		if (subtypesIds) {
			await this.incidentTypeValidator.validateHierarchy(id, undefined, subtypesIds);
		}
		
		return await this.incidentTypeRepository.update(id, name, description, severity, subtypesIds);
	}

	async removeSubtype(id: string, subtypeId: string): Promise<IncidentTypeDto | null> {
		return await this.incidentTypeRepository.removeSubtype(id, subtypeId);
	}

	async deleteIncidentType(id: string): Promise<boolean> {
		return await this.incidentTypeRepository.deleteById(id);
	}

	async count(): Promise<number> {
		return await this.incidentTypeRepository.count();
	}
}