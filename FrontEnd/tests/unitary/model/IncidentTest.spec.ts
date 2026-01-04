import { describe, it, expect, beforeEach } from 'vitest';
import { Incident, type IncidentSeverity } from '../../../src/model/Incident';
import { IncidentType } from '../../../src/model/IncidentType';

describe('Incident', () => {
    let incidentType: IncidentType;

    beforeEach(() => {
        incidentType = new IncidentType(
            'IT001',
            'Equipment Failure',
            'Equipment malfunction',
            'Major'
        );
    });

    describe('Constructor', () => {
        it('should create incident with all required fields', () => {
            const startTime = new Date('2024-01-01T10:00:00Z');
            const incident = new Incident(
                'INC001',
                incidentType,
                startTime,
                'Major',
                'Test incident',
                'user@example.com'
            );

            expect(incident.bid).toBe('INC001');
            expect(incident.type).toBe(incidentType);
            expect(incident.startTime).toEqual(startTime);
            expect(incident.severity).toBe('Major');
            expect(incident.description).toBe('Test incident');
            expect(incident.createdBy).toBe('user@example.com');
            expect(incident.endTime).toBeUndefined();
        });

        it('should create incident with optional fields', () => {
            const startTime = new Date('2024-01-01T10:00:00Z');
            const endTime = new Date('2024-01-01T12:00:00Z');
            const incident = new Incident(
                'INC001',
                incidentType,
                startTime,
                'Critical',
                'Test incident',
                'user@example.com',
                endTime,
                ['VVE001', 'VVE002']
            );

            expect(incident.endTime).toEqual(endTime);
            expect(incident.affectedVVECodes).toEqual(['VVE001', 'VVE002']);
        });
    });

    describe('validate', () => {
        it('should pass validation for valid incident', () => {
            const incident = new Incident(
                'INC001',
                incidentType,
                new Date(),
                'Major',
                'Test',
                'user@example.com'
            );

            expect(() => incident.validate()).not.toThrow();
        });

        it('should throw error when bid is missing', () => {
            const incident = new Incident(
                '',
                incidentType,
                new Date(),
                'Major',
                'Test',
                'user@example.com'
            );

            expect(() => incident.validate()).toThrow('Incident must have a business ID');
        });

        it('should throw error when end time is before start time', () => {
            const startTime = new Date('2024-01-01T12:00:00Z');
            const endTime = new Date('2024-01-01T10:00:00Z');
            const incident = new Incident(
                'INC001',
                incidentType,
                startTime,
                'Major',
                'Test',
                'user@example.com',
                endTime
            );

            expect(() => incident.validate()).toThrow('End time cannot be before start time');
        });
    });

    describe('isOngoing', () => {
        it('should return true when endTime is undefined', () => {
            const incident = new Incident(
                'INC001',
                incidentType,
                new Date(),
                'Major',
                'Test',
                'user@example.com'
            );

            expect(incident.isOngoing()).toBe(true);
        });

        it('should return false when endTime is defined', () => {
            const incident = new Incident(
                'INC001',
                incidentType,
                new Date('2024-01-01T10:00:00Z'),
                'Major',
                'Test',
                'user@example.com',
                new Date('2024-01-01T12:00:00Z')
            );

            expect(incident.isOngoing()).toBe(false);
        });
    });

    describe('isActive', () => {
        it('should return true when incident is currently active', () => {
            const pastTime = new Date(Date.now() - 3600000); // 1 hour ago
            const incident = new Incident(
                'INC001',
                incidentType,
                pastTime,
                'Major',
                'Test',
                'user@example.com'
            );

            expect(incident.isActive()).toBe(true);
        });

        it('should return false when incident hasnt started yet', () => {
            const futureTime = new Date(Date.now() + 3600000); // 1 hour from now
            const incident = new Incident(
                'INC001',
                incidentType,
                futureTime,
                'Major',
                'Test',
                'user@example.com'
            );

            expect(incident.isActive()).toBe(false);
        });

        it('should return false when incident has ended', () => {
            const pastStartTime = new Date(Date.now() - 7200000); // 2 hours ago
            const pastEndTime = new Date(Date.now() - 3600000); // 1 hour ago
            const incident = new Incident(
                'INC001',
                incidentType,
                pastStartTime,
                'Major',
                'Test',
                'user@example.com',
                pastEndTime
            );

            expect(incident.isActive()).toBe(false);
        });
    });

    describe('getDuration', () => {
        it('should return duration in milliseconds when endTime is defined', () => {
            const startTime = new Date('2024-01-01T10:00:00Z');
            const endTime = new Date('2024-01-01T12:00:00Z');
            const incident = new Incident(
                'INC001',
                incidentType,
                startTime,
                'Major',
                'Test',
                'user@example.com',
                endTime
            );

            const duration = incident.getDuration();
            expect(duration).toBe(7200000); // 2 hours in milliseconds
        });

        it('should return undefined when endTime is undefined', () => {
            const incident = new Incident(
                'INC001',
                incidentType,
                new Date(),
                'Major',
                'Test',
                'user@example.com'
            );

            expect(incident.getDuration()).toBeUndefined();
        });
    });

    describe('hasAffectedVVECodes', () => {
        it('should return true when affectedVVECodes is not empty', () => {
            const incident = new Incident(
                'INC001',
                incidentType,
                new Date(),
                'Major',
                'Test',
                'user@example.com',
                undefined,
                ['VVE001']
            );

            expect(incident.hasAffectedVVECodes()).toBe(true);
        });

        it('should return false when affectedVVECodes is undefined', () => {
            const incident = new Incident(
                'INC001',
                incidentType,
                new Date(),
                'Major',
                'Test',
                'user@example.com'
            );

            expect(incident.hasAffectedVVECodes()).toBe(false);
        });

        it('should return false when affectedVVECodes is empty array', () => {
            const incident = new Incident(
                'INC001',
                incidentType,
                new Date(),
                'Major',
                'Test',
                'user@example.com',
                undefined,
                []
            );

            expect(incident.hasAffectedVVECodes()).toBe(false);
        });
    });

    describe('toDto', () => {
        it('should convert incident to DTO correctly', () => {
            const startTime = new Date('2024-01-01T10:00:00Z');
            const endTime = new Date('2024-01-01T12:00:00Z');
            const incident = new Incident(
                'INC001',
                incidentType,
                startTime,
                'Major',
                'Test incident',
                'user@example.com',
                endTime,
                ['VVE001']
            );

            const dto = incident.toDto();

            expect(dto.type).toBe('IT001');
            expect(dto.startTime).toBe(startTime.toISOString());
            expect(dto.endTime).toBe(endTime.toISOString());
            expect(dto.severity).toBe('Major');
            expect(dto.description).toBe('Test incident');
            expect(dto.affectedVVECodes).toEqual(['VVE001']);
        });
    });
});
