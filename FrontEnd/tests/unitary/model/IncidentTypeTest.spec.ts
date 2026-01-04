import { describe, it, expect } from 'vitest';
import { IncidentType, type Severity } from '../../../src/model/IncidentType';

describe('IncidentType', () => {
    describe('Constructor', () => {
        it('should create incident type with required fields', () => {
            const incidentType = new IncidentType(
                'IT001',
                'Equipment Failure',
                'Equipment malfunction or failure',
                'Major'
            );

            expect(incidentType.bid).toBe('IT001');
            expect(incidentType.name).toBe('Equipment Failure');
            expect(incidentType.description).toBe('Equipment malfunction or failure');
            expect(incidentType.severity).toBe('Major');
            expect(incidentType.subtypeOf).toBeUndefined();
            expect(incidentType.subtypes).toBeUndefined();
        });

        it('should create incident type with parent', () => {
            const parentType = new IncidentType(
                'IT001',
                'Equipment',
                'Equipment issues',
                'Major'
            );

            const childType = new IncidentType(
                'IT002',
                'Crane Failure',
                'Crane malfunction',
                'Critical',
                parentType
            );

            expect(childType.subtypeOf).toBe(parentType);
        });

        it('should create incident type with subtypes', () => {
            const subtype1 = new IncidentType('IT002', 'Type1', 'Desc1', 'Minor');
            const subtype2 = new IncidentType('IT003', 'Type2', 'Desc2', 'Minor');

            const parentType = new IncidentType(
                'IT001',
                'Parent',
                'Parent type',
                'Major',
                undefined,
                [subtype1, subtype2]
            );

            expect(parentType.subtypes).toHaveLength(2);
            expect(parentType.subtypes).toContain(subtype1);
            expect(parentType.subtypes).toContain(subtype2);
        });
    });

    describe('validate', () => {
        it('should pass validation for valid incident type', () => {
            const incidentType = new IncidentType(
                'IT001',
                'Equipment Failure',
                'Equipment malfunction',
                'Major'
            );

            expect(() => incidentType.validate()).not.toThrow();
        });

        it('should throw error when incident type is subtype of itself', () => {
            const incidentType = new IncidentType(
                'IT001',
                'Self Reference',
                'This should fail',
                'Major'
            );

            incidentType.subtypeOf = incidentType;

            expect(() => incidentType.validate()).toThrow(
                'An Incident Type cannot be a subtype of itself. Incident Type id: IT001'
            );
        });

        it('should pass validation when has valid parent', () => {
            const parentType = new IncidentType(
                'IT001',
                'Parent',
                'Parent type',
                'Major'
            );

            const childType = new IncidentType(
                'IT002',
                'Child',
                'Child type',
                'Minor',
                parentType
            );

            expect(() => childType.validate()).not.toThrow();
        });
    });

    describe('addSubtype', () => {
        it('should add subtype correctly', () => {
            const parentType = new IncidentType(
                'IT001',
                'Parent',
                'Parent type',
                'Major'
            );

            const childType = new IncidentType(
                'IT002',
                'Child',
                'Child type',
                'Minor'
            );

            parentType.addSubtype(childType);

            expect(parentType.subtypes).toHaveLength(1);
            expect(parentType.subtypes![0]).toBe(childType);
            expect(childType.subtypeOf).toBe(parentType);
        });

        it('should initialize subtypes array if undefined', () => {
            const parentType = new IncidentType(
                'IT001',
                'Parent',
                'Parent type',
                'Major'
            );

            expect(parentType.subtypes).toBeUndefined();

            const childType = new IncidentType(
                'IT002',
                'Child',
                'Child type',
                'Minor'
            );

            parentType.addSubtype(childType);

            expect(parentType.subtypes).toBeDefined();
            expect(parentType.subtypes).toHaveLength(1);
        });

        it('should add multiple subtypes', () => {
            const parentType = new IncidentType(
                'IT001',
                'Parent',
                'Parent type',
                'Major'
            );

            const child1 = new IncidentType('IT002', 'Child1', 'Desc1', 'Minor');
            const child2 = new IncidentType('IT003', 'Child2', 'Desc2', 'Minor');

            parentType.addSubtype(child1);
            parentType.addSubtype(child2);

            expect(parentType.subtypes).toHaveLength(2);
            expect(child1.subtypeOf).toBe(parentType);
            expect(child2.subtypeOf).toBe(parentType);
        });
    });

    describe('hasParent', () => {
        it('should return true when has parent', () => {
            const parentType = new IncidentType(
                'IT001',
                'Parent',
                'Parent type',
                'Major'
            );

            const childType = new IncidentType(
                'IT002',
                'Child',
                'Child type',
                'Minor',
                parentType
            );

            expect(childType.hasParent()).toBe(true);
        });

        it('should return false when has no parent', () => {
            const incidentType = new IncidentType(
                'IT001',
                'Type',
                'Description',
                'Major'
            );

            expect(incidentType.hasParent()).toBe(false);
        });
    });

    describe('toDto', () => {
        it('should convert to DTO without parent or subtypes', () => {
            const incidentType = new IncidentType(
                'IT001',
                'Equipment Failure',
                'Equipment malfunction',
                'Major'
            );

            const dto = incidentType.toDto();

            expect(dto.bid).toBe('IT001');
            expect(dto.name).toBe('Equipment Failure');
            expect(dto.description).toBe('Equipment malfunction');
            expect(dto.severity).toBe('Major');
            expect(dto.subtypeOf).toBeUndefined();
            expect(dto.subtypes).toBeUndefined();
        });

        it('should convert to DTO with parent', () => {
            const parentType = new IncidentType(
                'IT001',
                'Parent',
                'Parent type',
                'Major'
            );

            const childType = new IncidentType(
                'IT002',
                'Child',
                'Child type',
                'Minor',
                parentType
            );

            const dto = childType.toDto();

            expect(dto.subtypeOf).toBe('IT001');
        });

        it('should convert to DTO with subtypes', () => {
            const child1 = new IncidentType('IT002', 'Child1', 'Desc1', 'Minor');
            const child2 = new IncidentType('IT003', 'Child2', 'Desc2', 'Minor');

            const parentType = new IncidentType(
                'IT001',
                'Parent',
                'Parent type',
                'Major',
                undefined,
                [child1, child2]
            );

            const dto = parentType.toDto();

            expect(dto.subtypes).toEqual(['IT002', 'IT003']);
        });
    });
});
