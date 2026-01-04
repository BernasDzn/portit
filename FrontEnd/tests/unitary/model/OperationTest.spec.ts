import { describe, it, expect, vi } from 'vitest';
import { Operation } from '../../../src/model/Operation';
import { TaskCategory } from '../../../src/model/TaskCategory';
import type { ResourceDto } from '../../../src/model/dto/VesselVisitExecutionDto';

describe('Operation', () => {
    const mockTaskCategory = new TaskCategory({
        name: 'Loading',
        category: 'LOAD',
        description: 'Loading operations'
    });

    const mockResources: ResourceDto[] = [
        { name: 'RES001', type: 'Crane' }
    ];

    describe('Constructor', () => {
        it('should create operation with all required fields', () => {
            const startTime = new Date('2024-01-01T08:00:00Z');
            const endTime = new Date('2024-01-01T16:00:00Z');

            const operation = new Operation({
                operationType: mockTaskCategory,
                startTime,
                endTime,
                resources: mockResources
            });

            expect(operation.operationType).toBe(mockTaskCategory);
            expect(operation.startTime).toEqual(startTime);
            expect(operation.endTime).toEqual(endTime);
            expect(operation.resources).toEqual(mockResources);
            expect(operation.id).toBeUndefined();
            expect(operation.payload).toBeUndefined();
        });

        it('should create operation with optional fields', () => {
            const startTime = new Date('2024-01-01T08:00:00Z');
            const endTime = new Date('2024-01-01T16:00:00Z');
            const payload = { containersCount: 100 };

            const operation = new Operation({
                id: 'OP001',
                operationType: mockTaskCategory,
                startTime,
                endTime,
                resources: mockResources,
                payload
            });

            expect(operation.id).toBe('OP001');
            expect(operation.payload).toEqual(payload);
        });

        it('should throw error when start time is not before end time', () => {
            const startTime = new Date('2024-01-01T16:00:00Z');
            const endTime = new Date('2024-01-01T08:00:00Z');

            expect(() => new Operation({
                operationType: mockTaskCategory,
                startTime,
                endTime,
                resources: mockResources
            })).toThrow('Start time must be before end time.');
        });

        it('should throw error when start time equals end time', () => {
            const time = new Date('2024-01-01T08:00:00Z');

            expect(() => new Operation({
                operationType: mockTaskCategory,
                startTime: time,
                endTime: time,
                resources: mockResources
            })).toThrow('Start time must be before end time.');
        });

        it('should throw error when resources is empty', () => {
            const startTime = new Date('2024-01-01T08:00:00Z');
            const endTime = new Date('2024-01-01T16:00:00Z');

            expect(() => new Operation({
                operationType: mockTaskCategory,
                startTime,
                endTime,
                resources: []
            })).toThrow('Operation must have at least one resource.');
        });

        it('should throw error when resources is null', () => {
            const startTime = new Date('2024-01-01T08:00:00Z');
            const endTime = new Date('2024-01-01T16:00:00Z');

            expect(() => new Operation({
                operationType: mockTaskCategory,
                startTime,
                endTime,
                resources: null as any
            })).toThrow('Operation must have at least one resource.');
        });
    });

    describe('Getters', () => {
        it('should return correct values for all getters', () => {
            const startTime = new Date('2024-01-01T08:00:00Z');
            const endTime = new Date('2024-01-01T16:00:00Z');
            const payload = { data: 'test' };

            const operation = new Operation({
                id: 'OP001',
                operationType: mockTaskCategory,
                startTime,
                endTime,
                resources: mockResources,
                payload
            });

            expect(operation.id).toBe('OP001');
            expect(operation.operationType).toBe(mockTaskCategory);
            expect(operation.startTime).toEqual(startTime);
            expect(operation.endTime).toEqual(endTime);
            expect(operation.resources).toEqual(mockResources);
            expect(operation.payload).toEqual(payload);
        });
    });

    describe('toDto', () => {
        it('should convert operation to DTO correctly', () => {
            const startTime = new Date('2024-01-01T08:00:00Z');
            const endTime = new Date('2024-01-01T16:00:00Z');
            const payload = { containersCount: 50 };

            const operation = new Operation({
                id: 'OP001',
                operationType: mockTaskCategory,
                startTime,
                endTime,
                resources: mockResources,
                payload
            });

            const dto = operation.toDto();

            expect(dto.id).toBe('OP001');
            expect(dto.type).toEqual(mockTaskCategory.toDto());
            expect(dto.startTime).toBe(startTime.toISOString());
            expect(dto.endTime).toBe(endTime.toISOString());
            expect(dto.resources).toEqual(mockResources);
            expect(dto.payload).toEqual(payload);
        });

        it('should convert operation to DTO with empty id when id is undefined', () => {
            const startTime = new Date('2024-01-01T08:00:00Z');
            const endTime = new Date('2024-01-01T16:00:00Z');

            const operation = new Operation({
                operationType: mockTaskCategory,
                startTime,
                endTime,
                resources: mockResources
            });

            const dto = operation.toDto();

            expect(dto.id).toBe('');
        });

        it('should convert operation to DTO with null payload when payload is undefined', () => {
            const startTime = new Date('2024-01-01T08:00:00Z');
            const endTime = new Date('2024-01-01T16:00:00Z');

            const operation = new Operation({
                operationType: mockTaskCategory,
                startTime,
                endTime,
                resources: mockResources
            });

            const dto = operation.toDto();

            expect(dto.payload).toBeNull();
        });
    });
});
