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

    /**
     * @swagger
     * /vessel-visit-executions/count:
     *   get:
     *     summary: Get the total count of Vessel Visit Executions
     *     tags:
     *       - Vessel Visit Executions
     *     responses:
     *       200:
     *         description: Total count of Vessel Visit Executions retrieved successfully
     *         content:
     *           application/json:
     *             schema:
     *               type: object
     *               properties:
     *                 count:
     *                   type: integer
     *                   description: Total number of Vessel Visit Executions
     *       500:
     *         description: Server error
     */
    async countVesselVisitExecutions(req: Request, res: Response, next: NextFunction): Promise<void> {
        try {
            const count = await this.vesselVisitExecutionService.count();
            res.status(200).json({ count });
        } catch (error) {
            next(error);
        }
    }
}

export const vesselVisitExecutionController = new VesselVisitExecutionController();