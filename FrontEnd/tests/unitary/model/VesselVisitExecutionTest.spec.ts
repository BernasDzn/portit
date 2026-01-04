import { describe, it, expect } from 'vitest';
import { VesselVisitExecution, OperationWithStatus, type VesselVisitExecutionStatus, type OperationStatus } from '../../../src/model/VesselVisitExecution';
import { Operation } from '../../../src/model/Operation';
import { TaskCategory } from '../../../src/model/TaskCategory';
import type { ResourceDto } from '../../../src/model/dto/VesselVisitExecutionDto';

describe('OperationWithStatus', () => {
    const mockTaskCategory = new TaskCategory({
        name: 'Loading',
        category: 'LOAD',
        description: 'Loading operations'
    });

    const mockResources: ResourceDto[] = [
        { name: 'RES001', type: 'Crane' }
    ];

    const mockOperation = new Operation({
        id: 'OP001',
        operationType: mockTaskCategory,
        startTime: new Date('2024-01-01T08:00:00Z'),
        endTime: new Date('2024-01-01T16:00:00Z'),
        resources: mockResources
    });

    describe('Constructor', () => {
        it('should create operation with status with required fields', () => {
            const opWithStatus = new OperationWithStatus({
                id: 'OPS001',
                operation: mockOperation,
                status: 'Pending'
            });

            expect(opWithStatus.id).toBe('OPS001');
            expect(opWithStatus.operation).toBe(mockOperation);
            expect(opWithStatus.status).toBe('Pending');
            expect(opWithStatus.impactedOperations).toEqual([]);
        });

        it('should create operation with status with impacted operations', () => {
            const opWithStatus = new OperationWithStatus({
                id: 'OPS001',
                operation: mockOperation,
                status: 'Delayed',
                impactedOperations: ['OP002', 'OP003']
            });

            expect(opWithStatus.impactedOperations).toEqual(['OP002', 'OP003']);
        });
    });

    describe('Status setter', () => {
        it('should update status', () => {
            const opWithStatus = new OperationWithStatus({
                id: 'OPS001',
                operation: mockOperation,
                status: 'Pending'
            });

            opWithStatus.status = 'Started';

            expect(opWithStatus.status).toBe('Started');
        });

        it('should update status multiple times', () => {
            const opWithStatus = new OperationWithStatus({
                id: 'OPS001',
                operation: mockOperation,
                status: 'Pending'
            });

            opWithStatus.status = 'Started';
            expect(opWithStatus.status).toBe('Started');

            opWithStatus.status = 'Completed';
            expect(opWithStatus.status).toBe('Completed');
        });
    });

    describe('toDto', () => {
        it('should convert to DTO correctly', () => {
            const opWithStatus = new OperationWithStatus({
                id: 'OPS001',
                operation: mockOperation,
                status: 'Started',
                impactedOperations: ['OP002']
            });

            const dto = opWithStatus.toDto();

            expect(dto.id).toBe('OPS001');
            expect(dto.status).toBe('Started');
            expect(dto.impactedOperations).toEqual(['OP002']);
            expect(dto.operation).toBeDefined();
        });
    });
});

describe('VesselVisitExecution', () => {
    const mockTaskCategory = new TaskCategory({
        name: 'Loading',
        category: 'LOAD',
        description: 'Loading operations'
    });

    const mockResources: ResourceDto[] = [
        { name: 'RES001', type: 'Crane' }
    ];

    const mockOperation = new Operation({
        id: 'OP001',
        operationType: mockTaskCategory,
        startTime: new Date('2024-01-01T08:00:00Z'),
        endTime: new Date('2024-01-01T16:00:00Z'),
        resources: mockResources
    });

    const mockOpWithStatus = new OperationWithStatus({
        id: 'OPS001',
        operation: mockOperation,
        status: 'Pending'
    });

    describe('Constructor', () => {
        it('should create vessel visit execution with required fields', () => {
            const vve = new VesselVisitExecution({
                id: 'VVE001',
                code: 'VVE-2024-001',
                relatedVVN: 'VVN001',
                operationsExecuted: [mockOpWithStatus],
                status: 'Open',
                createdBy: 'user@example.com'
            });

            expect(vve.id).toBe('VVE001');
            expect(vve.code).toBe('VVE-2024-001');
            expect(vve.relatedVVN).toBe('VVN001');
            expect(vve.operationsExecuted).toEqual([mockOpWithStatus]);
            expect(vve.status).toBe('Open');
            expect(vve.createdBy).toBe('user@example.com');
            expect(vve.dateOpen).toBeUndefined();
            expect(vve.dateClosed).toBeUndefined();
        });

        it('should create vessel visit execution with optional fields', () => {
            const dateOpen = new Date('2024-01-01T08:00:00Z');
            const dateClosed = new Date('2024-01-01T18:00:00Z');

            const vve = new VesselVisitExecution({
                id: 'VVE001',
                code: 'VVE-2024-001',
                relatedVVN: 'VVN001',
                operationsExecuted: [],
                dateOpen,
                dateClosed,
                status: 'Closed',
                createdBy: 'user@example.com'
            });

            expect(vve.dateOpen).toEqual(dateOpen);
            expect(vve.dateClosed).toEqual(dateClosed);
        });

        it('should throw error when relatedVVN is empty', () => {
            expect(() => new VesselVisitExecution({
                id: 'VVE001',
                code: 'VVE-2024-001',
                relatedVVN: '',
                operationsExecuted: [],
                status: 'Open',
                createdBy: 'user@example.com'
            })).toThrow('Related VVN cannot be null or empty.');
        });

        it('should throw error when code is empty', () => {
            expect(() => new VesselVisitExecution({
                id: 'VVE001',
                code: '',
                relatedVVN: 'VVN001',
                operationsExecuted: [],
                status: 'Open',
                createdBy: 'user@example.com'
            })).toThrow('Code cannot be null or empty.');
        });

        it('should throw error when createdBy is empty', () => {
            expect(() => new VesselVisitExecution({
                id: 'VVE001',
                code: 'VVE-2024-001',
                relatedVVN: 'VVN001',
                operationsExecuted: [],
                status: 'Open',
                createdBy: ''
            })).toThrow('Creator email cannot be null or empty.');
        });

        it('should default status to Open when not provided', () => {
            const vve = new VesselVisitExecution({
                id: 'VVE001',
                code: 'VVE-2024-001',
                relatedVVN: 'VVN001',
                operationsExecuted: [],
                status: undefined as any,
                createdBy: 'user@example.com'
            });

            expect(vve.status).toBe('Open');
        });
    });

    describe('addOperation', () => {
        it('should add operation successfully', () => {
            const vve = new VesselVisitExecution({
                id: 'VVE001',
                code: 'VVE-2024-001',
                relatedVVN: 'VVN001',
                operationsExecuted: [],
                status: 'Open',
                createdBy: 'user@example.com'
            });

            vve.addOperation(mockOpWithStatus);

            expect(vve.operationsExecuted).toHaveLength(1);
            expect(vve.operationsExecuted[0]).toBe(mockOpWithStatus);
        });

        it('should throw error when adding null operation', () => {
            const vve = new VesselVisitExecution({
                id: 'VVE001',
                code: 'VVE-2024-001',
                relatedVVN: 'VVN001',
                operationsExecuted: [],
                status: 'Open',
                createdBy: 'user@example.com'
            });

            expect(() => vve.addOperation(null as any)).toThrow('Operation cannot be null.');
        });
    });

    describe('Getters', () => {
        it('should return correct values for all getters', () => {
            const dateOpen = new Date('2024-01-01T08:00:00Z');
            const vve = new VesselVisitExecution({
                id: 'VVE001',
                code: 'VVE-2024-001',
                relatedVVN: 'VVN001',
                operationsExecuted: [mockOpWithStatus],
                dateOpen,
                status: 'Open',
                createdBy: 'user@example.com'
            });

            expect(vve.id).toBe('VVE001');
            expect(vve.code).toBe('VVE-2024-001');
            expect(vve.relatedVVN).toBe('VVN001');
            expect(vve.operationsExecuted).toEqual([mockOpWithStatus]);
            expect(vve.dateOpen).toEqual(dateOpen);
            expect(vve.status).toBe('Open');
            expect(vve.createdBy).toBe('user@example.com');
        });
    });
});
