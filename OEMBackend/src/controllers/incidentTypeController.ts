import { Route, Tags, Controller, Get, Post, Put, Path, Body, Query } from "tsoa";
import { Pageable } from "../utils/page";
import { IncidentTypeService } from "../services/incidentTypeService";
import { PartialIncidentTypeDto } from "../dto/incidentTypeDto";
import { NotFoundError } from "../core/infra/extraErrors";

@Route("incident-types")
@Tags("Incident Types")
export class IncidentTypeController extends Controller {

	private incidentTypeService = new IncidentTypeService();

	@Get("count")
	public async count(): Promise<{ count : number }> {
	const count = await this.incidentTypeService.count();
	return { count }; 
	// we could just return count directly but it's not a good practice... (enforce json res)
	}

	@Get()
	public async getPaged(
		@Query() pageNumber: number = 1,
		@Query() pageSize: number = 10
	) {
		const pagination: Pageable = { pageNumber, pageSize };
		const incidentTypesPage = await this.incidentTypeService.getPaged(pagination);
		return incidentTypesPage;
	}

	@Get("{id}")
	public async getById(@Path() id: string) {
		try{
			const incidentType = await this.incidentTypeService.getById(id);
			return incidentType;
		} catch(error: any){ // example of error handling, this is tedious so I dont car anymore
			if(error instanceof NotFoundError){
				this.setStatus(404);
				return { message: error.message };
			}
			this.setStatus(500);
			return { message: "Internal server error" };
		}
	}

	@Post()
	public async createIncidentType(@Body() body: PartialIncidentTypeDto) {
		const incidentType = await this.incidentTypeService.create(body);
		this.setStatus(201);
		return incidentType;
	}

	@Put("{id}")
	public async updateIncidentType(@Path() id: string, @Body() body: PartialIncidentTypeDto) {
		const incidentType = await this.incidentTypeService.update(id, body);
		if (!incidentType) {
			this.setStatus(404);
			return { message: "Incident type not found" };
		}
		return incidentType;
	}
}
