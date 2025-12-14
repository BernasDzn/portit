import { Request, Response, NextFunction } from 'express';
import { SchedulingRequestService, schedulingRequestService } from '../services/schedulingRequestService';
import { BaseController } from '../core/infra/baseController';
import { Inject, Service } from 'typedi';

@Service()
export default class SchedulingRequestController extends BaseController{

    constructor(
        @Inject() private schedulingRequestService : SchedulingRequestService
    ) {
        super();
    }

    /**
     * @openapi
     * /schedule/request:
     *   get:
     *     tags: [Scheduling]
     *     summary: Request scheduling
     *     security:
     *       - bearerAuth: []
     *     parameters:
     *       - in: query
     *         name: day
     *         required: true
     *         schema:
     *           type: string
     *           format: date
     *         description: Day to schedule
     *     responses:
     *       200:
     *         description: Scheduling request created
     */
    public async scheduleRequest(req: Request, res: Response, next: NextFunction): Promise<void> {
        try {
            const day = req.query.day as string;
            const alg = req.query.alg as string;
            let daysAhead = parseInt(req.query.daysAhead as string) || 2;

            if (!day || !alg) {
                this.clientError(res, 'Missing required query parameters: day and alg');
                return;
            }

            // Hard cap at one this might cause issues later
            daysAhead = 1;
            const userEmail = req.user?.emailAddress || 'unknown';
            const data = await this.schedulingRequestService.scheduleRequest(day, alg, daysAhead, userEmail);
            this.ok(res, data);
        } catch (error) {
            next(error);
        }
    }

    /**
     * @openapi
     * /schedule/queueState:
     *   get:
     *     tags: [Scheduling]
     *     summary: Get queue state
     *     security:
     *       - bearerAuth: []
     *     responses:
     *       200:
     *         description: Current scheduling queue state
     */
    public async getQueueState(req: Request, res: Response, next: NextFunction): Promise<void> {
        try {
            const data = await this.schedulingRequestService.getQueueState();
            this.ok(res, data);
        } catch (error) {
            next(error);
        }
    }

    /**
     * @openapi
     * /schedule/acceptRequest:
     *   post:
     *     tags: [Scheduling]
     *     summary: Accept scheduling request
     *     security:
     *       - bearerAuth: []
     *     requestBody:
     *       required: true
     *       content:
     *         application/json:
     *           schema:
     *             type: object
     *             properties:
     *               requestId:
     *                 type: string
     *     responses:
     *       200:
     *         description: Request accepted
     */
    public async acceptRequest(req: Request, res: Response, next: NextFunction): Promise<void> {
        try {
            const id = req.query.id as string;
            if (!id) {
                this.clientError(res, 'Missing required query parameter: id');
                return;
            }

            const userEmail = req.user?.emailAddress || 'unknown';
            let token = req.user?.token;
            const data = await this.schedulingRequestService.acceptRequest(id, userEmail, token!);
            this.ok(res, data);
        } catch (error) {
            next(error);
        }
    }

    /**
     * @openapi
     * /schedule/rejectRequest:
     *   post:
     *     tags: [Scheduling]
     *     summary: Reject scheduling request
     *     security:
     *       - bearerAuth: []
     *     requestBody:
     *       required: true
     *       content:
     *         application/json:
     *           schema:
     *             type: object
     *             properties:
     *               requestId:
     *                 type: string
     *     responses:
     *       200:
     *         description: Request rejected
     */
    public async rejectRequest(req: Request, res: Response, next: NextFunction): Promise<void> {
        try {
            const id = req.query.id as string;
            if (!id) {
                this.clientError(res, 'Missing required query parameter: id');
                return;
            }

            const userEmail = req.user?.emailAddress || 'unknown';
            const data = await this.schedulingRequestService.rejectRequest(id, userEmail);
            this.ok(res, data);
        } catch (error) {
            next(error);
        }
    }

}