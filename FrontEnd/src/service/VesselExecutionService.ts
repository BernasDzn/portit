import { injectable, inject } from "inversify";
import { TYPES } from "@/inversify/types";
import type { IHttpService } from "./IService/IHttpService";
import type { Filter, Page } from "@/model/Page";
import type { VesselVisitExecution } from "@/model/VesselVisitExecution";
import type { VesselVisitExecutionDto, OperationDto, VesselVisitExecutionFilter } from "@/model/dto/VesselVisitExecutionDto";
import type { IVesselVisitExecutionService } from "./IService/IVesselExecutionService";

@injectable()
export class VesselVisitExecutionService implements IVesselVisitExecutionService {
    constructor(
        @inject(TYPES.api)
        private http: IHttpService
    ) { }

    async count(): Promise<number> {
        
        const res = await this.http.get<{ count: number }>(
            `/oem/vessel-visit-executions/count`
        );

        return res.data.count;
    }

    async openVesselVisitExecution(relatedVVN: string): Promise<VesselVisitExecution> {
        const res = await this.http.post<VesselVisitExecution>(
            `/oem/vessel-visit-executions/${encodeURIComponent(relatedVVN)}/open`,
            {}
        );
        return res.data;
    }

    async closeVesselVisitExecution(relatedVVN: string): Promise<VesselVisitExecution> {
        const res = await this.http.put<VesselVisitExecution>(
            `/oem/vessel-visit-executions/${encodeURIComponent(relatedVVN)}/close`,
            {}
        );
        return res.data;
    }

    async getVesselVisitExecutionByVVN(relatedVVN: string): Promise<VesselVisitExecution> {
        const res = await this.http.get<VesselVisitExecution>(
            `/oem/vessel-visit-executions/${encodeURIComponent(relatedVVN)}`
        );
        return res.data;
    }

    async getAllVesselVisitExecutions(filter?: Filter<VesselVisitExecutionFilter>): Promise<Page<VesselVisitExecution>> {
        const query: string[] = [];

        if (filter) {
            query.push(filter.pageNumber !== undefined ? `PageNumber=${filter.pageNumber}&` : "");
            query.push(filter.pageSize !==undefined ? `PageSize=${filter.pageSize}` : "");
        }

        const res = await this.http.get<Page<VesselVisitExecution>>(
            `/oem/vessel-visit-executions${query.length ? `?${query.join('')}` : ''}`
        );
        return res.data;
    }

    async startOperation(relatedVVN: string, operation: OperationDto): Promise<VesselVisitExecution> {
        const res = await this.http.put<VesselVisitExecution>(
            `/oem/vessel-visit-executions/${encodeURIComponent(relatedVVN)}/operations/start`,
            operation
        );
        return res.data;
    }

    async completeOperation(relatedVVN: string, operationId: string, endTime: Date): Promise<VesselVisitExecution> {
        const res = await this.http.put<VesselVisitExecution>(
            `/oem/vessel-visit-executions/${encodeURIComponent(relatedVVN)}/operations/${encodeURIComponent(operationId)}/complete`,
            { endTime: endTime.toISOString() }
        );
        return res.data;
    }

    async getOperationsByVVN(relatedVVN: string): Promise<VesselVisitExecution> {
        const res = await this.http.get<VesselVisitExecution>(
            `/oem/vessel-visit-executions/${encodeURIComponent(relatedVVN)}`
        );
        return res.data;
    }
}