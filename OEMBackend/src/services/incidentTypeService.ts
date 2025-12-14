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

	private async validateHierarchy(typeId: string, subtypeOfId?: string, subtypesIds?: string[]): Promise<void> {

		if (subtypeOfId && subtypeOfId === typeId) {
			throw new Error('A type cannot be a subtype of itself');
		}

		if (subtypesIds) {
			for (const subtypeId of subtypesIds) {
				if (subtypeId === typeId) {
					throw new Error('A type cannot have itself as a subtype');
				}
			}
		}

		if (subtypeOfId) {
			await this.checkCircularDependency(typeId, subtypeOfId);
		}

		if (subtypesIds && subtypesIds.length > 0) {
			for (const subtypeId of subtypesIds) {
				const isDescendant = await this.isDescendantOf(subtypeId, typeId);
				if (isDescendant) {
					throw new Error(`Type ${subtypeId} is already a descendant of ${typeId}. Subtypes cannot skip layers.`);
				}

				const existingSubtype = await this.incidentTypeRepository.getById(subtypeId);
				if (existingSubtype && existingSubtype.subtypeOfId && existingSubtype.subtypeOfId !== typeId) {
					throw new Error(`Type ${subtypeId} already has a parent (${existingSubtype.subtypeOfId}). A type can only have one parent.`);
				}
			}
		}
	}

	private async checkCircularDependency(typeId: string, subtypeOfId: string): Promise<void> {
		let currentId: string | undefined = subtypeOfId;
		const visited = new Set<string>([typeId]);

		while (currentId) {
			if (visited.has(currentId)) {
				throw new Error('Circular dependency detected in type hierarchy');
			}
			visited.add(currentId);

			const currentType = await this.incidentTypeRepository.getById(currentId);
			currentId = currentType?.subtypeOfId;
		}
	}

	private async isDescendantOf(potentialDescendant: string, ancestorId: string): Promise<boolean> {
		const descendant = await this.incidentTypeRepository.getById(potentialDescendant);
		if (!descendant) return false;

		let currentId: string | undefined = descendant.subtypeOfId;

		while (currentId) {
			if (currentId === ancestorId) {
				return true;
			}
			const currentType = await this.incidentTypeRepository.getById(currentId);
			currentId = currentType?.subtypeOfId;
		}

		return false;
	}

	async createIncidentType(name: string, subtypeOfId?: string, subtypesIds?: string[]): Promise<IncidentTypeDto> {
		const incidentType = new IncidentType({ name });
		
		await this.validateHierarchy(incidentType.id, subtypeOfId, subtypesIds);
		
		return await this.incidentTypeRepository.create(incidentType, subtypeOfId, subtypesIds);
	}

	async getIncidentTypeById(id: string): Promise<IncidentTypeDto | null> {
		return await this.incidentTypeRepository.getById(id);
	}

	async getAllIncidentTypes(): Promise<IncidentTypeDto[]> {
		return await this.incidentTypeRepository.getAll();
	}

	async updateIncidentType(id: string, name?: string, subtypesIds?: string[]): Promise<IncidentTypeDto | null> {

		if (subtypesIds) {
			await this.validateHierarchy(id, undefined, subtypesIds);
		}
		
		return await this.incidentTypeRepository.update(id, name, subtypesIds);
	}

	async removeSubtype(id: string, subtypeId: string): Promise<IncidentTypeDto | null> {
		return await this.incidentTypeRepository.removeSubtype(id, subtypeId);
	}

	async deleteIncidentType(id: string): Promise<boolean> {
		return await this.incidentTypeRepository.deleteById(id);
	}

}