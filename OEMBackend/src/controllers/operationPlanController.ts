import { Request, Response, NextFunction } from 'express';
import { operationPlanService } from "../services/operationPlanService";

/**
 * @swagger
 * /plans:
 *   get:
 *     tags: [OperationPlans]
 *     responses:
 *       200:
 *         description: List of operation plans
 */
export const getPlans = async (req: Request, res: Response, next: NextFunction) => {
    try {
        
        const items = await operationPlanService.getAll();
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