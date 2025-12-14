import IncidentType from "../domain/incidentType";
import { IncidentTypeDto } from "../dto/incidentTypeDto";
import { IncidentTypeMapper } from "../mappers/incidentTypeMapper";
import { IncidentTypeModel } from "../schemas/incidentTypeSchema";

export class IncidentTypeRepository {

	async create(incidentType : IncidentType) : Promise<IncidentTypeDto> {
		const newIncidentType = IncidentTypeMapper.toSchema(incidentType);
		const createdIncidentType = await IncidentTypeModel.create(newIncidentType);
		const type = IncidentTypeMapper.fromSchema(createdIncidentType);
		return type.toDto();
	}

	async getById(id: string) : Promise<IncidentTypeDto | null> {
		const doc = await IncidentTypeModel.findOne({ id }).populate('parent').populate('children');
		if (!doc) return null;
		const type = IncidentTypeMapper.fromSchema(doc);
		const dto = type.toDto();
		
		// Add parent and children IDs from the database document
		if (doc.parent && typeof doc.parent !== 'string') {
			dto.parentId = (doc.parent as any).id;
		}
		if (doc.children && doc.children.length > 0) {
			dto.childrenIds = doc.children.map((child: any) => 
				typeof child === 'string' ? child : child.id
			);
		}
		
		return dto;
	}

	async getAll() : Promise<IncidentTypeDto[]> {
		const docs = await IncidentTypeModel.find().populate('parent').populate('children');
		return docs.map(doc => {
			const type = IncidentTypeMapper.fromSchema(doc);
			const dto = type.toDto();
			
			// Add parent ID from the database document
			if (doc.parent && typeof doc.parent !== 'string') {
				dto.parentId = (doc.parent as any).id;
			}
			
			// Add children IDs from the database document
			if (doc.children && doc.children.length > 0) {
				dto.childrenIds = doc.children.map((child: any) => 
					typeof child === 'string' ? child : child.id
				);
			}
			
			return dto;
		});
	}

	async update(id: string, name?: string, childrenIds?: string[]): Promise<IncidentTypeDto | null> {
		const doc = await IncidentTypeModel.findOne({ id });
		if (!doc) return null;

		if (name) {
			doc.name = name;
		}

		if (childrenIds && childrenIds.length > 0) {
			for (const childId of childrenIds) {
				const childDoc = await IncidentTypeModel.findOne({ id: childId });
				if (childDoc && !doc.children.includes(childDoc._id)) {
					doc.children.push(childDoc._id);
					childDoc.parent = doc._id;
					await childDoc.save();
				}
			}
		}

		await doc.save();
		
		// Reload with populated references
		const updatedDoc = await IncidentTypeModel.findOne({ id }).populate('parent').populate('children');
		if (!updatedDoc) return null;
		
		const type = IncidentTypeMapper.fromSchema(updatedDoc);
		const dto = type.toDto();
		
		// Add parent and children IDs from the database document
		if (updatedDoc.parent && typeof updatedDoc.parent !== 'string') {
			dto.parentId = (updatedDoc.parent as any).id;
		}
		if (updatedDoc.children && updatedDoc.children.length > 0) {
			dto.childrenIds = updatedDoc.children.map((child: any) => 
				typeof child === 'string' ? child : child.id
			);
		}
		
		return dto;
	}

	async removeChild(id: string, childId: string): Promise<IncidentTypeDto | null> {
		const doc = await IncidentTypeModel.findOne({ id });
		if (!doc) return null;

		const childDoc = await IncidentTypeModel.findOne({ id: childId });
		if (!childDoc) return null;

		doc.children = doc.children.filter((childObjId: any) => !childObjId.equals(childDoc._id));
		childDoc.parent = null;

		await doc.save();
		await childDoc.save();

		// Reload with populated references
		const updatedDoc = await IncidentTypeModel.findOne({ id }).populate('parent').populate('children');
		if (!updatedDoc) return null;
		
		const type = IncidentTypeMapper.fromSchema(updatedDoc);
		const dto = type.toDto();
		
		// Add parent and children IDs from the database document
		if (updatedDoc.parent && typeof updatedDoc.parent !== 'string') {
			dto.parentId = (updatedDoc.parent as any).id;
		}
		if (updatedDoc.children && updatedDoc.children.length > 0) {
			dto.childrenIds = updatedDoc.children.map((child: any) => 
				typeof child === 'string' ? child : child.id
			);
		}
		
		return dto;
	}

	async deleteById(id: string): Promise<boolean> {
		const doc = await IncidentTypeModel.findOne({ id });
		if (!doc) return false;

		// Remove references from parent if this is a child
		if (doc.parent) {
			await IncidentTypeModel.updateOne(
				{ _id: doc.parent },
				{ $pull: { children: doc._id } }
			);
		}

		// Set parent to null for all children
		if (doc.children && doc.children.length > 0) {
			await IncidentTypeModel.updateMany(
				{ _id: { $in: doc.children } },
				{ $set: { parent: null } }
			);
		}

		await IncidentTypeModel.deleteOne({ id });
		return true;
	}

}