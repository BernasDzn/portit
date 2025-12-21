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

        let token: string | undefined = request.user?.token;
        let userEmail: string = request.user?.emailAddress || 'unknown';

        if (!token) {
            this.setStatus(401);
            return { message: 'Authorization token required' };
        }

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

        // Use middleware user when available, otherwise read Authorization header
        let token: string | undefined = (request as any).user?.token;
        let userEmail: string = (request as any).user?.emailAddress || 'unknown';

        if (!token) {
            const authHeader = request.headers.authorization;
            token = authHeader?.startsWith('Bearer ') ? authHeader.substring(7) : authHeader || undefined;
            if (token) {
                const decoded = jwt.decode(token) as any;
                userEmail = decoded?.email_address || decoded?.emailAddress || userEmail;
            }
        }

        if (!token) {
            this.setStatus(401);
            return { message: 'Authorization token required' };
        }

        const data = await this.schedulingRequestService.acceptRequest(id, userEmail, token);
        return data;
    }

    @Post("rejectRequest")
    public async rejectRequest(@Query() id: string, @Request() request: ExpressRequest) {
        if (!id) {
            this.setStatus(400);
            return { message: 'Missing required query parameter: id' };
        }

        // Use middleware user when available, otherwise read Authorization header
        let token: string | undefined = (request as any).user?.token;
        let userEmail: string = (request as any).user?.emailAddress || 'unknown';

        if (!token) {
            const authHeader = request.headers.authorization;
            token = authHeader?.startsWith('Bearer ') ? authHeader.substring(7) : authHeader || undefined;
            if (token) {
                const decoded = jwt.decode(token) as any;
                userEmail = decoded?.email_address || decoded?.emailAddress || userEmail;
            }
        }

        if (!token) {
            this.setStatus(401);
            return { message: 'Authorization token required' };
        }

        const data = await this.schedulingRequestService.rejectRequest(id, userEmail);
        return data;
    }

}