import { Service } from "typedi";
import { OperationPlanRepository } from "../repository/operationPlanRepository";

@Service("vesselVisitExecutionService")
export class VesselVisitExecutionService {

    operationPlanRepository: OperationPlanRepository;

    constructor() {
        this.operationPlanRepository = new OperationPlanRepository();
    }

}