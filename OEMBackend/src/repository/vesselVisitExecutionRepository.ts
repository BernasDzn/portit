import VesselVisitExecution from "../domain/vesselVisitExecution";
import { VesselVisitExecutionMapper } from "../mappers/vesselVisitExecutionMapper";
import { VesselVisitExecutionModel } from "../schemas/vesselVisitExecutionSchema";
import { Page } from "../utils/page";
import { TaskCategoryRepository } from "./taskCategoryRepository";
import { VesselVisitExecutionFilter } from "../dto/filters/vesselVisitExecutionFilter";

export class VesselVisitExecutionRepository {
    
    private taskCategoryRepository: TaskCategoryRepository;

    constructor() {
        this.taskCategoryRepository = new TaskCategoryRepository();
    }

    async createVesselVisitExecution(vve: VesselVisitExecution): Promise<VesselVisitExecution> {
        const createdVVE = await VesselVisitExecutionMapper.toSchema(vve);
        const createdDoc = await VesselVisitExecutionModel.create(createdVVE);

        const vesselVisitExecution = await VesselVisitExecutionMapper.fromSchema(createdDoc, this.taskCategoryRepository);
        return vesselVisitExecution;
    }

    async getById(id: string): Promise<VesselVisitExecution | null> {
        const doc = await VesselVisitExecutionModel.findById(id).exec();
        if (!doc)
            return null;
        return VesselVisitExecutionMapper.fromSchema(doc, this.taskCategoryRepository);
    }

    async getByVVN(relatedVVN: string): Promise<VesselVisitExecution | null> {
        const doc = await VesselVisitExecutionModel.findOne({ relatedVVN: relatedVVN }).exec();
        if (!doc)
            return null;
        return VesselVisitExecutionMapper.fromSchema(doc, this.taskCategoryRepository);
    }

    async updateVesselVisitExecution(vve: VesselVisitExecution): Promise<VesselVisitExecution> {
        const updatedVVE = await VesselVisitExecutionMapper.toSchema(vve);
        const updatedDoc = await VesselVisitExecutionModel.findByIdAndUpdate(vve.id, updatedVVE, { new: true }).exec();
        if (!updatedDoc) {
            throw new Error(`Vessel Visit Execution with id ${vve.id} not found for update.`);
        }
        
        return VesselVisitExecutionMapper.fromSchema(updatedDoc, this.taskCategoryRepository);
    }

    async count(): Promise<number> {
        return await VesselVisitExecutionModel.countDocuments().exec();
    }

    async getAllVesselVisitExecutions(filter: VesselVisitExecutionFilter): Promise<Page<VesselVisitExecution>> {
        const pageNumber = filter.pageNumber && filter.pageNumber > 0 ? filter.pageNumber : 1;
        const pageSize = filter.pageSize && filter.pageSize > 0 ? filter.pageSize : 10;
        const skip = (pageNumber - 1) * pageSize;

        // build a query filter object so date range and other filters are applied correctly
        const queryFilter: any = {};

        const parseDate = (d?: string) => {
            if (!d) return null;
            const dt = new Date(d);
            return isNaN(dt.getTime()) ? null : dt;
        };

        const start = parseDate(filter.startDate);
        const end = parseDate(filter.endDate);

        if (start || end) {
            queryFilter.dateOpen = {};
            if (start) queryFilter.dateOpen.$gte = start;
            if (end) queryFilter.dateOpen.$lte = end;
        }

        if (filter.relatedVVN) {
            // partial match, case-insensitive
            queryFilter.relatedVVN = { $regex: filter.relatedVVN, $options: 'i' };
        }

        if (filter.status) {
            queryFilter.status = filter.status;
        }

        const query = VesselVisitExecutionModel.find(queryFilter)
            .sort({ 'updatedAt': -1 })
            .skip(skip)
            .limit(pageSize);

        const docs = await query.exec();
        const totalItems = await VesselVisitExecutionModel.countDocuments(queryFilter);

        return {
            pageNumber,
            pageSize,
            pageCount: Math.ceil(totalItems / pageSize),
            items: await Promise.all(docs.map(async (doc) => {
                return VesselVisitExecutionMapper.fromSchema(doc, this.taskCategoryRepository);
            }))
        };
    }
}

export const vesselVisitExecutionRepository = new VesselVisitExecutionRepository();