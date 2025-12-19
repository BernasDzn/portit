import { Route, Tags, Controller, Get, Post, Put, Path, Body, Query } from "tsoa";
import { VesselVisitExecutionService } from "../services/vesselVisitExecutionService";

@Route("vessel-visit-executions")
@Tags("Vessel Visit Executions")
export class VesselVisitExecutionController extends Controller {
    
    private vesselVisitExecutionService = new VesselVisitExecutionService();

    @Post("{relatedVVN}/open")
    public async openVesselVisitExecution(@Path() relatedVVN: string) {
        // Note: creatorUser requires auth context
        const creatorUser = 'system';

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

    @Get("{relatedVVN}")
    public async getVesselVisitExecution(@Path() relatedVVN: string) {
        const execution = await this.vesselVisitExecutionService.getVesselVisitExecutionByVVN(relatedVVN);
        if (!execution) {
            this.setStatus(404);
            return { message: `Vessel Visit Execution with VVN ${relatedVVN} not found.` };
        }

        return execution;
    }

    @Get()
    public async getAllVesselVisitExecutions(
        @Query() page: number = 1,
        @Query() limit: number = 10
    ) {
        const executionsPage = await this.vesselVisitExecutionService.getAllVesselVisitExecutions(page, limit);
        return executionsPage;
    }
}

export const vesselVisitExecutionController = new VesselVisitExecutionController();