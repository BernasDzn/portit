import { Route, Tags, Controller, Get, Post, Put, Path, Body, Query, Request } from "tsoa";
import { VesselVisitExecutionService } from "../services/vesselVisitExecutionService";
import { Request as ExpressRequest } from 'express';
import jwt from 'jsonwebtoken';

@Route("vessel-visit-executions")
@Tags("Vessel Visit Executions")
export class VesselVisitExecutionController extends Controller {
    
    private vesselVisitExecutionService = new VesselVisitExecutionService();

    @Post("{relatedVVN}/open")
    public async openVesselVisitExecution(@Path() relatedVVN: string, @Request() request: ExpressRequest) {
        // Check for token in Authorization header first, then in cookies
        const authHeader = request.headers.authorization;
        let token = authHeader?.startsWith('Bearer ') ? authHeader.substring(7) : undefined;
        
        if (!token && request.cookies?.AuthToken) {
            token = request.cookies.AuthToken;
        }
        
        if (!token) {
            this.setStatus(401);
            return { message: 'Authorization token required' };
        }
        const decoded = jwt.decode(token) as any;
        const creatorUser = decoded?.email_address || 'unknown';

        const execution = await this.vesselVisitExecutionService.createVesselVisitExecution(relatedVVN, creatorUser);
        this.setStatus(201);
        return execution;
    }

    @Put("{relatedVVN}/close")
    public async closeVesselVisitExecution(@Path() relatedVVN: string) {
        const execution = await this.vesselVisitExecutionService.closeVesselVisitExecution(relatedVVN);
        return execution;
    }

    @Put("{relatedVVN}/operations/start")
    public async startOperation(@Path() relatedVVN: string, @Body() operation: any) {
        const execution = await this.vesselVisitExecutionService.startOperation(relatedVVN, operation);
        return execution;
    }

    @Put("{relatedVVN}/operations/{operationId}/complete")
    public async completeOperation(
        @Path() relatedVVN: string,
        @Path() operationId: string,
        @Body() body: { endTime: string }
    ) {
        const execution = await this.vesselVisitExecutionService.completeOperation(relatedVVN, operationId, new Date(body.endTime));
        return execution;
    }

    @Get("count")
    public async countVesselVisitExecutions() {
        const count = await this.vesselVisitExecutionService.count();
        return { count };
    }

    @Get()
    public async getAllVesselVisitExecutions(
        @Query() page: number = 1,
        @Query() limit: number = 10
    ) {
        const executionsPage = await this.vesselVisitExecutionService.getAllVesselVisitExecutions(page, limit);
        return executionsPage;
    }

    @Get("{relatedVVN}")
    public async getVesselVisitExecution(@Path() relatedVVN: string) {
        const execution = await this.vesselVisitExecutionService.getVesselVisitExecutionByVVN(relatedVVN);
        if (!execution) {
            this.setStatus(404);
            return { message: `Vessel Visit Execution with VVN ${relatedVVN} not found.` };
        }

        return execution;
    }
}

export const vesselVisitExecutionController = new VesselVisitExecutionController();