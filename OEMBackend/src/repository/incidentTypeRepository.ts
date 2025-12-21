import { NotFoundError } from "../core/infra/extraErrors";
import { IncidentTypeMapper } from "../mappers/incidentTypeMapper";
import { IncidentTypeModel } from "../schemas/incidentTypeSchema";
import { Page, Pageable } from "../utils/page";
import IncidentType from "../domain/incidentType";
import { IncidentTypeFilter } from "../dto/filters/incidentTypeFilter";

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
	 * @param filter the filter to be used, including the page number and size
	 * @returns a page of IncidentTypes
	 */
	async getPaged(filter: IncidentTypeFilter): Promise<Page<IncidentType>> {

		const data = await IncidentTypeModel.find()
			.where(filter.name ? { name: { $regex: filter.name, $options: 'i' } } : {})
			.where(filter.severity ? { severity: filter.severity } : {})
			.skip((filter.pageNumber - 1) * filter.pageSize)
			.limit(filter.pageSize)
			.populate('subtypeOf')
			.populate('subtypes');
		
		return {
			pageNumber: filter.pageNumber,
			pageSize: filter.pageSize,
			pageCount: Math.ceil(await this.count() / filter.pageSize),
			items: data.map(item => this.mapper.fromSchema(item))
		};
	}

	/**
	 * Gets an IncidentType by its id.
	 * 
	 * NOTE: This method doesn't catch potential errors, so the caller must handle them.
	 * @param bid the bussiness id of the IncidentType to fetch from db
	 * @returns the IncidentType with the given bid
	 */
	async getById(bid: string): Promise<IncidentType> {
		const data = await IncidentTypeModel.findOne({ bid: bid })
			.populate('subtypeOf').populate('subtypes');
		if (!data) throw new NotFoundError("IncidentType with bid " + bid + " not found.");
		return this.mapper.fromSchema(data);
	}

	/**
	 * Creates and saves a new IncidentType in the db.
	 * @param newIncidentType the IncidentType to save
	 * @returns the created IncidentType, as stored in the db
	 */
	async save(newIncidentType: IncidentType): Promise<IncidentType> {
		const created = await this.mapper.toPersistence(newIncidentType).save();
		if (!created) throw new Error("There was an error saving IncidentType " + newIncidentType.bid);
		return this.mapper.fromSchema(await this.getById(created.bid));
	}

	/**
	 * Updates an existing IncidentType in the db.
	 * @param incidentType the new incident type with updated data
	 * @returns the updated IncidentType as stored/updated in the db
	 */
	async update(incidentType: IncidentType): Promise<IncidentType> {
		
		const updatedData = await IncidentTypeModel.findOneAndUpdate(
			{ bid: incidentType.bid },
			this.mapper.toPersistence(incidentType),
			{ new: true }
		).exec();

		if (!updatedData) throw new Error("IncidentType " + incidentType.bid + " was not found.");
		return this.mapper.fromSchema(await this.getById(updatedData.bid));
	}

}