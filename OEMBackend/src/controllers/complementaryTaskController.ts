import { Inject, Service } from "typedi";
import { BaseController } from "../core/infra/baseController";
import { ComplementaryTaskService } from "../services/complementaryTaskService";
import { NextFunction, Request, Response } from "express";
import { CreateComplementaryTaskDto, UpdateComplementaryTaskDto } from "../dto/complementaryTaskDto";
import { ComplementaryTaskFilter } from "../dto/filters/complementaryTaskFilter";
import { TaskStatus, TaskImpact } from "../domain/complementaryTask";

@Service()
export default class ComplementaryTaskController extends BaseController {

	constructor(
		@Inject("complementaryTaskService") private service: ComplementaryTaskService
	) {
		super();
	}

	/**
	 * @openapi
	 * /complementary-tasks:
	 *   post:
	 *     tags: [Complementary Tasks]
	 *     summary: Create a new complementary task
	 *     security:
	 *       - bearerAuth: []
	 *     requestBody:
	 *       required: true
	 *       content:
	 *         application/json:
	 *           schema:
	 *             type: object
	 *             required:
	 *               - categoryId
	 *               - responsibleTeam
	 *               - startTimestamp
	 *               - vesselVisitEventId
	 *               - impact
	 *             properties:
	 *               categoryId:
	 *                 type: string
	 *               responsibleTeam:
	 *                 type: string
	 *               startTimestamp:
	 *                 type: string
	 *                 format: date-time
	 *               endTimestamp:
	 *                 type: string
	 *                 format: date-time
	 *               status:
	 *                 type: string
	 *                 enum: [ongoing, completed, cancelled]
	 *               vesselVisitEventId:
	 *                 type: string
	 *               impact:
	 *                 type: string
	 *                 enum: [parallel, suspends_operations]
	 *               description:
	 *                 type: string
	 *     responses:
	 *       201:
	 *         description: Task created successfully
	 *       400:
	 *         description: Invalid request
	 */
	public async create(req: Request, res: Response, next: NextFunction): Promise<void> {
		try {
			const dto: CreateComplementaryTaskDto = {
				categoryId: req.body.categoryId,
				responsibleTeam: req.body.responsibleTeam,
				startTimestamp: new Date(req.body.startTimestamp),
				endTimestamp: req.body.endTimestamp ? new Date(req.body.endTimestamp) : undefined,
				status: req.body.status || undefined,
				vesselVisitEventId: req.body.vesselVisitEventId,
				impact: req.body.impact,
				description: req.body.description || undefined
			};

			const result = await this.service.create(dto);
			res.status(201).json(result);
		} catch (e: any) {
			this.fail(res, e.message);
			return next(e);
		}
	}

	/**
	 * @openapi
	 * /complementary-tasks/{id}:
	 *   get:
	 *     tags: [Complementary Tasks]
	 *     summary: Get complementary task by ID
	 *     security:
	 *       - bearerAuth: []
	 *     parameters:
	 *       - in: path
	 *         name: id
	 *         required: true
	 *         schema:
	 *           type: string
	 *     responses:
	 *       200:
	 *         description: Task details
	 *       404:
	 *         description: Task not found
	 */
	public async getById(req: Request, res: Response, next: NextFunction): Promise<void> {
		try {
			const id = req.params.id;
			if (!id) {
				this.clientError(res, "Task ID is required");
				return;
			}
			const result = await this.service.findById(id);

			if (!result) {
				this.notFound(res, "Complementary task not found");
				return;
			}

			this.ok(res, result);
		} catch (e: any) {
			this.fail(res, e.message);
			return next(e);
		}
	}

	/**
	 * @openapi
	 * /complementary-tasks:
	 *   get:
	 *     tags: [Complementary Tasks]
	 *     summary: Get all complementary tasks with filtering
	 *     security:
	 *       - bearerAuth: []
	 *     parameters:
	 *       - in: query
	 *         name: pageNumber
	 *         schema:
	 *           type: integer
	 *           default: 1
	 *       - in: query
	 *         name: pageSize
	 *         schema:
	 *           type: integer
	 *           default: 10
	 *       - in: query
	 *         name: vesselVisitEventId
	 *         schema:
	 *           type: string
	 *       - in: query
	 *         name: status
	 *         schema:
	 *           type: string
	 *           enum: [ongoing, completed, cancelled]
	 *       - in: query
	 *         name: impact
	 *         schema:
	 *           type: string
	 *           enum: [parallel, suspends_operations]
	 *       - in: query
	 *         name: categoryId
	 *         schema:
	 *           type: string
	 *       - in: query
	 *         name: startDate
	 *         schema:
	 *           type: string
	 *           format: date-time
	 *       - in: query
	 *         name: endDate
	 *         schema:
	 *           type: string
	 *           format: date-time
	 *       - in: query
	 *         name: responsibleTeam
	 *         schema:
	 *           type: string
	 *     responses:
	 *       200:
	 *         description: List of complementary tasks
	 */
	public async getAll(req: Request, res: Response, next: NextFunction): Promise<void> {
		try {
			const pageNumber = parseInt(req.query.pageNumber as string) || 1;
			const pageSize = parseInt(req.query.pageSize as string) || 10;

			const filter: ComplementaryTaskFilter = {
				vesselVisitEventId: req.query.vesselVisitEventId as string | undefined,
				status: req.query.status as TaskStatus | undefined,
				impact: req.query.impact as TaskImpact | undefined,
				categoryId: req.query.categoryId as string | undefined,
				startDate: req.query.startDate ? new Date(req.query.startDate as string) : undefined,
				endDate: req.query.endDate ? new Date(req.query.endDate as string) : undefined,
				responsibleTeam: req.query.responsibleTeam as string | undefined
			};

			const result = await this.service.findAll(filter, { pageNumber, pageSize });
			this.ok(res, result);
		} catch (e: any) {
			this.fail(res, e.message);
			return next(e);
		}
	}

	/**
	 * @openapi
	 * /complementary-tasks/{id}:
	 *   put:
	 *     tags: [Complementary Tasks]
	 *     summary: Update complementary task
	 *     security:
	 *       - bearerAuth: []
	 *     parameters:
	 *       - in: path
	 *         name: id
	 *         required: true
	 *         schema:
	 *           type: string
	 *     requestBody:
	 *       required: true
	 *       content:
	 *         application/json:
	 *           schema:
	 *             type: object
	 *     responses:
	 *       200:
	 *         description: Task updated successfully
	 *       404:
	 *         description: Task not found
	 */
	public async update(req: Request, res: Response, next: NextFunction): Promise<void> {
		try {
			const id = req.params.id;
			if (!id) {
				this.clientError(res, "Task ID is required");
				return;
			}
			const dto: UpdateComplementaryTaskDto = {
				categoryId: req.body.categoryId || undefined,
				responsibleTeam: req.body.responsibleTeam || undefined,
				startTimestamp: req.body.startTimestamp ? new Date(req.body.startTimestamp) : undefined,
				endTimestamp: req.body.endTimestamp ? new Date(req.body.endTimestamp) : undefined,
				status: req.body.status || undefined,
				vesselVisitEventId: req.body.vesselVisitEventId || undefined,
				impact: req.body.impact || undefined,
				description: req.body.description || undefined
			};

			const result = await this.service.update(id, dto);

			if (!result) {
				this.notFound(res, "Complementary task not found");
				return;
			}

			this.ok(res, result);
		} catch (e: any) {
			this.fail(res, e.message);
			return next(e);
		}
	}

	/**
	 * @openapi
	 * /complementary-tasks/{id}:
	 *   delete:
	 *     tags: [Complementary Tasks]
	 *     summary: Delete complementary task
	 *     security:
	 *       - bearerAuth: []
	 *     parameters:
	 *       - in: path
	 *         name: id
	 *         required: true
	 *         schema:
	 *           type: string
	 *     responses:
	 *       204:
	 *         description: Task deleted successfully
	 *       404:
	 *         description: Task not found
	 */
	public async delete(req: Request, res: Response, next: NextFunction): Promise<void> {
		try {
			const id = req.params.id;
			if (!id) {
				this.clientError(res, "Task ID is required");
				return;
			}
			const result = await this.service.delete(id);

			if (!result) {
				this.notFound(res, "Complementary task not found");
				return;
			}

			res.sendStatus(204);
		} catch (e: any) {
			this.fail(res, e.message);
			return next(e);
		}
	}

	/**
	 * @openapi
	 * /complementary-tasks/{id}/complete:
	 *   post:
	 *     tags: [Complementary Tasks]
	 *     summary: Mark task as completed
	 *     security:
	 *       - bearerAuth: []
	 *     parameters:
	 *       - in: path
	 *         name: id
	 *         required: true
	 *         schema:
	 *           type: string
	 *     requestBody:
	 *       content:
	 *         application/json:
	 *           schema:
	 *             type: object
	 *             properties:
	 *               endTimestamp:
	 *                 type: string
	 *                 format: date-time
	 *     responses:
	 *       200:
	 *         description: Task completed successfully
	 *       404:
	 *         description: Task not found
	 */
	public async complete(req: Request, res: Response, next: NextFunction): Promise<void> {
		try {
			const id = req.params.id;
			if (!id) {
				this.clientError(res, "Task ID is required");
				return;
			}
			const endTimestamp = req.body.endTimestamp ? new Date(req.body.endTimestamp) : new Date();

			const result = await this.service.completeTask(id, endTimestamp);

			if (!result) {
				this.notFound(res, "Complementary task not found");
				return;
			}

			this.ok(res, result);
		} catch (e: any) {
			this.fail(res, e.message);
			return next(e);
		}
	}

	/**
	 * @openapi
	 * /complementary-tasks/impacting-operations:
	 *   get:
	 *     tags: [Complementary Tasks]
	 *     summary: Get ongoing tasks that are suspending operations
	 *     security:
	 *       - bearerAuth: []
	 *     parameters:
	 *       - in: query
	 *         name: vesselVisitEventId
	 *         schema:
	 *           type: string
	 *     responses:
	 *       200:
	 *         description: List of impacting tasks
	 */
	public async getImpactingOperations(req: Request, res: Response, next: NextFunction): Promise<void> {
		try {
			const vesselVisitEventId = req.query.vesselVisitEventId as string;
			const result = await this.service.findOngoingTasksImpactingOperations(vesselVisitEventId);
			this.ok(res, result);
		} catch (e: any) {
			this.fail(res, e.message);
			return next(e);
		}
	}
}
