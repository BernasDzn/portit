import VesselVisitExecution from "../domain/vesselVisitExecution";
import { VesselVisitExecutionMapper } from "../mappers/vesselVisitExecutionMapper";
import { VesselVisitExecutionModel } from "../schemas/vesselVisitExecutionSchema";
import { Page } from "../utils/page";
import { TaskCategoryRepository } from "./taskCategoryRepository";

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
}

export const vesselVisitExecutionRepository = new VesselVisitExecutionRepository();