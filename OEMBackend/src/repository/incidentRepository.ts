import Incident from "../domain/incident";
import { NotFoundError } from "../core/infra/extraErrors";
import { IncidentMapper } from "../mappers/incidentMapper";
import { IncidentModel } from "../schemas/incidentSchema";
import { incidentFilter } from "../dto/filters/incidentFilter";
import { Page } from "../utils/page";

export class IncidentRepository {

	private mapper = new IncidentMapper();

	async count(): Promise<number> {
		return await IncidentModel.countDocuments();
	}

	async getByBid(bid: string): Promise<Incident> {
		const data = await IncidentModel.findOne({ bid: bid });
		if (!data) throw new NotFoundError("Incident with bid " + bid + " not found.");
		return await this.mapper.fromSchemaAsync(data);
	}

	async getPaged(filter : incidentFilter): Promise<Page<Incident>> {

		const conditions: any[] = [];

		if (filter.vveCode) {
			conditions.push({ affectedVVECodes: { $regex: filter.vveCode, $options: 'i' } });
		}

		if (typeof filter.severity === 'string' && filter.severity.length > 0) {
			conditions.push({ severity: filter.severity });
		}

		if (filter.isResolved === true) {
			conditions.push({ endTime: { $exists: true } });
		} else if (filter.isResolved === false) {
			conditions.push({ endTime: { $exists: false } });
		}

		if (filter.filterStartTime && filter.filterEndTime) {
			const start = new Date(filter.filterStartTime);
			const end = new Date(filter.filterEndTime);
			conditions.push({
				$or: [
					{ startTime: { $gte: start }, endTime: { $exists: true, $lte: end } },
					{ startTime: { $gte: start, $lte: end }, endTime: { $exists: false } }
				]
			});
		} else if (filter.filterStartTime) {
			conditions.push({ startTime: { $gte: new Date(filter.filterStartTime) } });
		} else if (filter.filterEndTime) {
			const end = new Date(filter.filterEndTime);
			conditions.push({ $or: [ { endTime: { $exists: true, $lte: end } }, { startTime: { $lte: end } } ] });
		}

		const query = conditions.length ? { $and: conditions } : {};

		const pageNumber = filter.pageNumber || 1;
		const pageSize = filter.pageSize || 10;

		const total = await IncidentModel.countDocuments(query);
		const pageCount = Math.ceil(total / pageSize);

		const results = await IncidentModel.find(query)
			.skip((pageNumber - 1) * pageSize)
			.limit(pageSize)
			.exec();

		return {
			items: await Promise.all(results.map(async (r) => {
                return this.mapper.fromSchemaAsync(r)
            })),
			pageNumber,
			pageSize,
			pageCount
		};
	}

	async save(newIncident: Incident): Promise<Incident> {
		const created = await this.mapper.toPersistence(newIncident).save();
		if (!created) throw new Error("There was an error saving Incident " + newIncident.bid);
		return await this.mapper.fromSchemaAsync(await this.getByBid(created.bid));
	}

	async update(incident: Incident): Promise<Incident> {
		
		const updatedData = await IncidentModel.findOneAndUpdate(
			{ bid: incident.bid },
			this.mapper.toPersistence(incident),
			{ new: true }
		).exec();

		if (!updatedData) throw new Error("Incident " + incident.bid + " was not found.");
		return incident;
	}


}