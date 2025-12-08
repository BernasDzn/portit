import { OperationPlan } from "../domain/operationPlan";
import { LinkedList } from "../utils/linkedList";
import { Operation, OperationType } from "../domain/value/operation";
import { OperationPlanMetadata } from "../domain/value/operationPlanMetadata";
import { Resource, ResourceType } from "../domain/value/resource";

export class OperationPlanMapper {

	static toSchema(operationPlan: OperationPlan): any {
		return {
			dock: operationPlan.dock,
			relatedVVN: operationPlan.relatedVVN,
			operationSchedule: operationPlan.operationSchedule.toArray().map(op => ({
				operationType: OperationType[op.operationType],
				startTime: op.startTime,
				endTime: op.endTime,
				resources: op.resources.map(res => ({
					name: res.name,
					type: ResourceType[res.type]
				}))
			})),
			metadata: {
				createdBy: operationPlan.metadata.createdBy,
				createdAt: operationPlan.metadata.createdAt,
				algorithmUsed: operationPlan.metadata.algorithmUsed
			}
		};
	}

	static fromSchema(doc: any): OperationPlan {
		if (!doc.metadata) {
			throw new Error('Operation plan document is missing metadata');
		}

		const metadata = new OperationPlanMetadata({
			createdBy: doc.metadata.createdBy,
			createdAt: doc.metadata.createdAt,
			algorithmUsed: doc.metadata.algorithmUsed
		});

		const operationSchedule = new LinkedList<Operation>();

		if (doc.operationSchedule) {
			doc.operationSchedule.forEach((op: any) => {
				const resources = op.resources.map((res: any) => {
					return new Resource({
						name: res.name,
						type: ResourceType[res.type as keyof typeof ResourceType]
					});
				});

				const operation = new Operation({
					operationType: OperationType[op.operationType as keyof typeof OperationType],
					startTime: op.startTime,
					endTime: op.endTime,
					resources: resources
				});

				operationSchedule.insertAtEnd(operation);
			});
		}

		return new OperationPlan({
			id: doc._id.toString(),
			relatedVVN: doc.relatedVVN,
			dock: doc.dock,
			operationSchedule: operationSchedule,
			metadata: metadata,
		});
	}
}