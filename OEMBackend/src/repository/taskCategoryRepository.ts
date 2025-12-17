import { TaskCategory } from "../domain/taskCategory";
import { TaskCategoryFilter } from "../dto/filters/taskCategoryFilter";
import { TaskCategoryDto } from "../dto/taskCategoryDto";
import { TaskCategoryMapper } from "../mappers/taskCategoryMapper";
import { TaskCategoryModel } from "../schemas/taskCategories";
import { Page } from "../utils/page";

export class TaskCategoryRepository {

    async createCategory(category: TaskCategory): Promise<TaskCategory> {
        const createdCategory = new TaskCategoryModel(TaskCategoryMapper.toSchema(category));
        const savedCategory = await createdCategory.save();
        return TaskCategoryMapper.fromSchema(savedCategory)!;
    }

    async updateCategory(category: TaskCategory): Promise<TaskCategory | null> {
        const updatedCategory = await TaskCategoryModel.findByIdAndUpdate(
            category.id,
            TaskCategoryMapper.toSchema(category),
            { new: true }
        ).exec();

        if (!updatedCategory)
            return null;

        return TaskCategoryMapper.fromSchema(updatedCategory);
    }

    async getCategoryByCode(categoryCode: string): Promise<TaskCategory | null> {
        
        const entry = await TaskCategoryModel.findOne({ category: categoryCode }).exec();
        if (!entry)
            return null;
        return TaskCategoryMapper.fromSchema(entry);
    }

    async getCategoryById(id: string): Promise<TaskCategory | null> {

        const entry = await TaskCategoryModel.findById(id).exec();
        if (!entry)
            return null;
        return TaskCategoryMapper.fromSchema(entry);
    }

    async getAllCategories(pageable: TaskCategoryFilter): Promise<Page<TaskCategoryDto>> {

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
                return (TaskCategoryMapper.fromSchema(doc)!).toDto();
            }))
        };
    }
}

export const taskCategoryRepository = new TaskCategoryRepository();