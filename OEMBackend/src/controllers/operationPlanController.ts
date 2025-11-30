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