import { ScheduleQueueItem } from "../../../src/domain/scheduleQueue";
import { WorkQueueItem } from "../../../src/domain/workQueueItem";

describe('ScheduleQueueItem', () => {
	describe('constructor', () => {
		it('should create a schedule queue item with all properties', () => {
			const workItem = new WorkQueueItem({
				day: '2024-12-27',
				alg: 'greedy',
				daysAhead: 5
			});

			const now = new Date();
			const scheduledItem = new ScheduleQueueItem({
				id: 'queue-1',
				data: workItem,
				priority: 1,
				requestedAt: now,
				status: 'pending',
				issuer: 'user-123'
			});

			expect(scheduledItem.id).toBe('queue-1');
			expect(scheduledItem.data).toBe(workItem);
			expect(scheduledItem.priority).toBe(1);
			expect(scheduledItem.requestedAt).toBe(now);
			expect(scheduledItem.status).toBe('pending');
			expect(scheduledItem.issuer).toBe('user-123');
			expect(scheduledItem.estimatedStartTime).toBeNull();
			expect(scheduledItem.estimatedEndTime).toBeNull();
		});

		it('should create with optional estimated times', () => {
			const workItem = new WorkQueueItem({
				day: '2024-12-28',
				alg: 'genetic',
				daysAhead: 3
			});

			const now = new Date();
			const startTime = new Date(now.getTime() + 3600000); // 1 hour later
			const endTime = new Date(now.getTime() + 7200000); // 2 hours later

			const scheduledItem = new ScheduleQueueItem({
				id: 'queue-2',
				data: workItem,
				priority: 2,
				requestedAt: now,
				status: 'processing',
				issuer: 'user-456',
				estimatedStartTime: startTime,
				estimatedEndTime: endTime
			});

			expect(scheduledItem.estimatedStartTime).toBe(startTime);
			expect(scheduledItem.estimatedEndTime).toBe(endTime);
		});

		it('should handle optional result parameter', () => {
			const workItem = new WorkQueueItem({
				day: '2024-12-29',
				alg: 'tabu',
				daysAhead: 2
			});

			const now = new Date();
			const result = { scheduled: true, operations: 100 };

			const scheduledItem = new ScheduleQueueItem({
				id: 'queue-3',
				data: workItem,
				priority: 3,
				requestedAt: now,
				result: result,
				status: 'completed',
				issuer: 'user-789'
			});

			expect(scheduledItem.result).toEqual(result);
			expect(scheduledItem.status).toBe('completed');
		});

		it('should initialize null estimated times when not provided', () => {
			const workItem = new WorkQueueItem({
				day: '2024-12-30',
				alg: 'simulated_annealing',
				daysAhead: 1
			});

			const scheduledItem = new ScheduleQueueItem({
				id: 'queue-4',
				data: workItem,
				priority: 0,
				requestedAt: new Date(),
				status: 'pending',
				issuer: 'system'
			});

			expect(scheduledItem.estimatedStartTime).toBeNull();
			expect(scheduledItem.estimatedEndTime).toBeNull();
		});
	});
});
