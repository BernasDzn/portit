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
		
		return (await this.incidentTypeRepository.create(incidentType, subtypeOfId, subtypesIds)).toDto();
	}

	async getIncidentTypeById(id: string): Promise<IncidentTypeDto | null> {
        const doc = await this.incidentTypeRepository.getById(id);
        return doc!.toDto();
	}

	async getAllIncidentTypes(pageable: Pageable): Promise<Page<IncidentTypeDto>> {
		const page = await this.incidentTypeRepository.getAll(pageable);
        
        return {
            items: page.items.map(item => item.toDto()),
            pageCount: page.pageCount,
            pageNumber: page.pageNumber,
            pageSize: page.pageSize
        };
	}

	async updateIncidentType(id: string, name?: string, description?: string, severity?: Severity, subtypesIds?: string[]): Promise<IncidentTypeDto> {

		if (subtypesIds) {
			await this.incidentTypeValidator.validateHierarchy(id, undefined, subtypesIds);
		}
		
		const doc = (await this.incidentTypeRepository.update(id, name, description, severity, subtypesIds))?.toDto();
        return doc!;
	}

	async count(): Promise<number> {
		return await this.incidentTypeRepository.count();
	}
}