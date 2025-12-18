import { Inject, Service } from "typedi";
import { BaseController } from "../core/infra/baseController";
import { IncidentTypeService } from "../services/incidentTypeService";
import { Pageable } from "../utils/page";

@Service()
export default class IncidentTypeController extends BaseController {

	constructor(
		@Inject("incidentTypeService") private incidentTypeService: IncidentTypeService
	) {
		super();
	}

	/**
	 * @openapi
	 * /incident-types/count:
	 *   get:
	 *    tags: [Incident Types]
	 *    summary: Get the total count of incident types
	 *    security:
	 *      - bearerAuth: []
	 *    responses:
	 *      200:
	 *        description: Total count of incident types retrieved successfully
	 */
	public async count(req: any, res: any, next: any): Promise<void> {
		try {
			const count = await this.incidentTypeService.count();
			this.ok(res, { count });
		} catch (error) {
			next(error);
		}
	}

	/**
	 * @openapi
	 * /incident-types:
	 *   get:
	 *     tags: [Incident Types]
	 *     summary: Get all incident types
	 *     security:
	 *       - bearerAuth: []
	 *     parameters:
	 *       - in: query
	 *         name: pageNumber
	 *         schema:
	 *           type: integer
	 *           default: 1
	 *       - in: query
	 *         name: pageSize
	 *         schema:
	 *           type: integer
	 *           default: 10
	 *     responses:
	 *       200:
	 *         description: List of incident types retrieved successfully
	 */
	public async getPaged(req: any, res: any, next: any): Promise<void> {
		try {
			const pageNumber = parseInt(req.query.pageNumber) || 1;
			const pageSize = parseInt(req.query.pageSize) || 10;

			const filter: Pageable = {
				pageNumber,
				pageSize
			};

			const incidentTypesPage = await this.incidentTypeService.getPaged(filter);
			this.ok(res, incidentTypesPage);
		} catch (error) {
			next(error);
		}
	}

	/**
	 * @openapi
	 * /incident-types/{id}:
	 *   get:
	 *     tags: [Incident Types]
	 *     summary: Get an incident type by ID
	 *     security:
	 *       - bearerAuth: []
	 *     parameters:
	 *       - in: path
	 *         name: id
	 *         required: true
	 *         schema:
	 *           type: string
	 *         description: Incident type ID
	 *     responses:
	 *       200:
	 *         description: Incident type retrieved successfully
	 */
	public async getById(req: any, res: any, next: any): Promise<void> {
		try {
			const { id } = req.params;
			const incidentType = await this.incidentTypeService.getById(id);
			this.ok(res, incidentType);
		} catch (error) {
			next(error);
		}
	}

	/**
	 * @openapi
	 * /incident-types:
	 *   post:
	 *     tags:
	 *       - Incident Types
	 *     summary: Create a new incident type
	 *     security:
	 *       - bearerAuth: []
	 *     requestBody:
	 *       required: true
	 *       content:
	 *         application/json:
	 *           schema:
	 *             type: object
	 *             properties:
	 *               name:
	 *                 type: string
	 *               description:
	 *                 type: string
	 *               severity:
	 *                 type: string
	 *               subtypeOfId:
	 *                 type: string
	 *                 description: ID of parent type (optional)
	 *             required:
	 *               - name
	 *     responses:
	 *       201:
	 *         description: Incident type created successfully
	 */
	public async createIncidentType(req: any, res: any, next: any): Promise<void> {
		try {
			const incidentType = await this.incidentTypeService.create(req.body);
			this.created(res, incidentType);
		} catch (error) {
			next(error);
		}
	}


	/**
	 * @openapi
	 * /incident-types/{id}:
	 *   patch:
	 *     tags: [Incident Types]
	 *     summary: Update incident type name and/or add subtypes
	 *     security:
	 *       - bearerAuth: []
	 *     parameters:
	 *       - in: path
	 *         name: id
	 *         required: true
	 *         schema:
	 *           type: string
	 *         description: Incident type ID
	 *     requestBody:
	 *       required: true
	 *       content:
	 *         application/json:
	 *           schema:
	 *             type: object
	 *             properties:
	 *               name:
	 *                 type: string
	 *               subtypesIds:
	 *                 type: array
	 *                 items:
	 *                   type: string
	 *     responses:
	 *       200:
	 *         description: Incident type updated successfully
	 */
	public async updateIncidentType(req: any, res: any, next: any): Promise<void> {
		try {
			const { id } = req.params;
			const incidentType = await this.incidentTypeService.update(id, req.body);
			if (incidentType) {
				this.ok(res, incidentType);
			} else {
				this.notFound(res, 'Incident type not found');
			}
		} catch (error) {
			next(error);
		}
	}

}