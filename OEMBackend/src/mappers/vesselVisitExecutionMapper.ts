import VesselVisitExecution, { OperationWithStatus } from "../domain/vesselVisitExecution";
import Operation from "../domain/value/operation";
import { Resource, ResourceType } from "../domain/value/resource";
import { Payload } from "../domain/value/payload";
import { TaskCategoryMapper } from "./taskCategoryMapper";
import { TaskCategoryRepository } from "../repository/taskCategoryRepository";
import mongoose from "mongoose"; 

export class VesselVisitExecutionMapper {

  static toSchema(vve: VesselVisitExecution): any {

    return {
      code: vve.code,
      createdBy: vve.createdBy,
      relatedVVN: vve.relatedVVN,
      status: vve.status,
      dateOpen: vve.dateOpen,
      dateClosed: vve.dateClosed,
      operationsExecuted: vve.operationsExecuted.map(opWS => ({
        status: opWS.status,
        impactedOperations: opWS.impactedOperations || [],
        operation: {
            id: opWS.operation.id,    
            operationType: opWS.operation.operationType?.id 
              ? new mongoose.Types.ObjectId(opWS.operation.operationType.id)
              : undefined,
            startTime: opWS.operation.startTime,
            endTime: opWS.operation.endTime,
            resources: opWS.operation.resources.map(res => ({
                name: res.name,
                type: res.type,
                startTime: res.startTime,
                endTime: res.endTime
            })),
            payload: opWS.operation.payload
        }
      })),
      dock: vve.dock,
      berthTime: vve.berthTime
    };
  }

  static async fromSchema(
    doc: any,
    taskCategoryRepo: TaskCategoryRepository
  ): Promise<VesselVisitExecution> {

    const operationsExecuted: OperationWithStatus[] = [];

    if (doc.operationsExecuted && doc.operationsExecuted.length > 0) {
      for (const opWS of doc.operationsExecuted) {

        const resources = opWS.operation.resources.map((res: any) => {
          return new Resource({
            name: res.name,
            type: ResourceType[res.type as keyof typeof ResourceType],
            startTime: res.startTime,
            endTime: res.endTime
          });
        });

        const operationType = await taskCategoryRepo.getCategoryById(
          opWS.operation.operationType
        );

        // Skip invalid operations
        if (!operationType) {
          console.warn(
            `Skipping executed operation with missing task category: ${opWS.operation.operationType}`
          );
          continue;
        }

        const payload = opWS.operation.payload;

        const operation = new Operation({
          id: (opWS.operation.id || opWS.operation._id)?.toString(),
          operationType,
          startTime: opWS.operation.startTime,
          endTime: opWS.operation.endTime,
          resources,
          payload
        });

        const operationWithStatus = new OperationWithStatus(
          {
            operation,
            status: opWS.status,
            impactedOperations: opWS.impactedOperations || []
          },
          opWS._id?.toString()
        );

        operationsExecuted.push(operationWithStatus);
      }
    }

    const vesselVisitExecution = new VesselVisitExecution(
      {
        code: doc.code,
        createdBy: doc.createdBy,
        relatedVVN: doc.relatedVVN,
        operationsExecuted,
        dateOpen: doc.dateOpen,
        dateClosed: doc.dateClosed,
        status: doc.status,
        dock: doc.dock,
        berthTime: doc.berthTime
      },
      doc._id.toString()
    );

    return vesselVisitExecution;
  }
}
