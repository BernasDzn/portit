import { Request, Response, NextFunction } from 'express';
import { operationPlanService } from "../services/operationPlanService";

/**
 * @swagger
 * /plans:
 *   get:
 *     tags: [OperationPlans]
 *     description: Retrieve all operation plans
 *     parameters:
 *       - in: query
 *         name: pageNumber
 *         required: false
 *         schema:
 *           type: integer
 *         description: Page number for pagination
 *       - in: query
 *         name: pageSize
 *         required: false
 *         schema:
 *           type: integer
 *           default: 10
 *         description: Page size for pagination
 *     responses:
 *       200:
 *         description: List of operation plans
 */
export const getPlans = async (req: Request, res: Response, next: NextFunction) => {
    try {
        
        const pageNumber = req.query.pageNumber ? parseInt(req.query.pageNumber as string, 10) : 1;
        const pageSize = req.query.pageSize ? parseInt(req.query.pageSize as string, 10) : 10;

        const items = await operationPlanService.getAll({
            pageNumber,
            pageSize
        });
        res.json(items);

    } catch (error) {
        next(error);
    }
};

/**
 * @swagger
 * /plans/{id}:
 *   get:
 *     tags: [OperationPlans]
 *     parameters:
 *       - in: path
 *         name: id
 *         required: true
 *         schema:
 *           type: string
 *         description: The operation plan ID
 *     responses:
 *       200:
 *         description: Operation plan by ID
 *       404:
 *         description: Operation plan not found
 */
export const getPlanById = async (req: Request, res: Response, next: NextFunction) => {
    try {
        const id = req.params.id;
        if (!id) {
            return res.status(400).json({ message: 'ID parameter is required' });
        }

        const item = await operationPlanService.getById(id!);
        
        if (!item) {
            return res.status(404).json({ message: 'Operation plan not found' });
        }

        res.json(item);

    } catch (error) {
        next(error);
    }
}

/**
 * @swagger
 * /plans/notifications-without-plan:
 *   get:
 *     tags: [OperationPlans]
 *     responses:
 *       200:
 *         description: List of VVN IDs without associated operation plans
 *       404:
 *         description: No notifications without associated operation plans found
 */
export const getNotificationWithoutPlan = async (req: Request, res: Response, next: NextFunction) => {
    try {
        let token = req.user?.token;
        const items = await operationPlanService.getNotificationsWithoutPlan(token!);
        if(items === null){
            return res.status(404).json({ message: 'No notifications without associated operation plans found' });
        }
        res.json(items);
    } catch (error) {
        next(error);
    }
}

/**
 * @swagger
 * /plans/range:
 *   get:
 *     tags: [OperationPlans]
 *     parameters:
 *       - in: query
 *         name: startDate
 *         required: true
 *         schema:
 *           type: string
 *         description: Start date for the range filter
 *       - in: query
 *         name: endDate
 *         required: true
 *         schema:
 *           type: string
 *         description: End date for the range filter
 *       - in: query
 *         name: pageNumber
 *         required: true
 *         schema:
 *           type: integer
 *         description: Page number for pagination
 *       - in: query
 *         name: pageSize
 *         required: true
 *         schema:
 *           type: integer
 *           default: 10
 *         description: Page size for pagination
 *     responses:
 *       200:
 *         description: Operation plans within the specified date range
 */
export const getPlansByDateRange = async (req: Request, res: Response, next: NextFunction) => {
    try {
        const { startDate, endDate } = req.query;

        const pageNumber = req.query.pageNumber ? parseInt(req.query.pageNumber as string, 10) : 1;
        const pageSize = req.query.pageSize ? parseInt(req.query.pageSize as string, 10) : 10;

        if (!startDate || !endDate) {
            return res.status(400).json({ message: 'startDate and endDate query parameters are required' });
        }

        const items = await operationPlanService.findByDateRange(startDate as string, endDate as string, {
            pageNumber,
            pageSize
        });
        res.json(items);

    } catch (error) {
        next(error);
    }
}

/**
 * @swagger
 * /plans/group:
 *   get:
 *     tags: [OperationPlans]
 *     responses:
 *       200:
 *         description: Operation plans grouped by date
 */
export const groupPlansByDate = async (req: Request, res: Response, next: NextFunction) => {
    try {
        
        const items = await operationPlanService.groupByDate();
        res.json(items);

    } catch (error) {
        next(error);
    }
}