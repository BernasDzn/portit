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

            const creatorUser = req.user?.emailAddress;
            if (!creatorUser) {
                res.status(400).json({ message: 'Creator user information is missing.' });
                return;
            }

            const execution = await this.vesselVisitExecutionService.createVesselVisitExecution(relatedVVN, creatorUser);
            res.status(201).json(execution);
        } catch (error) {
            next(error);
        }
    }

    /**
     * @swagger
     * /vessel-visit-executions/{relatedVVN}/close:
     *   put:
     *     summary: Close an existing Vessel Visit Execution for the given Vessel Visit Number (VVN)
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
     *       200:
     *         description: Vessel Visit Execution closed successfully
     *         content:
     *           application/json:
     *             schema:
     *               type: object
     *               properties:
     *                 id:
     *                   type: string
     *                   description: The ID of the closed Vessel Visit Execution
     *       400:
     *         description: Invalid Vessel Visit Number
     *       500:
     *         description: Server error
     */
    async closeVesselVisitExecution(req: Request, res: Response, next: NextFunction): Promise<void> {
        try {
            const { relatedVVN } = req.params;
            if (!relatedVVN) {
                res.status(400).json({ message: 'relatedVVN parameter is required.' });
                return;
            }

            const execution = await this.vesselVisitExecutionService.closeVesselVisitExecution(relatedVVN);
            res.status(200).json(execution);
        } catch (error) {
            next(error);
        }
    }

    /**
     * @swagger
     * /vessel-visit-executions/{relatedVVN}/operations/start:
     *   put:
     *     summary: Start a new operation for a Vessel Visit Execution
     *     tags:
     *       - Vessel Visit Executions
     *     parameters:
     *       - in: path
     *         name: relatedVVN
     *         required: true
     *         schema:
     *           type: string
     *         description: Vessel Visit Execution ID
     *     requestBody:
     *       required: true
     *       content:
     *         application/json:
     *           schema:
     *             $ref: '#/components/schemas/OperationDto'
     *     responses:
     *       200:
     *         description: Operation started successfully
     *         content:
     *           application/json:
     *             schema:
     *               $ref: '#/components/schemas/VesselVisitExecutionDto'
     *       400:
     *         description: Invalid input data
     *       404:
     *         description: Vessel Visit Execution not found
     *       500:
     *         description: Server error
     */
    async startOperation(req: Request, res: Response, next: NextFunction): Promise<void> {
        try {
            const { relatedVVN } = req.params;
            const operation = req.body;

            if (!relatedVVN) {
                res.status(400).json({ message: 'relatedVVN parameter is required.' });
                return;
            }

            if (!operation) {
                res.status(400).json({ message: 'Operation data is required in the request body.' });
                return;
            }

            const execution = await this.vesselVisitExecutionService.startOperation(relatedVVN, operation);
            res.status(200).json(execution);
        } catch (error) {
            next(error);
        }
    }
}

/**
 * @swagger
 * components:
 *   schemas:
 *     OperationDto:
 *       type: object
 *       required:
 *         - id
 *         - type
 *         - startTime
 *         - endTime
 *         - resources
 *       properties:
 *         id:
 *           type: string
 *           description: Operation identifier
 *         type:
 *           type: string
 *           description: Operation type code/identifier
 *         startTime:
 *           type: string
 *           format: date-time
 *           description: Operation start time (ISO-8601)
 *         endTime:
 *           type: string
 *           format: date-time
 *           description: Operation end time (ISO-8601)
 *         resources:
 *           type: array
 *           items:
 *             $ref: '#/components/schemas/ResourceStartDto'
 *         payload:
 *           type: object
 *           nullable: true
 *           description: Additional operation-specific data
 *     ResourceStartDto:
 *       type: object
 *       required:
 *         - name
 *         - startTime
 *         - endTime
 *       properties:
 *         name:
 *           type: string
 *           description: Resource name/identifier
 *         startTime:
 *           type: string
 *           format: date-time
 *           description: Resource allocation start time (ISO-8601)
 *         endTime:
 *           type: string
 *           format: date-time
 *           description: Resource allocation end time (ISO-8601)
 */
export const vesselVisitExecutionController = new VesselVisitExecutionController();