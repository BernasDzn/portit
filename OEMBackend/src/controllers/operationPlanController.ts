import { Inject, Service } from "typedi";
import { BaseController } from "../core/infra/baseController";
import { OperationPlanService } from "../services/operationPlanService";
import { NextFunction, Request, Response } from "express";

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
     *     responses:
     *       200:
     *         description: List of operation plans
     */
    public async getPlans(req: Request, res: Response, next: NextFunction): Promise<void> {
        try{
            const pageNumber = parseInt(req.query.pageNumber as string) || 1;
            const pageSize = parseInt(req.query.pageSize as string) || 10;
            const plans = await this.operationPlanService.getAll({pageNumber,pageSize});
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
            this.fail(res, "Error retrieving notifications without plan");
            return next(e);
        }
    }

}