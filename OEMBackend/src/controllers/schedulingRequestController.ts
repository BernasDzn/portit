import { Route, Tags, Controller, Get, Post, Query, Request } from "tsoa";
import { SchedulingRequestService } from '../services/schedulingRequestService';
import { Request as ExpressRequest } from 'express';
import jwt from 'jsonwebtoken';

@Route("schedule")
@Tags("Scheduling")
export class SchedulingRequestController extends Controller {

    private schedulingRequestService = new SchedulingRequestService();

    @Get("request")
    public async scheduleRequest(
        @Query() day: string,
        @Query() alg: string,
        @Query() daysAhead: number = 2,
        @Request() request: ExpressRequest
    ) {
        if (!day || !alg) {
            this.setStatus(400);
            return { message: 'Missing required query parameters: day and alg' };
        }

        // Hard cap at one this might cause issues later
        daysAhead = 1;
        const authHeader = request.headers.authorization;
        const token = authHeader?.startsWith('Bearer ') ? authHeader.substring(7) : undefined;
        if (!token) {
            this.setStatus(401);
            return { message: 'Authorization token required' };
        }
        const decoded = jwt.decode(token) as any;
        const userEmail = decoded?.email_address || 'unknown';
        const data = await this.schedulingRequestService.scheduleRequest(day, alg, daysAhead, userEmail);
        return data;
    }

    @Get("queueState")
    public async getQueueState() {
        const data = await this.schedulingRequestService.getQueueState();
        return data;
    }

    @Post("acceptRequest")
    public async acceptRequest(@Query() id: string, @Request() request: ExpressRequest) {
        if (!id) {
            this.setStatus(400);
            return { message: 'Missing required query parameter: id' };
        }

        const authHeader = request.headers.authorization;
        const token = authHeader?.startsWith('Bearer ') ? authHeader.substring(7) : undefined;
        
        if (!token) {
            this.setStatus(401);
            return { message: 'Authorization token required' };
        }

        const decoded = jwt.decode(token) as any;
        const userEmail = decoded?.email_address || 'unknown';
        
        const data = await this.schedulingRequestService.acceptRequest(id, userEmail, token);
        return data;
    }

    @Post("rejectRequest")
    public async rejectRequest(@Query() id: string, @Request() request: ExpressRequest) {
        if (!id) {
            this.setStatus(400);
            return { message: 'Missing required query parameter: id' };
        }

        const authHeader = request.headers.authorization;
        const token = authHeader?.startsWith('Bearer ') ? authHeader.substring(7) : undefined;
        
        if (!token) {
            this.setStatus(401);
            return { message: 'Authorization token required' };
        }

        const decoded = jwt.decode(token) as any;
        const userEmail = decoded?.email_address || 'unknown';

        const data = await this.schedulingRequestService.rejectRequest(id, userEmail);
        return data;
    }

}