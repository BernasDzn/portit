import { Request, Response, NextFunction } from 'express';
import { Inject, Service } from 'typedi';
import { CreateTaskCategoryDto, TaskCategoryDto } from '../dto/taskCategoryDto';
import { TaskCategoryFilter } from '../dto/filters/taskCategoryFilter';
import { TaskCategoryService } from '../services/taskCategoryService';

@Service()
export class TaskCategoryController {
    taskCategoryService: TaskCategoryService;

    constructor(
        @Inject("taskCategoryService") private taskCategoryServiceInjected?: TaskCategoryService
    ) {
        this.taskCategoryService = this.taskCategoryServiceInjected!;
    }

    /**
     * @swagger
     * /task-categories:
     *   post:
     *     summary: Create a new task category
     *     tags:
     *       - Task Categories
     *     requestBody:
     *       required: true
     *       content:
     *         application/json:
     *           schema:
     *             $ref: '#/components/schemas/CreateTaskCategoryDto'
     *     responses:
     *       201:
     *         description: Task category created successfully
     *         content:
     *           application/json:
     *             schema:
     *               $ref: '#/components/schemas/TaskCategoryDto'
     *       400:
     *         description: Invalid request body
     *       500:
     *         description: Server error
     */
    async createCategory(req: Request, res: Response, next: NextFunction): Promise<void> {
        try {
            const categoryDto: CreateTaskCategoryDto = req.body;
            const createdCategory = await this.taskCategoryService.createCategory(categoryDto);
            res.status(201).json(createdCategory);
        } catch (error) {
            next(error);
        }
    }

    /**
     * @swagger
     * /task-categories/{id}:
     *   put:
     *     summary: Update an existing task category
     *     tags:
     *       - Task Categories
     *     parameters:
     *       - in: path
     *         name: id
     *         required: true
     *         schema:
     *           type: string
     *         description: Task category ID
     *     requestBody:
     *       required: true
     *       content:
     *         application/json:
     *           schema:
     *             $ref: '#/components/schemas/CreateTaskCategoryDto'
     *     responses:
     *       200:
     *         description: Task category updated successfully
     *         content:
     *           application/json:
     *             schema:
     *               $ref: '#/components/schemas/TaskCategoryDto'
     *       400:
     *         description: Category ID is required
     *       404:
     *         description: Task category not found
     *       500:
     *         description: Server error
     */
    async updateCategory(req: Request, res: Response, next: NextFunction): Promise<void> {
        try {
            const categoryDto: CreateTaskCategoryDto = req.body;
            const id = req.params.id;
            if (!id) {
                res.status(400).json({ message: 'Category ID is required' });
                return;
            }

            const updatedCategory = await this.taskCategoryService.updateCategory(id, categoryDto);
            
            if (!updatedCategory) {
                res.status(404).json({ message: 'Task category not found' });
                return;
            }
            
            res.status(200).json(updatedCategory);
        } catch (error) {
            next(error);
        }
    }
    
    /**
     * @swagger
     * /task-categories/code/{code}:
     *   get:
     *     summary: Get a task category by category code
     *     tags:
     *       - Task Categories
     *     parameters:
     *       - in: path
     *         name: code
     *         required: true
     *         schema:
     *           type: string
     *         description: Category code
     *     responses:
     *       200:
     *         description: Task category found
     *         content:
     *           application/json:
     *             schema:
     *               $ref: '#/components/schemas/TaskCategoryDto'
     *       400:
     *         description: Category code is required
     *       404:
     *         description: Task category not found
     *       500:
     *         description: Server error
     */
    async getCategoryByCode(req: Request, res: Response, next: NextFunction): Promise<void> {
        try {
            const categoryCode = req.params.code;
            if (!categoryCode) {
                res.status(400).json({ message: 'Category code is required' });
                return;
            }

            const category = await this.taskCategoryService.getCategoryByCode(categoryCode);
            
            if (!category) {
                res.status(404).json({ message: 'Task category not found' });
                return;
            }
            
            res.status(200).json(category);
        } catch (error) {
            next(error);
        }
    }
    /**
     * @swagger
     * /task-categories:
     *   get:
     *     summary: Get all task categories
     *     tags:
     *       - Task Categories
     *     parameters:
     *       - in: query
     *         name: pageNumber
     *         schema:
     *           type: integer
     *           default: 1
     *         description: Page number
     *       - in: query
     *         name: pageSize
     *         schema:
     *           type: integer
     *           default: 10
     *         description: Number of records per page
     *       - in: query
     *         name: name
     *         schema:
     *           type: string
     *         description: Filter by category name
     *     responses:
     *       200:
     *         description: List of task categories
     *         content:
     *           application/json:
     *             schema:
     *               type: array
     *               items:
     *                 $ref: '#/components/schemas/TaskCategoryDto'
     *       500:
     *         description: Server error
     */
    async getAllCategories(req: Request, res: Response, next: NextFunction): Promise<void> {
        try {
            const name = req.query.name as string | undefined;

            const filter: TaskCategoryFilter = {
                pageNumber: parseInt(req.query.pageNumber as string) || 0,
                pageSize: parseInt(req.query.pageSize as string) || 10,
                name: name,
            };
            
            const categories = await this.taskCategoryService.getAllCategories(filter);
            res.status(200).json(categories);
        } catch (error) {
            next(error);
        }
    }
}

/**
 * @swagger
 * components:
 *   schemas:
 *     TaskCategoryDto:
 *       type: object
 *       required:
 *         - name
 *         - category
 *         - description
 *       properties:
 *         id:
 *           type: string
 *           nullable: true
 *           description: Task category ID
 *         name:
 *           type: string
 *         category:
 *           type: string
 *         description:
 *           type: string
 *
 *     CreateTaskCategoryDto:
 *       type: object
 *       required:
 *         - name
 *         - category
 *         - description
 *       properties:
 *         name:
 *           type: string
 *         category:
 *           type: string
 *         description:
 *           type: string
 */

export const taskCategoryController = new TaskCategoryController();