import { Body, Controller, Get, Path, Post, Put, Query, Request, Route, Tags } from "tsoa";
import { IncidentService } from "../services/incidentService";
import { incidentFilter } from "../dto/filters/incidentFilter";
import { CreateIncidentDto, UpdateIncidentDto } from "../dto/incidentDto";
import { Request as ExpressRequest } from 'express';

@Route("incidents")
@Tags("Incidents")
export class IncidentController extends Controller {

	private incidentService = new IncidentService();

	@Get("count")
	public async count(): Promise<{ count : number }> {
		const count = await this.incidentService.count();
		return { count };
	}

	@Get("{bid}")
	public async getByBid(
		@Path() bid: string
	){
		try{
			const incident = await this.incidentService.getByBid(bid);
			return incident;
		}catch(error: any){
			this.setStatus(500);
			return { message: "Internal server error: " + error.message };
		}
	}

	@Get()
	public async getPaged(
		@Query() vveCode?: string,
		@Query() filterStartTime?: string,
		@Query() filterEndTime?: string,
		@Query() severity?: string,
		@Query() isResolved?: boolean,
		@Query() pageNumber: number = 1,
		@Query() pageSize: number = 10
	) {
		try{
			const filter : incidentFilter = {
				vveCode,
				filterStartTime,
				filterEndTime,
				severity,
				isResolved,
				pageNumber,
				pageSize
			};

			const incidentsPage = await this.incidentService.getPaged(filter);
			return incidentsPage;
		}catch(error: any){
			this.setStatus(500);
			return { message: "Internal server error: " + error.message };
		}
	}

	@Post()
	public async createIncident(
		@Body() body: CreateIncidentDto,
		@Request() request: ExpressRequest
	) {
		try{
			const token: string | undefined = request.user?.token;
			if (!token) {
				this.setStatus(401);
				return { message: 'Authorization token required' };
			}

			const createdBy = request.user?.emailAddress || 'unknown';

			const incident = await this.incidentService.create(body, createdBy);
			this.setStatus(201);
			return incident;
		}catch(error: any){
			this.setStatus(500);
			return { message: "Internal server error: " + error.message };
		}
	}

	@Put("{bid}")
	public async updateIncident(
		@Path() bid: string,
		@Body() body: Partial<UpdateIncidentDto>
	) {
		try{
			const incident = await this.incidentService.update(bid, body);
			if (!incident) {
				this.setStatus(404);
				return { message: "Incident not found" };
			}
			return incident;
		}catch(error: any){
			this.setStatus(500);
			return { message: "Internal server error: " + error.message };
		}
	}


}