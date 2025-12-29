import { Route, Tags, Controller, Get, Post, Put, Path, Body, Query, Request } from "tsoa";
import { VesselVisitExecutionService } from "../services/vesselVisitExecutionService";
import { Request as ExpressRequest } from 'express';
import jwt from 'jsonwebtoken';
import { VesselVisitExecutionFilter } from "../dto/filters/vesselVisitExecutionFilter";

@Route("vessel-visit-executions")
@Tags("Vessel Visit Executions")
export class VesselVisitExecutionController extends Controller {
    
    private vesselVisitExecutionService = new VesselVisitExecutionService();

    @Post("{relatedVVN}/open")
    public async openVesselVisitExecution(@Path() relatedVVN: string, @Request() request: ExpressRequest) {
        
        let token: string | undefined = request.user?.token;
        let userEmail: string = request.user?.emailAddress || 'unknown';
        
        if (!token) {
            this.setStatus(401);
            return { message: 'Authorization token required' };
        }

        const execution = await this.vesselVisitExecutionService.createVesselVisitExecution(relatedVVN, userEmail);
        this.setStatus(201);
        return execution;
    }

    @Put("{relatedVVN}/close")
    public async closeVesselVisitExecution(@Path() relatedVVN: string) {
        const execution = await this.vesselVisitExecutionService.closeVesselVisitExecution(relatedVVN);
        return execution;
    }

    @Put("{relatedVVN}/berth")
    public async updateBerthDetails(@Path() relatedVVN: string, @Body() body: { dock: string; berthTime: string }) {
        const execution = await this.vesselVisitExecutionService.updateBerthDetails(
            relatedVVN,
            body.dock,
            new Date(body.berthTime)
        );
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
        @Query() pageNumber: number = 1,
        @Query() pageSize: number = 10,
        @Query() startDate?: string,
        @Query() endDate?: string,
        @Query() relatedVVN?: string,
        @Query() status?: string,
        @Request() request?: ExpressRequest
    ) {
        // Log raw request.query so we can see unencoded values
        try {
            console.log('Raw request.query:', request?.query);
        } catch (e) {
            // ignore
        }

        // Some clients (or manual browser URLs) may not encode dates; attempt to
        // read values from the raw request query as a fallback.
        const rawQuery = request?.query || {};
        const resolvedStart = startDate ?? (rawQuery.startDate as string | undefined);
        const resolvedEnd = endDate ?? (rawQuery.endDate as string | undefined);
        const resolvedVVN = relatedVVN ?? (rawQuery.relatedVVN as string | undefined);
        const resolvedStatus = status ?? (rawQuery.status as string | undefined);

        const filter: VesselVisitExecutionFilter = {
            pageNumber: pageNumber,
            pageSize: pageSize,
            startDate: resolvedStart,
            endDate: resolvedEnd,
            relatedVVN: resolvedVVN,
            status: resolvedStatus
        };

        // Log the incoming filter for debugging
        try {
            console.log("Filtering VVEs with filter:", JSON.stringify(filter));
        } catch (e) {
            console.log("Filtering VVEs with filter: (unserializable)");
        }

        const executionsPage = await this.vesselVisitExecutionService.getAllVesselVisitExecutions(filter);
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

    @Get("code/{code}")
    public async getVeselVisitExecution(
        @Path() code: string
    ){
        const execution = await this.vesselVisitExecutionService.getVesselVisitExecutionByCode(code);
        if (!execution) {
            this.setStatus(404);
            return { message: `Vessel Visit Execution with code ${code} not found.` };
        }
        return execution;
    }

}

export const vesselVisitExecutionController = new VesselVisitExecutionController();