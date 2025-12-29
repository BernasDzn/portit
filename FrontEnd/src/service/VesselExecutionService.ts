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

    async getAllVesselVisitExecutions(filtering?: Filter<VesselVisitExecutionFilter>): Promise<Page<VesselVisitExecution>> {
        const query: string[] = [];

        if (filtering) {

            if (filtering.filter) {
                if (filtering.filter.startDate) {
                    query.push(`startDate=${encodeURIComponent(filtering.filter.startDate)}`);
                }
                if (filtering.filter.endDate) {
                    query.push(`endDate=${encodeURIComponent(filtering.filter.endDate)}`);
                }
                if (filtering.filter.relatedVVN) {
                    query.push(`relatedVVN=${encodeURIComponent(filtering.filter.relatedVVN)}`);
                }
                if (filtering.filter.status) {
                    query.push(`status=${encodeURIComponent(filtering.filter.status)}`);
                }
            }

            // Ensure pageNumber is at least 0 (backend expects 1-based, so we send 1 as minimum)
            if (filtering.pageNumber !== undefined) {
                const safePageNumber = Math.max(0, filtering.pageNumber);
                query.push(`pageNumber=${safePageNumber}`);
            }
            if (filtering.pageSize !== undefined) {
                const safePageSize = Math.max(1, filtering.pageSize);
                query.push(`pageSize=${safePageSize}`);
            }
        }

        const queryString = query.length ? `?${query.join("&")}` : "";
        console.log("Fetching VVEs with query:", queryString);
        const res = await this.http.get<Page<VesselVisitExecution>>(
            `/oem/vessel-visit-executions${queryString}`
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

    async updateBerthDetails(relatedVVN: string, dock: string, berthTime: string): Promise<VesselVisitExecution> {
        const res = await this.http.put<VesselVisitExecution>(
            `/oem/vessel-visit-executions/${encodeURIComponent(relatedVVN)}/berth`,
            { dock, berthTime }
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