import { Inject, Service } from "typedi";
import { VesselVisitExecutionService } from "../services/vesselVisitExecutionService";
import { NextFunction, Request, Response } from "express";

@Service()
export class VesselVisitExecutionController {
    
    vesselVisitExecutionService: VesselVisitExecutionService;

    constructor(
        @Inject("vesselVisitExecutionService") private vesselVisitExecutionServiceInjected?: VesselVisitExecutionService
    ) {
        this.vesselVisitExecutionService = this.vesselVisitExecutionServiceInjected!;
    }

    /**
     * @swagger
     * /vessel-visit-executions/{relatedVVN}/open:
     *   post:
     *     summary: Open a new Vessel Visit Execution for the given Vessel Visit Number (VVN)
     *     tags:
     *       - Vessel Visit Executions
     *     parameters:
     *       - in: path
     *         name: relatedVVN
     *         required: true
     *         schema:
     *           type: string
     *         description: The related Vessel Visit Number
     *     responses:
     *       201:
     *         description: Vessel Visit Execution created successfully
     *         content:
     *           application/json:
     *             schema:
     *               type: object
     *               properties:
     *                 id:
     *                   type: string
     *                   description: The ID of the created Vessel Visit Execution
     *       400:
     *         description: Invalid Vessel Visit Number
     *       500:
     *         description: Server error
     */
    async openVesselVisitExecution(req: Request, res: Response, next: NextFunction): Promise<void> {

        try {
            const { relatedVVN } = req.params;
            if (!relatedVVN) {
                res.status(400).json({ message: 'relatedVVN parameter is required.' });
                return;
            }

            const execution = await this.vesselVisitExecutionService.createVesselVisitExecution(relatedVVN);
            res.status(201).json(execution);
        } catch (error) {
            next(error);
        }
    }
}