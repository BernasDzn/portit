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
}

export const vesselVisitExecutionRepository = new VesselVisitExecutionRepository();