import IncidentType, { Severity } from "../domain/incidentType";
import { IncidentTypeMapper } from "../mappers/incidentTypeMapper";
import { IncidentTypeModel } from "../schemas/incidentTypeSchema";
import { Page, Pageable } from "../utils/page";

export class IncidentTypeRepository {

	async create(incidentType: IncidentType, subtypeOfId?: string, subtypesIds?: string[]): Promise<IncidentType> {
		const newIncidentType = IncidentTypeMapper.toSchema(incidentType);
		
		if (subtypeOfId) {
			const parentDoc = await IncidentTypeModel.findOne({ id: subtypeOfId });
			if (parentDoc) {
				newIncidentType.subtypeOf = parentDoc._id;
			}
		}
		
		const createdIncidentType = new IncidentTypeModel(newIncidentType);
		await createdIncidentType.save();
		
		if (subtypesIds && subtypesIds.length > 0) {
			for (const subtypeId of subtypesIds) {
				const subtypeDoc = await IncidentTypeModel.findOne({ id: subtypeId });
				if (subtypeDoc) {
					subtypeDoc.subtypeOf = createdIncidentType._id;
					await subtypeDoc.save();
					createdIncidentType.subtypes.push(subtypeDoc._id);
				}
			}
			await createdIncidentType.save();
		}
		
		if (subtypeOfId) {
			const parentDoc = await IncidentTypeModel.findOne({ id: subtypeOfId });
			if (parentDoc && !parentDoc.subtypes.includes(createdIncidentType._id)) {
				parentDoc.subtypes.push(createdIncidentType._id);
				await parentDoc.save();
			}
		}
		
		const finalDoc = await IncidentTypeModel.findOne({ id: incidentType.id }).populate('subtypeOf').populate('subtypes');
		if (!finalDoc) throw new Error('Failed to create incident type');
		
		return IncidentTypeMapper.fromSchema(finalDoc);
	}

	async getById(id: string): Promise<IncidentType | null> {
		const doc = await IncidentTypeModel.findOne({ id }).populate('subtypeOf').populate('subtypes');
		if (!doc) return null;
		return IncidentTypeMapper.fromSchema(doc);
	}

	async getAll(pageable: Pageable): Promise<Page<IncidentType>> {
		//const docs = await IncidentTypeModel.find().populate('subtypeOf').populate('subtypes');
        const {pageNumber, pageSize} = pageable;
        const skip = (pageNumber - 1) * pageSize;

        const data = await IncidentTypeModel.find()
            .skip(skip)
            .limit(pageSize)
            .populate('subtypeOf')
            .populate('subtypes');

		return {
            pageNumber,
            pageSize,
            pageCount: Math.ceil(await IncidentTypeModel.countDocuments() / pageSize),
            items: data.map(doc => IncidentTypeMapper.fromSchema(doc))
        };
	}

	async update(id: string, name?: string, description?: string, severity?: Severity, subtypesIds?: string[]): Promise<IncidentType | null> {
		const doc = await IncidentTypeModel.findOne({ id });
		if (!doc) return null;

		if (name) {
			doc.name = name;
		}
		if (description !== undefined) {
			doc.description = description;
		}
		if (severity) {
			doc.severity = severity;
		}

		if (subtypesIds && subtypesIds.length > 0) {
			for (const subtypeId of subtypesIds) {
				const subtypeDoc = await IncidentTypeModel.findOne({ id: subtypeId });
				if (subtypeDoc && !doc.subtypes.includes(subtypeDoc._id)) {
					doc.subtypes.push(subtypeDoc._id);
					subtypeDoc.subtypeOf = doc._id;
					await subtypeDoc.save();
				}
			}
		}

		await doc.save();
		
		const updatedDoc = await IncidentTypeModel.findOne({ id }).populate('subtypeOf').populate('subtypes');
		if (!updatedDoc) return null;
		
		return IncidentTypeMapper.fromSchema(updatedDoc);
	}

	async removeSubtype(id: string, subtypeId: string): Promise<IncidentType | null> {
		const doc = await IncidentTypeModel.findOne({ id });
		if (!doc) return null;

		const subtypeDoc = await IncidentTypeModel.findOne({ id: subtypeId });
		if (!subtypeDoc) return null;

		doc.subtypes = doc.subtypes.filter((subtypeObjId: any) => !subtypeObjId.equals(subtypeDoc._id));
		subtypeDoc.subtypeOf = null;

		await doc.save();
		await subtypeDoc.save();

		const updatedDoc = await IncidentTypeModel.findOne({ id }).populate('subtypeOf').populate('subtypes');
		if (!updatedDoc) return null;
		
		return IncidentTypeMapper.fromSchema(updatedDoc);
	}

	async count(): Promise<number> {
		return await IncidentTypeModel.countDocuments();
	}

}