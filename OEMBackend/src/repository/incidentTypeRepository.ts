import IncidentType from "../domain/incidentType";
import { IncidentTypeMapper } from "../mappers/incidentTypeMapper";
import { IncidentTypeModel } from "../schemas/incidentTypeSchema";
import { Page, Pageable } from "../utils/page";

export class IncidentTypeRepository {

	private mapper = new IncidentTypeMapper();

	/**
	 * Counts the total number of IncidentTypes in the db
	 * @returns the number of IncidentTypes saved in the db
	 */
	async count(): Promise<number> {
		return await IncidentTypeModel.countDocuments();
	}

	/**
	 * Fetches a paged list of IncidentTypes from the db
	 * @param pageable the pagination info, including the page number and size
	 * @returns a page of IncidentTypes
	 */
	async getPaged(pageable: Pageable): Promise<Page<IncidentType>> {
		const data = await IncidentTypeModel.find()
			.skip((pageable.pageNumber - 1) * pageable.pageSize)
			.limit(pageable.pageSize)
			.populate('subtypeOf')
			.populate('subtypes');
		
		return {
			pageNumber: pageable.pageNumber,
			pageSize: pageable.pageSize,
			pageCount: Math.ceil(await this.count() / pageable.pageSize),
			items: data.map(item => this.mapper.fromSchema(item))
		};
	}

	/**
	 * Gets an IncidentType by its id.
	 * 
	 * NOTE: This method doesn't catch potential errors, so the caller must handle them.
	 * @param fetchId the id of the IncidentType to fetch from db
	 * @returns the IncidentType with the given id
	 */
	async getById(fetchId: string): Promise<IncidentType> {
		const data = await IncidentTypeModel.findOne({ id: fetchId })
			.populate('subtypeOf').populate('subtypes');
		return this.mapper.fromSchema(data);
	}

	/**
	 * Creates and saves a new IncidentType in the db.
	 * @param newIncidentType the IncidentType to save
	 * @returns the created IncidentType, as stored in the db
	 */
	async save(newIncidentType: IncidentType): Promise<IncidentType> {

		const created = await this.mapper.toPersistence(newIncidentType).save();
		if (!created) throw new Error("There was an error saving IncidentType " + newIncidentType.id);

		if(newIncidentType.hasParent()){
			let parent = await this.getById(newIncidentType.subtypeOf!.id);
			parent.addSubtype(this.mapper.fromSchema(created));
			let updatedParent = await this.update(parent);
			if(!updatedParent)
				throw new Error("There was an error adding " + newIncidentType.id + " as subtype to " + parent.id);
		} // We might need to revert creation on error here, but it's not critical nor likely to happen

		return this.mapper.fromSchema(created);
	}

	/**
	 * Updates an existing IncidentType in the db.
	 * @param incidentType the new incident type with updated data
	 * @returns the updated IncidentType as stored/updated in the db
	 */
	async update(incidentType: IncidentType): Promise<IncidentType> {
		const updatedData = await IncidentTypeModel.findByIdAndUpdate(
			incidentType.id,
			this.mapper.toPersistence(incidentType),
			{ new: true }
		).exec();

		if (!updatedData) throw new Error("IncidentType " + incidentType.id + " was not found.");
		
		return this.mapper.fromSchema(updatedData);
	}

}