import type { Filter, Page } from "@/model/Page";
import type { VesselVisitExecution } from "@/model/VesselVisitExecution";
import type { OperationDto, VesselVisitExecutionFilter } from "@/model/dto/VesselVisitExecutionDto";

export interface IVesselVisitExecutionService {
    openVesselVisitExecution(relatedVVN: string): Promise<VesselVisitExecution>;
    closeVesselVisitExecution(relatedVVN: string): Promise<VesselVisitExecution>;
   
    getVesselVisitExecutionByVVN(relatedVVN: string): Promise<VesselVisitExecution>;
    getAllVesselVisitExecutions(filter?: Filter<VesselVisitExecutionFilter>): Promise<Page<VesselVisitExecution>>;
    
    completeOperation(relatedVVN: string, operationId: string, endTime: Date): Promise<VesselVisitExecution>;
    startOperation(relatedVVN: string, operation: OperationDto): Promise<VesselVisitExecution>;
    updateBerthDetails(relatedVVN: string, dock: string, berthTime: string): Promise<VesselVisitExecution>;
    
    getAllComplementaryTasks(filter?: Filter<any>): Promise<Page<import("@/model/dto/VesselVisitExecutionDto").ComplementaryTaskDto>>;

    count(): Promise<number>;
}