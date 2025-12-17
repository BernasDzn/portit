import OperationPlan from "../domain/operationPlan";
import LinkedList from "../utils/linkedList";
import Operation from "../domain/value/operation";
import OperationPlanMetadata from "../domain/value/operationPlanMetadata";
import { Resource, ResourceType } from "../domain/value/resource";
import { TaskCategoryMapper } from "./taskCategoryMapper";
import { TaskCategoryRepository } from "../repository/taskCategoryRepository";
import { Payload } from "../domain/value/payload";

export class OperationPlanMapper {

	static toSchema(operationPlan: OperationPlan): any {
		return {
			dock: operationPlan.dock,
			relatedVVN: operationPlan.relatedVVN,
			operationSchedule: operationPlan.operationSchedule.toArray().map(op => ({
				operationType: op.operationType.id,
				startTime: op.startTime,
				endTime: op.endTime,
				resources: op.resources.map(res => ({
					name: res.name,
					type: ResourceType[res.type],
                    startTime: res.startTime,
                    endTime: res.endTime
				})),
				payload: op.payload
			})),
			metadata: {
				createdBy: operationPlan.metadata.createdBy,
				createdAt: operationPlan.metadata.createdAt,
				algorithmUsed: operationPlan.metadata.algorithmUsed
			}
		};
	}

    static async fromSchema(doc: any, taskCategoryRepo: TaskCategoryRepository): Promise<OperationPlan> {
        if (!doc.metadata) {
            throw new Error('Operation plan document is missing metadata');
        }
    
        const metadata = new OperationPlanMetadata({
            createdBy: doc.metadata.createdBy,
            createdAt: doc.metadata.createdAt,
            algorithmUsed: doc.metadata.algorithmUsed
        });
    
        const operationSchedule = new LinkedList<Operation>();
    
        if (doc.operationSchedule && doc.operationSchedule.length > 0) {
    
            for (const op of doc.operationSchedule) {
    
                const resources = op.resources.map((res: any) => {
                    return new Resource({
                        name: res.name,
                        type: ResourceType[res.type as keyof typeof ResourceType],
                        startTime: res.startTime,
                        endTime: res.endTime
                    });
                });
    
                const operationType = await taskCategoryRepo.getCategoryById(op.operationType);
                
                // Skip operations with missing task categories
                if (!operationType) {
                    console.warn(`Skipping operation with missing task category: ${op.operationType}`);
                    continue;
                }
                
                const payload = op.payload;
    
                const operation = new Operation({
                    operationType,
                    startTime: op.startTime,
                    endTime: op.endTime,
                    resources,
                    payload
                });
    
                operationSchedule.insertAtEnd(operation);
            }
        }
        
        const operationPlan = new OperationPlan(
            {
                relatedVVN: doc.relatedVVN,
                dock: doc.dock,
                operationSchedule,
                metadata
            }, 
            doc._id.toString()
        );

        return operationPlan;
    }    
}