import VesselVisitExecution, { OperationWithStatus } from "../domain/vesselVisitExecution";
import Operation from "../domain/value/operation";
import { Resource, ResourceType } from "../domain/value/resource";
import { Payload } from "../domain/value/payload";
import { TaskCategoryMapper } from "./taskCategoryMapper";
import { TaskCategoryRepository } from "../repository/taskCategoryRepository";

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
        operation: {
          operationType: opWS.operation.operationType.id,
          startTime: opWS.operation.startTime,
          endTime: opWS.operation.endTime,
          resources: opWS.operation.resources.map(res => ({
            name: res.name,
            type: ResourceType[res.type],
            startTime: res.startTime,
            endTime: res.endTime
          })),
          payload: opWS.operation.payload
        }
      }))
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

        const operationTypeDoc = await taskCategoryRepo.getCategoryById(
          opWS.operation.operationType
        );

        const operationType = TaskCategoryMapper.fromSchema(operationTypeDoc);

        // Skip invalid operations
        if (!operationType) {
          console.warn(
            `Skipping executed operation with missing task category: ${opWS.operation.operationType}`
          );
          continue;
        }

        const payload = opWS.operation.payload;

        const operation = new Operation({
          operationType,
          startTime: opWS.operation.startTime,
          endTime: opWS.operation.endTime,
          resources,
          payload
        });

        const operationWithStatus = new OperationWithStatus(
          {
            operation,
            status: opWS.status
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
        status: doc.status
      },
      doc._id.toString()
    );

    return vesselVisitExecution;
  }
}
