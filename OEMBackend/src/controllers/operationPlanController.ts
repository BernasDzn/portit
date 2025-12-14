import { Inject, Service } from "typedi";
import { BaseController } from "../core/infra/baseController";
import { OperationPlanService } from "../services/operationPlanService";
import { NextFunction, Request, Response } from "express";
import { PlanFilter } from "../dto/filters/planFilter";

@Service()
export default class OperationPlanController extends BaseController {
 
    constructor(
        @Inject("operationPlanService") private operationPlanService : OperationPlanService
    ) {
        super();
    }

    /**
     * @openapi
     * /operation-plans:
     *   get:
     *     tags: [Operation Plans]
     *     summary: Get all operation plans
     *     security:
     *       - bearerAuth: []
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
     *         description: Items per page
     *       - in: query
     *         name: startDate
     *         schema:
     *           type: string
     *         description: Filter plans starting from this date (YYYY-MM-DD)
     *       - in: query
     *         name: endDate
     *         schema:
     *           type: string
     *         description: Filter plans up to this date (YYYY-MM-DD)
     *     responses:
     *       200:
     *         description: List of operation plans
     */
    public async getPlans(req: Request, res: Response, next: NextFunction): Promise<void> {
        try{
            const pageNumber = parseInt(req.query.pageNumber as string) || 1;
            const pageSize = parseInt(req.query.pageSize as string) || 10;

            const pageFilter: PlanFilter = {
                startDate: req.query.startDate as string,
                endDate: req.query.endDate as string,
                pageNumber: pageNumber,
                pageSize: pageSize
            };

            const plans = await this.operationPlanService.getAll(pageFilter);
            this.ok(res, plans);
        }catch(e){
            this.fail(res, "Error retrieving operation plans");
            return next(e);
        }
    }

    /**
     * @openapi
     * /operation-plans/{id}:
     *   get:
     *     tags: [Operation Plans]
     *     summary: Get operation plan by ID
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
     */
    public async getPlanById(req: Request, res: Response, next: NextFunction): Promise<void> {
        try{
            const id = req.params.id as string;
            const plan = await this.operationPlanService.getById(id);
            if (!plan) {
                this.notFound(res, 'Operation plan not found');
                return;
            }
            this.ok(res, plan);
        }catch(e){
            this.fail(res, "Error retrieving operation plan by ID");
            return next(e);
        }
    }

    /**
     * @openapi
     * /operation-plans/by-date:
     *   get:
     *     tags: [Operation Plans]
     *     summary: Get operation plans grouped by date
     *     security:
     *       - bearerAuth: []
     *     responses:
     *       200:
     *         description: Operation plans grouped by date
     */
    public async getPlansByDate(req: Request, res: Response, next: NextFunction): Promise<void> {
        try{
            const plans = await this.operationPlanService.getByDateGrouped();
            this.ok(res, plans);
        }catch(e){
            this.fail(res, "Error retrieving operation plans by date");
            return next(e);
        }
    }

    /**
     * @openapi
     * /operation-plans/notifications-without-plan:
     *   get:
     *     tags: [Operation Plans]
     *     summary: Get notifications without operation plan
     *     security:
     *       - bearerAuth: []
     *     responses:
     *       200:
     *         description: List of notifications without associated operation plan
     */
    public async getNotificationWithoutPlan(req: Request, res: Response, next: NextFunction): Promise<void> {
        try{
            let token = req.user?.token;
            const items = await this.operationPlanService.getNotificationsWithoutPlan(token!);
            if(items === null){
                this.notFound(res);
                return;
            }
            this.ok(res, items);
        }catch(e){
            //this.fail(res, "Error retrieving notifications without plan");
            return next(e);
        }
    }

    /**
     * @openapi
     * /operation-plans/regenerate:
     *   post:
     *     tags: [Operation Plans]
     *     summary: Regenerate operation plans for a specific day
     *     security:
     *       - bearerAuth: []
     *     requestBody:
     *       required: true
     *       content:
     *         application/json:
     *           schema:
     *             type: object
     *             properties:
     *               day:
     *                 type: string
     *                 format: date
     *                 description: Day to regenerate plans for (YYYY-MM-DD)
     *               algorithm:
     *                 type: string
     *                 description: Scheduling algorithm to use
     *               daysAhead:
     *                 type: integer
     *                 description: Number of days ahead to consider
     *             required:
     *               - day
     *               - algorithm
     *     responses:
     *       200:
     *         description: Plans regenerated successfully
     *       400:
     *         description: Missing required parameters
     */
    public async regeneratePlansForDay(req: Request, res: Response, next: NextFunction): Promise<void> {
        try {
            const { day, algorithm, daysAhead } = req.body;
            
            if (!day || !algorithm) {
                this.clientError(res, 'Missing required parameters: day and algorithm');
                return;
            }

            const userEmail = req.user?.emailAddress || 'unknown';
            
            const result = await this.operationPlanService.regeneratePlansForDay(
                day, 
                algorithm, 
                daysAhead || 1, 
                userEmail
            );
            
            this.ok(res, result);
        } catch (e: any) {
            this.fail(res, `Error regenerating plans: ${e.message}`);
            return next(e);
        }
    }

}
