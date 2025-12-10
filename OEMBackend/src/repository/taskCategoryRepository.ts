import { TaskCategory } from "../domain/taskCategory";
import { TaskCategoryMapper } from "../mappers/taskCategoryMapper";
import { TaskCategoryModel } from "../schemas/taskCategories";

export class TaskCategoryRepository {
    static async getCategoryById(operationType: any): Promise<TaskCategory | null> {
        
        const entry = await TaskCategoryModel.findById(operationType).exec();
        if (!entry)
            return null;
        return TaskCategoryMapper.fromSchema(entry);
    }

    async getCategoryByCode(categoryCode: string): Promise<TaskCategory | null> {
        
        const entry = await TaskCategoryModel.findOne({ category: categoryCode }).exec();
        if (!entry)
            return null;
        return TaskCategoryMapper.fromSchema(entry);
    }
}

export const taskCategoryRepository = new TaskCategoryRepository();