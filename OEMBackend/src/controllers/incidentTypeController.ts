import { Inject, Service } from "typedi";
import { BaseController } from "../core/infra/baseController";
import { IncidentTypeService } from "../services/incidentTypeService";

@Service()
export default class IncidentTypeController extends BaseController {

	constructor(
		@Inject("incidentTypeService") private incidentTypeService: IncidentTypeService
	) {
		super();
	}

	/**
	 * @openapi
	 * /incident-types:
	 *   post:
	 *     tags: [Incident Types]
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
	 *               subtypeOfId:
	 *                 type: string
	 *                 description: ID of parent type (optional)
	 *               subtypesIds:
	 *                 type: array
	 *                 items:
	 *                   type: string
	 *                 description: Array of subtype IDs (optional)
	 *             required:
	 *               - name
	 *     responses:
	 *       201:
	 *         description: Incident type created successfully
	 */
	public async createIncidentType(req: any, res: any, next: any): Promise<void> {
		try {
			const { name, subtypeOfId, subtypesIds } = req.body;
			const incidentType = await this.incidentTypeService.createIncidentType(name, subtypeOfId, subtypesIds);
			this.created(res, incidentType);
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
	public async getIncidentTypeById(req: any, res: any, next: any): Promise<void> {
		try {
			const { id } = req.params;
			const incidentType = await this.incidentTypeService.getIncidentTypeById(id);
			if (incidentType) {
				this.ok(res, incidentType);
			} else {
				this.notFound(res, 'Incident type not found');
			}
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
	 *     responses:
	 *       200:
	 *         description: List of incident types retrieved successfully
	 */
	public async getAllIncidentTypes(req: any, res: any, next: any): Promise<void> {
		try {
			const incidentTypes = await this.incidentTypeService.getAllIncidentTypes();
			this.ok(res, incidentTypes);
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
			const { name, subtypesIds } = req.body;
			const incidentType = await this.incidentTypeService.updateIncidentType(id, name, subtypesIds);
			if (incidentType) {
				this.ok(res, incidentType);
			} else {
				this.notFound(res, 'Incident type not found');
			}
		} catch (error) {
			next(error);
		}
	}

	/**
	 * @openapi
	 * /incident-types/{id}/subtypes/{subtypeId}:
	 *   delete:
	 *     tags: [Incident Types]
	 *     summary: Remove a subtype from incident type
	 *     security:
	 *       - bearerAuth: []
	 *     parameters:
	 *       - in: path
	 *         name: id
	 *         required: true
	 *         schema:
	 *           type: string
	 *         description: Parent incident type ID
	 *       - in: path
	 *         name: subtypeId
	 *         required: true
	 *         schema:
	 *           type: string
	 *         description: Subtype incident type ID to remove
	 *     responses:
	 *       200:
	 *         description: Subtype removed successfully
	 */
	public async removeSubtype(req: any, res: any, next: any): Promise<void> {
		try {
			const { id, subtypeId } = req.params;
			const incidentType = await this.incidentTypeService.removeSubtype(id, subtypeId);
			if (incidentType) {
				this.ok(res, incidentType);
			} else {
				this.notFound(res, 'Incident type not found');
			}
		} catch (error) {
			next(error);
		}
	}

	/**
	 * @openapi
	 * /incident-types/{id}:
	 *   delete:
	 *     tags: [Incident Types]
	 *     summary: Delete an incident type by ID
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
	 *       204:
	 *         description: Incident type deleted successfully
	 */
	public async deleteIncidentType(req: any, res: any, next: any): Promise<void> {
		try {
			const { id } = req.params;
			const deleted = await this.incidentTypeService.deleteIncidentType(id);
			if (deleted) {
				this.ok(res, 'Incident type deleted successfully');
			} else {
				this.notFound(res, 'Incident type not found');
			}
		} catch (error) {
			next(error);
		}
	}

}