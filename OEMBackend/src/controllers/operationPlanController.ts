import { Request, Response, NextFunction } from "express";
import { OperationPlanService } from "../services/operationPlanService";

export class OperationPlanController {

	operationPlanService: OperationPlanService;

	constructor() {
		this.operationPlanService = new OperationPlanService();
	}

	/**
	 * @swagger
	 * /operation-plans:
	 *   get:
	 *     tags: [Operation Plans]
	 *     security:
	 *       - bearerAuth: []
	 *     parameters:
	 *       - in: query
	 *         name: pageNumber
	 *         schema:
	 *           type: integer
	 *           default: 1
	 *         description: Page number for pagination
	 *       - in: query
	 *         name: pageSize
	 *         schema:
	 *           type: integer
	 *           default: 10
	 *         description: Number of items per page
	 *     responses:
	 *       200:
	 *         description: Paginated list of operation plans
	 *         content:
	 *           application/json:
	 *             schema:
	 *               type: object
	 *               properties:
	 *                 content:
	 *                   type: array
	 *                   items:
	 *                     $ref: '#/components/schemas/OperationPlanDto'
	 *                 pageNumber:
	 *                   type: integer
	 *                 pageSize:
	 *                   type: integer
	 *                 totalElements:
	 *                   type: integer
	 *                 totalPages:
	 *                   type: integer
	 *       401:
	 *         description: Unauthorized
	 *       403:
	 *         description: Forbidden - insufficient permissions
	 */
	async getPlans(req: Request, res: Response, next: NextFunction): Promise<void> {
		try {
			const pageNumber = parseInt(req.query.pageNumber as string) || 1;
			const pageSize = parseInt(req.query.pageSize as string) || 10;
			
			const plans = await this.operationPlanService.getAll({
				pageNumber,
				pageSize
			});
			res.json(plans);
		} catch (error) {
			next(error);
		}
	}

	/**
	 * @swagger
	 * /operation-plans/{id}:
	 *   get:
	 *     tags: [Operation Plans]
	 *     security:
	 *       - bearerAuth: []
	 *     parameters:
	 *       - in: path
	 *         name: id
	 *         required: true
	 *         schema:
	 *           type: string
	 *         description: Operation plan ID
	 *     responses:
	 *       200:
	 *         description: Operation plan details
	 *         content:
	 *           application/json:
	 *             schema:
	 *               $ref: '#/components/schemas/OperationPlanDto'
	 *       404:
	 *         description: Operation plan not found
	 *       401:
	 *         description: Unauthorized
	 */
	async getPlanById(req: Request, res: Response, next: NextFunction): Promise<void> {
		try {
			const id = req.params.id as string;
			const plan = await this.operationPlanService.getById(id);
			if (!plan) {
				res.status(404).json({ message: 'Operation plan not found' });
				return;
			}
			res.json(plan);
		} catch (error) {
			next(error);
		}
	}

	/**
	 * @swagger
	 * /operation-plans/by-date:
	 *   get:
	 *     tags: [Operation Plans]
	 *     security:
	 *       - bearerAuth: []
	 *     responses:
	 *       200:
	 *         description: Operation plans grouped by date
	 *         content:
	 *           application/json:
	 *             schema:
	 *               type: array
	 *               items:
	 *                 type: object
	 *                 properties:
	 *                   date:
	 *                     type: string
	 *                     format: date
	 *                   plans:
	 *                     type: array
	 *                     items:
	 *                       $ref: '#/components/schemas/OperationPlanDto'
	 *       401:
	 *         description: Unauthorized
	 */
	async getPlansByDate(req: Request, res: Response, next: NextFunction): Promise<void> {
		try {
			const plansByDate = await this.operationPlanService.getByDateGrouped();
			res.json(plansByDate);
		} catch (error) {
			next(error);
		}
	}

}