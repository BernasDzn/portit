import { describe, it, expect } from 'vitest';
import { TaskCategory } from '../../../src/model/TaskCategory';

describe('TaskCategory', () => {
    describe('Constructor', () => {
        it('should create task category with all fields', () => {
            const taskCategory = new TaskCategory({
                name: 'Loading',
                category: 'LOAD',
                description: 'Loading operations'
            });

            expect(taskCategory.name).toBe('Loading');
            expect(taskCategory.category).toBe('LOAD');
            expect(taskCategory.description).toBe('Loading operations');
        });

        it('should throw error when name is empty', () => {
            expect(() => new TaskCategory({
                name: '',
                category: 'LOAD',
                description: 'Description'
            })).toThrow('Name cannot be null or empty.');
        });

        it('should throw error when name is null', () => {
            expect(() => new TaskCategory({
                name: null as any,
                category: 'LOAD',
                description: 'Description'
            })).toThrow('Name cannot be null or empty.');
        });

        it('should throw error when category is empty', () => {
            expect(() => new TaskCategory({
                name: 'Loading',
                category: '',
                description: 'Description'
            })).toThrow('Category cannot be null or empty.');
        });

        it('should throw error when category is null', () => {
            expect(() => new TaskCategory({
                name: 'Loading',
                category: null as any,
                description: 'Description'
            })).toThrow('Category cannot be null or empty.');
        });

        it('should throw error when description is empty', () => {
            expect(() => new TaskCategory({
                name: 'Loading',
                category: 'LOAD',
                description: ''
            })).toThrow('Description cannot be null or empty.');
        });

        it('should throw error when description is null', () => {
            expect(() => new TaskCategory({
                name: 'Loading',
                category: 'LOAD',
                description: null as any
            })).toThrow('Description cannot be null or empty.');
        });
    });

    describe('Getters', () => {
        it('should return correct values for all getters', () => {
            const taskCategory = new TaskCategory({
                name: 'Unloading',
                category: 'UNLOAD',
                description: 'Unloading operations'
            });

            expect(taskCategory.name).toBe('Unloading');
            expect(taskCategory.category).toBe('UNLOAD');
            expect(taskCategory.description).toBe('Unloading operations');
        });
    });

    describe('toDto', () => {
        it('should convert task category to DTO correctly', () => {
            const taskCategory = new TaskCategory({
                name: 'Loading',
                category: 'LOAD',
                description: 'Loading operations'
            });

            const dto = taskCategory.toDto();

            expect(dto.name).toBe('Loading');
            expect(dto.category).toBe('LOAD');
            expect(dto.description).toBe('Loading operations');
        });

        it('should preserve all fields in DTO', () => {
            const taskCategory = new TaskCategory({
                name: 'Inspection',
                category: 'INSPECT',
                description: 'Vessel inspection tasks'
            });

            const dto = taskCategory.toDto();

            expect(Object.keys(dto)).toEqual(['name', 'category', 'description']);
            expect(dto.name).toBe('Inspection');
            expect(dto.category).toBe('INSPECT');
            expect(dto.description).toBe('Vessel inspection tasks');
        });
    });

    describe('fromDto', () => {
        it('should create task category from DTO', () => {
            const dto = {
                name: 'Loading',
                category: 'LOAD',
                description: 'Loading operations'
            };

            const taskCategory = TaskCategory.fromDto(dto);

            expect(taskCategory).toBeInstanceOf(TaskCategory);
            expect(taskCategory.name).toBe('Loading');
            expect(taskCategory.category).toBe('LOAD');
            expect(taskCategory.description).toBe('Loading operations');
        });

        it('should create multiple task categories from DTOs', () => {
            const dto1 = {
                name: 'Loading',
                category: 'LOAD',
                description: 'Loading ops'
            };

            const dto2 = {
                name: 'Unloading',
                category: 'UNLOAD',
                description: 'Unloading ops'
            };

            const cat1 = TaskCategory.fromDto(dto1);
            const cat2 = TaskCategory.fromDto(dto2);

            expect(cat1.name).toBe('Loading');
            expect(cat2.name).toBe('Unloading');
        });
    });

    describe('Round-trip conversion', () => {
        it('should maintain data through toDto and fromDto', () => {
            const original = new TaskCategory({
                name: 'Maintenance',
                category: 'MAINT',
                description: 'Maintenance tasks'
            });

            const dto = original.toDto();
            const reconstructed = TaskCategory.fromDto(dto);

            expect(reconstructed.name).toBe(original.name);
            expect(reconstructed.category).toBe(original.category);
            expect(reconstructed.description).toBe(original.description);
        });
    });
});
