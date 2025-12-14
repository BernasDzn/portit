import { Service } from "typedi";
import { IncidentTypeRepository } from "../repository/incidentTypeRepository";
import IncidentType from "../domain/incidentType";
import { IncidentTypeDto } from "../dto/incidentTypeDto";

@Service("incidentTypeService")
export class IncidentTypeService {

	incidentTypeRepository: IncidentTypeRepository;

	constructor() {
		this.incidentTypeRepository = new IncidentTypeRepository();
	}

	async createIncidentType(name: string): Promise<IncidentTypeDto> {
		const incidentType = new IncidentType(
			{ name }
		);
		return await this.incidentTypeRepository.create(incidentType);
	}

	async getIncidentTypeById(id: string): Promise<IncidentTypeDto | null> {
		return await this.incidentTypeRepository.getById(id);
	}

	async getAllIncidentTypes(): Promise<IncidentTypeDto[]> {
		return await this.incidentTypeRepository.getAll();
	}

	async updateIncidentType(id: string, name?: string, childrenIds?: string[]): Promise<IncidentTypeDto | null> {
		return await this.incidentTypeRepository.update(id, name, childrenIds);
	}

	async removeChild(id: string, childId: string): Promise<IncidentTypeDto | null> {
		return await this.incidentTypeRepository.removeChild(id, childId);
	}

	async deleteIncidentType(id: string): Promise<boolean> {
		return await this.incidentTypeRepository.deleteById(id);
	}

}