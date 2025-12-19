import { Route, Tags, Controller, Get, Post, Put, Path, Body, Query } from "tsoa";
import { CreateTaskCategoryDto } from '../dto/taskCategoryDto';
import { TaskCategoryFilter } from '../dto/filters/taskCategoryFilter';
import { TaskCategoryService } from '../services/taskCategoryService';

@Route("task-categories")
@Tags("Task Categories")
export class TaskCategoryController extends Controller {
    private taskCategoryService = new TaskCategoryService();

    @Post()
    public async createCategory(@Body() categoryDto: CreateTaskCategoryDto) {
        const createdCategory = await this.taskCategoryService.createCategory(categoryDto);
        this.setStatus(201);
        return createdCategory;
    }

    @Put("{id}")
    public async updateCategory(@Path() id: string, @Body() categoryDto: CreateTaskCategoryDto) {
        const updatedCategory = await this.taskCategoryService.updateCategory(id, categoryDto);
        
        if (!updatedCategory) {
            this.setStatus(404);
            return { message: 'Task category not found' };
        }
        
        return updatedCategory;
    }
    
    @Get("code/{code}")
    public async getCategoryByCode(@Path() code: string) {
        const category = await this.taskCategoryService.getCategoryByCode(code);
        
        if (!category) {
            this.setStatus(404);
            return { message: 'Task category not found' };
        }
        
        return category;
    }
    @Get()
    public async getAllCategories(
        @Query() pageNumber: number = 0,
        @Query() pageSize: number = 10,
        @Query() name?: string
    ) {
        const filter: TaskCategoryFilter = {
            pageNumber: pageNumber,
            pageSize: pageSize,
            name: name,
        };
        
        const categories = await this.taskCategoryService.getAllCategories(filter);
        return categories;
    }
}

export const taskCategoryController = new TaskCategoryController();