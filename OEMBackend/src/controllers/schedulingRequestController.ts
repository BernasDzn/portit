import { Request, Response, NextFunction } from 'express';
import { schedulingRequestService } from '../services/schedulingRequestService';

/**
 * @swagger
 * /schedule/request:
 *   get:
 *     tags: [Scheduling]
 *     parameters:
 *       - in: query
 *         name: day
 *         required: true
 *         schema:
 *           type: string
 *         description: The day to schedule
 *       - in: query
 *         name: alg
 *         required: true
 *         schema:
 *           type: string
 *         description: The algorithm to use for scheduling
 *       - in: query
 *         name: daysAhead
 *         required: false
 *         schema:
 *           type: integer
 *           default: 2
 *         description: Number of days ahead to schedule
 *     responses:
 *       200:
 *         description: Scheduled operation plan
 */
export const scheduleRequest = async (req: Request, res: Response, next: NextFunction) => {

    try {
    
        const day = req.query.day as string;
        const alg = req.query.alg as string;
        let daysAhead = parseInt(req.query.daysAhead as string) || 2;

        if (!day || !alg) {
            return res.status(400).json({ message: 'Missing required query parameters: day and alg' });
        }

        // Hard cap at one this might cause issues later
        daysAhead = 1;
        const userEmail = req.user?.emailAddress || 'unknown';
        const data = await schedulingRequestService.scheduleRequest(day, alg, daysAhead, userEmail);
        res.json(data);
    } catch (error) {
        next(error);
    }
}

/**
 * @swagger
 * /schedule/queueState:
 *   get:
 *     tags: [Scheduling]
 *     description: Get the current state of the scheduling queue
 *     responses:
 *       200:
 *         description: Current queue state
 */
export const getQueueState = async (req: Request, res: Response, next: NextFunction) => {
    try {
        const data = await schedulingRequestService.getQueueState();
        res.json(data);
    } catch (error) {
        next(error);
    }
}