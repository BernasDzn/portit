import { Route, Tags, Controller, Get, Post, Patch, Path, Body, Query, Request } from "tsoa";
import { OperationPlanService } from "../services/operationPlanService";
import { PlanFilter } from "../dto/filters/planFilter";
import { Request as ExpressRequest } from 'express';
import jwt from 'jsonwebtoken';

@Route("operation-plans")
@Tags("Operation Plans")
export class OperationPlanController extends Controller {
 
    private operationPlanService = new OperationPlanService();

    @Get()
    public async getPlans(
        @Query() pageNumber: number = 1,
        @Query() pageSize: number = 10,
        @Query() startDate?: string,
        @Query() endDate?: string
    ) {
        const pageFilter: PlanFilter = {
            startDate: startDate,
            endDate: endDate,
            pageNumber: pageNumber,
            pageSize: pageSize
        };

        const plans = await this.operationPlanService.getAll(pageFilter);
        return plans;
    }



    @Get("by-date")
    public async getPlansByDate() {
        const plans = await this.operationPlanService.getByDateGrouped();
        return plans;
    }

    @Get("notifications-without-plan")
    public async getNotificationWithoutPlan(@Request() request: ExpressRequest) {
        const authHeader = request.headers.authorization;
        const token = authHeader?.startsWith('Bearer ') ? authHeader.substring(7) : undefined;
        
        if (!token) {
            this.setStatus(401);
            return { message: 'Authorization token required' };
        }
        
        const items = await this.operationPlanService.getNotificationsWithoutPlan(token);
        if (items === null) {
            this.setStatus(404);
            return { message: 'Not found' };
        }
        return items;
    }

    @Post("regenerate")
    public async regeneratePlansForDay(
        @Body() body: { day: string; algorithm: string; daysAhead?: number }, 
        @Request() request: ExpressRequest
    ) {
        if (!body.day || !body.algorithm) {
            this.setStatus(400);
            return { message: 'Missing required parameters: day and algorithm' };
        }

        const authHeader = request.headers.authorization;
        const token = authHeader?.startsWith('Bearer ') ? authHeader.substring(7) : undefined;
        if (!token) {
            this.setStatus(401);
            return { message: 'Authorization token required' };
        }

        const decoded = jwt.decode(token) as any;
        const userEmail = decoded?.email_address || 'unknown';
        
        const result = await this.operationPlanService.regeneratePlansForDay(
            body.day, 
            body.algorithm, 
            body.daysAhead || 1, 
            userEmail
        );
        
        return result;
    }

    @Patch("{id}")
    public async updateOperationPlan(@Path() id: string, @Body() planData: any) {
        const updatedPlan = await this.operationPlanService.updateOperationPlan(id, planData);
        if (!updatedPlan) {
            this.setStatus(404);
            return { message: 'Operation plan not found for update' };
        }
        return updatedPlan;
    }

    @Get("{id}")
    public async getPlanById(@Path() id: string) {
        const plan = await this.operationPlanService.getById(id);
        if (!plan) {
            this.setStatus(404);
            return { message: 'Operation plan not found' };
        }
        return plan;
    }
}
