import { Request, Response, NextFunction } from 'express';
import { operationPlanService } from "../services/operationPlanService";

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
 *                     $ref: '#/schemas/OperationPlanDto'
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
export const getPlans = async (req: Request, res: Response, next: NextFunction) => {
    try {
        const pageNumber = parseInt(req.query.pageNumber as string) || 1;
        const pageSize = parseInt(req.query.pageSize as string) || 10;
        
        const plans = await operationPlanService.getAll({
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
 *     responses:
 *       200:
 *         description: Operation plan details
 *       404:
 *         description: Operation plan not found
 *       401:
 *         description: Unauthorized
 */
export const getPlanById = async (req: Request, res: Response, next: NextFunction) =>  {
    try {
        const id = req.params.id as string;
        const plan = await operationPlanService.getById(id);
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
 *                       $ref: '#/schemas/OperationPlanDto'
 *       401:
 *         description: Unauthorized
 */
export const getPlansByDate = async (req: Request, res: Response, next: NextFunction) => {
    try {
        const plansByDate = await operationPlanService.getByDateGrouped();
        res.json(plansByDate);
    } catch (error) {
        next(error);
    }
}

/**
 * @swagger
 * /plans/notifications-without-plan:
 *   get:
 *     tags: [Operation Plans]
 *     security:
 *       - bearerAuth: []
 *     responses:
 *       200:
 *         description: List of VVN IDs without associated operation plans
 *         content:
 *           application/json:
 *             schema:
 *               type: array
 *               items:
 *                 type: string
 *       204:
 *         description: No content
 *       401:
 *         description: Unauthorized
 */
export const getNotificationWithoutPlan = async (req: Request, res: Response, next: NextFunction) => {
    try {
        let token = req.user?.token;
        const items = await operationPlanService.getNotificationsWithoutPlan(token!);
        if(items === null){
            res.status(204).send();
            return;
        }
        res.json(items);
    } catch (error) {
        next(error);
    }
}