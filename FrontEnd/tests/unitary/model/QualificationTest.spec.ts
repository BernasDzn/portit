import { describe, it, expect } from 'vitest';
import { Qualification } from '../../../src/model/Qualifications';

describe('Constructor', () => {
	it.each([
		['Q001', 'Forklift Operation'],
		['Q002', 'Crane Operation'],
		['Q003', 'Hazardous Materials Handling']
	])('should create qualification when passing correct data - %s', (code, designation) => {
		const qualification = new Qualification({
			idCode: code,
			qualificationName: designation
		});

		expect(qualification).toBeDefined();
		expect(qualification.idCode).toBe(code);
		expect(qualification.qualificationName).toBe(designation);
		expect(qualification.code).toBe(code);
		expect(qualification.name).toBe(designation);
	});

	it.each([
		['', 'Forklift Operation'],
		['Q002', '']
	])('should throw exception when passing invalid data - %s, %s', (code, designation) => {
		expect(() => new Qualification({
			idCode: code,
			qualificationName: designation
		})).toThrow();
	});
});

describe('UpdateQualificationName', () => {
	it('should update qualification name successfully', () => {
		const qualification = new Qualification({
			idCode: 'Q001',
			qualificationName: 'Old Name'
		});

		qualification.updateQualificationName('New Name');

		expect(qualification.qualificationName).toBe('New Name');
		expect(qualification.name).toBe('New Name');
	});

	it('should throw error when updating with empty name', () => {
		const qualification = new Qualification({
			idCode: 'Q001',
			qualificationName: 'Original Name'
		});

		expect(() => qualification.updateQualificationName('')).toThrow('Qualification name cannot be null or empty.');
	});
});

describe('DisplayName', () => {
	it('should return formatted display name', () => {
		const qualification = new Qualification({
			idCode: 'Q001',
			qualificationName: 'Crane Operation'
		});

		expect(qualification.displayName).toBe('Q001 - Crane Operation');
	});
});

describe('ToDto', () => {
	it('should return correct DTO', () => {
		const qualification = new Qualification({
			idCode: 'Q001',
			qualificationName: 'Forklift Operation'
		});

		const dto = qualification.toDto();

		expect(dto.idCode).toBe(qualification.idCode);
		expect(dto.qualificationName).toBe(qualification.qualificationName);
	});
});
