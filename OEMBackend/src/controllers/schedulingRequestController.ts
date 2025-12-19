import { Route, Tags, Controller, Get, Post, Query } from "tsoa";
import { SchedulingRequestService } from '../services/schedulingRequestService';

@Route("schedule")
@Tags("Scheduling")
export class SchedulingRequestController extends Controller {

    private schedulingRequestService = new SchedulingRequestService();

    @Get("request")
    public async scheduleRequest(
        @Query() day: string,
        @Query() alg: string,
        @Query() daysAhead: number = 2
    ) {
        if (!day || !alg) {
            this.setStatus(400);
            return { message: 'Missing required query parameters: day and alg' };
        }

        // Hard cap at one this might cause issues later
        daysAhead = 1;
        const userEmail = 'system'; // Note: requires auth context
        const data = await this.schedulingRequestService.scheduleRequest(day, alg, daysAhead, userEmail);
        return data;
    }

    @Get("queueState")
    public async getQueueState() {
        const data = await this.schedulingRequestService.getQueueState();
        return data;
    }

    @Post("acceptRequest")
    public async acceptRequest(@Query() id: string) {
        if (!id) {
            this.setStatus(400);
            return { message: 'Missing required query parameter: id' };
        }

        const userEmail = 'system'; // Note: requires auth context
        const token = undefined; // Note: requires auth context
        const data = await this.schedulingRequestService.acceptRequest(id, userEmail, token!);
        return data;
    }

    @Post("rejectRequest")
    public async rejectRequest(@Query() id: string) {
        if (!id) {
            this.setStatus(400);
            return { message: 'Missing required query parameter: id' };
        }

        const userEmail = 'system'; // Note: requires auth context
        const data = await this.schedulingRequestService.rejectRequest(id, userEmail);
        return data;
    }

}