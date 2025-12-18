import { IncidentTypeRepository } from "../../repository/incidentTypeRepository";


export class IncidentTypeValidator {

	incidentTypeRepository : IncidentTypeRepository;

	constructor(
		incidentTypeRepository : IncidentTypeRepository
	) {
		this.incidentTypeRepository = incidentTypeRepository;
	}

	public async validateHierarchy(
		typeId: string,
		parentId?: string,
		subtypesIds?: string[]
	): Promise<void> {

		this.ensureNotSelfReferencing(typeId, parentId, subtypesIds);
		this.ensureParentNotAlsoSubtype(parentId, subtypesIds);

		if (parentId) {
			await this.ensureNoCircularParenting(typeId, parentId);
		}

		if (parentId && subtypesIds?.length) {
			await this.ensureParentIsNotDescendantOfSubtypes(parentId, subtypesIds);
		}

		if (subtypesIds?.length) {
			await this.validateSubtypes(typeId, subtypesIds);
		}
	}

	private ensureNotSelfReferencing(
		typeId: string,
		parentId?: string,
		subtypesIds?: string[]
	): void {
		if (parentId === typeId) {
			throw new Error('A type cannot be a subtype of itself');
		}

		if (subtypesIds?.includes(typeId)) {
			throw new Error('A type cannot have itself as a subtype');
		}
	}

	private ensureParentNotAlsoSubtype(
		parentId?: string,
		subtypesIds?: string[]
	): void {
		if (parentId && subtypesIds?.includes(parentId)) {
			throw new Error('Invalid hierarchy: the same type cannot be both parent and subtype.');
		}
	}

	private async ensureNoCircularParenting(
		typeId: string,
		parentId: string
	): Promise<void> {
		let currentId: string | undefined = parentId;
		const visited = new Set<string>([typeId]);

		while (currentId) {
			if (visited.has(currentId)) {
				throw new Error('Circular dependency detected in type hierarchy');
			}

			visited.add(currentId);
			const current = await this.incidentTypeRepository.getById(currentId);
			currentId = current?.subtypeOf?.id;
		}
	}

	private async ensureParentIsNotDescendantOfSubtypes(
		parentId: string,
		subtypesIds: string[]
	): Promise<void> {
		for (const subtypeId of subtypesIds) {
			const parentIsDescendant = await this.isDescendantOf(parentId, subtypeId);
			if (parentIsDescendant) {
				throw new Error(
					`Invalid hierarchy: parent ${parentId} is already a descendant of subtype ${subtypeId}.`
				);
			}
		}
	}

	private async validateSubtypes(
		typeId: string,
		subtypesIds: string[]
	): Promise<void> {
		for (const subtypeId of subtypesIds) {

			// Check if subtypeId is a descendant, but allow if it's a DIRECT child (existing relationship)
			const existingSubtype = await this.incidentTypeRepository.getById(subtypeId);
			const isDirectChild = existingSubtype?.subtypeOf?.id === typeId;
			
			if (!isDirectChild && await this.isDescendantOf(subtypeId, typeId)) {
				throw new Error(
					`Type ${subtypeId} is already a descendant of ${typeId}. Subtypes cannot skip layers.`
				);
			}

			if (await this.isDescendantOf(typeId, subtypeId)) {
				throw new Error(
					`Assigning ${subtypeId} as a subtype of ${typeId} would create a circular hierarchy.`
				);
			}

			// Only throw error if the subtype has a DIFFERENT parent
			if (existingSubtype?.subtypeOf?.id && existingSubtype.subtypeOf?.id !== typeId) {
				throw new Error(
					`Type ${subtypeId} already has a parent (${existingSubtype.subtypeOf?.id}). A type can only have one parent.`
				);
			}
		}
	}

	private async isDescendantOf(
		potentialDescendantId: string,
		ancestorId: string
	): Promise<boolean> {

		let currentId: string | undefined = potentialDescendantId;

		while (currentId) {
			const current = await this.incidentTypeRepository.getById(currentId);
			if (!current?.subtypeOf?.id) {
				return false;
			}

			if (current.subtypeOf?.id === ancestorId) {
				return true;
			}

			currentId = current.subtypeOf?.id;
		}

		return false;
	}


}