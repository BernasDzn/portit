import { OperationPlan } from "../../src/domain/operationPlan";
import { OperationPlanMetadata } from "../../src/domain/value/operationPlanMetadata";
import { LinkedList } from "../../src/utils/linkedList";

describe ('Operation Plan Domain Tests', () => {

	it('should create operation plan with correct properties', () => {
		const operationPlan = new OperationPlan({
			relatedVVN: 'VVN123',
			dock: 'DockA',
			operationSchedule: new LinkedList(),
			metadata: new OperationPlanMetadata({ 
				createdBy: 'User1', 
				createdAt: new Date(), 
				algorithmUsed: 'AlgoTest'
			})
		});

		expect(operationPlan.id).toBeDefined();
		expect(operationPlan.relatedVVN).toBe('VVN123');
		expect(operationPlan.dock).toBe('DockA');
		expect(operationPlan.operationSchedule).toBeInstanceOf(LinkedList);
		expect(operationPlan.metadata).toBeInstanceOf(OperationPlanMetadata);
	});

});
