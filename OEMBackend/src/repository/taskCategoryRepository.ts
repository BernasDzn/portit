import { TaskCategory } from "../domain/taskCategory";
import { TaskCategoryFilter } from "../dto/filters/taskCategoryFilter";
import { TaskCategoryMapper } from "../mappers/taskCategoryMapper";
import { TaskCategoryModel } from "../schemas/taskCategories";
import { Page } from "../utils/page";

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

    async getAllCategories(pageable: TaskCategoryFilter): Promise<Page<TaskCategory>> {

        const { pageNumber, pageSize } = pageable;
		const skip = (pageNumber - 1) * pageSize;

        const name = pageable.name ? pageable.name : null;

        const data = await TaskCategoryModel.find()
            .where(name ? { name: { $regex: name, $options: 'i' } } : {})
            .sort({ 'updatedAt': -1 })
            .skip(skip)
            .limit(pageSize);

        return {
            pageNumber,
            pageSize,
            pageCount: Math.ceil(await TaskCategoryModel.countDocuments() / pageSize),
            items: await Promise.all(data.map(async (doc) => {
                return TaskCategoryMapper.fromSchema(doc)!;
            }))
        };
    }
}

export const taskCategoryRepository = new TaskCategoryRepository();