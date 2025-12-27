import VesselVisitExecution, { OperationWithStatus } from "../../../src/domain/vesselVisitExecution";
import Operation from "../../../src/domain/value/operation";
import { Resource, ResourceType } from "../../../src/domain/value/resource";
import LinkedList from "../../../src/utils/linkedList";

describe('VesselVisitExecution', () => {
	describe('constructor', () => {
		it('should create a vessel visit execution with all properties', () => {
			const vve = new VesselVisitExecution({
				code: 'VVE-001',
				relatedVVN: 'VVN-001',
				operationsExecuted: [],
				status: 'Open',
				createdBy: 'user-123'
			});

			expect(vve.code).toBe('VVE-001');
			expect(vve.relatedVVN).toBe('VVN-001');
			expect(vve.status).toBe('Open');
			expect(vve.createdBy).toBe('user-123');
			expect(vve.operationsExecuted).toEqual([]);
		});

		it('should default operationsExecuted to empty array when not provided', () => {
			const vve = new VesselVisitExecution({
				code: 'VVE-003',
				relatedVVN: 'VVN-003',
				operationsExecuted: [],
				status: 'Open',
				createdBy: 'user-789'
			});

			expect(vve.operationsExecuted).toEqual([]);
		});

		it('should set optional dates', () => {
			const now = new Date();
			const closedDate = new Date(now.getTime() + 86400000); // 1 day later

			const vve = new VesselVisitExecution({
				code: 'VVE-004',
				relatedVVN: 'VVN-004',
				operationsExecuted: [],
				status: 'Closed',
				createdBy: 'user-sys',
				dateOpen: now,
				dateClosed: closedDate
			});

			expect(vve.dateOpen).toBe(now);
			expect(vve.dateClosed).toBe(closedDate);
		});
	});

	describe('status management', () => {
		it('should allow changing status', () => {
			const vve = new VesselVisitExecution({
				code: 'VVE-005',
				relatedVVN: 'VVN-005',
				operationsExecuted: [],
				status: 'Open',
				createdBy: 'user-123'
			});

			expect(vve.status).toBe('Open');

			vve.status = 'Closed';
			expect(vve.status).toBe('Closed');
		});

		it('should support Open and Closed status values', () => {
			const statuses: Array<'Open' | 'Closed'> = ['Open', 'Closed'];

			statuses.forEach(status => {
				const vve = new VesselVisitExecution({
					code: `VVE-${status}`,
					relatedVVN: `VVN-${status}`,
					operationsExecuted: [],
					status: status,
					createdBy: 'user-123'
				});

				expect(vve.status).toBe(status);
			});
		});
	});

	describe('toDto', () => {
		it('should convert to DTO with correct structure', () => {
			const now = new Date();
			const vve = new VesselVisitExecution({
				code: 'VVE-006',
				relatedVVN: 'VVN-006',
				operationsExecuted: [],
				status: 'Open',
				createdBy: 'user-123',
				dateOpen: now
			});

			const dto = vve.toDto();

			expect(dto.id).toBeDefined();
			expect(dto.code).toBe('VVE-006');
			expect(dto.relatedVVN).toBe('VVN-006');
			expect(dto.status).toBe('Open');
			expect(dto.createdBy).toBe('user-123');
			expect(dto.dateOpen).toBe(now);
			expect(dto.operationsExecuted).toEqual([]);
		});

		it('should include all optional fields in DTO', () => {
			const now = new Date();
			const closedDate = new Date(now.getTime() + 3600000); // 1 hour later

			const vve = new VesselVisitExecution({
				code: 'VVE-007',
				relatedVVN: 'VVN-007',
				operationsExecuted: [],
				status: 'Closed',
				createdBy: 'user-456',
				dateOpen: now,
				dateClosed: closedDate
			});

			const dto = vve.toDto();

			expect(dto.dateOpen).toBe(now);
			expect(dto.dateClosed).toBe(closedDate);
		});

		it('should handle undefined optional dates in DTO', () => {
			const vve = new VesselVisitExecution({
				code: 'VVE-008',
				relatedVVN: 'VVN-008',
				operationsExecuted: [],
				status: 'Open',
				createdBy: 'user-789'
			});

			const dto = vve.toDto();

			expect(dto.dateOpen).toBeUndefined();
			expect(dto.dateClosed).toBeUndefined();
		});
	});

	describe('id generation', () => {
		it('should have a unique id', () => {
			const vve1 = new VesselVisitExecution({
				code: 'VVE-009',
				relatedVVN: 'VVN-009',
				operationsExecuted: [],
				status: 'Open',
				createdBy: 'user-123'
			});

			const vve2 = new VesselVisitExecution({
				code: 'VVE-010',
				relatedVVN: 'VVN-010',
				operationsExecuted: [],
				status: 'Open',
				createdBy: 'user-123'
			});

			expect(vve1.id).toBeDefined();
			expect(vve2.id).toBeDefined();
			expect(vve1.id).not.toBe(vve2.id);
		});
	});
});
