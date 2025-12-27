import { WorkQueueItem } from "../../../src/domain/workQueueItem";

describe('WorkQueueItem', () => {
	describe('constructor', () => {
		it('should create a work queue item with all properties', () => {
			const item = new WorkQueueItem({
				day: '2024-12-27',
				alg: 'greedy',
				daysAhead: 5
			});

			expect(item.day).toBe('2024-12-27');
			expect(item.alg).toBe('greedy');
			expect(item.daysAhead).toBe(5);
		});

		it('should handle different algorithm types', () => {
			const algorithms = ['greedy', 'genetic', 'tabu', 'simulated_annealing'];

			algorithms.forEach(alg => {
				const item = new WorkQueueItem({
					day: '2024-12-27',
					alg: alg,
					daysAhead: 3
				});

				expect(item.alg).toBe(alg);
			});
		});

		it('should support different days ahead values', () => {
			const daysAheadValues = [1, 2, 5, 10, 30];

			daysAheadValues.forEach(days => {
				const item = new WorkQueueItem({
					day: '2024-12-27',
					alg: 'greedy',
					daysAhead: days
				});

				expect(item.daysAhead).toBe(days);
			});
		});

		it('should handle different date formats', () => {
			const dates = [
				'2024-12-27',
				'2024-01-01',
				'2025-06-15'
			];

			dates.forEach(date => {
				const item = new WorkQueueItem({
					day: date,
					alg: 'genetic',
					daysAhead: 2
				});

				expect(item.day).toBe(date);
			});
		});
	});
});
