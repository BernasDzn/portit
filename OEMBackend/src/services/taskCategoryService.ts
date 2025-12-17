import { Service } from "typedi";
import { TaskCategoryRepository } from "../repository/taskCategoryRepository";
import { TaskCategory } from "../domain/taskCategory";
import { CreateTaskCategoryDto, TaskCategoryDto } from "../dto/taskCategoryDto";
import { Page } from "../utils/page";
import { TaskCategoryFilter } from "../dto/filters/taskCategoryFilter";

@Service("taskCategoryService")
export class TaskCategoryService {

	taskCategoryRepository: TaskCategoryRepository;

	constructor() {
		this.taskCategoryRepository = new TaskCategoryRepository();
	}

	async createCategory(category: CreateTaskCategoryDto): Promise<TaskCategoryDto> {

        const existingCategory = await this.taskCategoryRepository.getCategoryByCode(category.category);
        if (existingCategory) {
            throw new Error(`Task Category with code ${category.category} already exists.`);
        }

        return (await this.taskCategoryRepository.createCategory(new TaskCategory(
            {
                id: undefined,
                category: category.category,
                description: category.description,
                name: category.name
            }
        ))).toDto();
    }

    async updateCategory(id: string, category: CreateTaskCategoryDto): Promise<TaskCategoryDto | null> {

        const existingCategory = await this.taskCategoryRepository.getCategoryByCode(category.category);
        if (!existingCategory || existingCategory.id !== id) {
            throw new Error(`Task Category with code ${category.category} does not exist.`);
        }

        const updatedCategory = await this.taskCategoryRepository.updateCategory(new TaskCategory(
            {
                id: id,
                category: category.category,
                description: category.description,
                name: category.name
            }
        ));
        if (!updatedCategory)
            return null;
        return updatedCategory.toDto();
    }

    async getCategoryByCode(categoryCode: string): Promise<TaskCategoryDto | null> {
        const category = await this.taskCategoryRepository.getCategoryByCode(categoryCode);
        if (!category)
            return null;
        return category.toDto();
    }

    async getAllCategories(pageable: TaskCategoryFilter): Promise<Page<TaskCategoryDto>> {
        return this.taskCategoryRepository.getAllCategories(pageable);
    }

}