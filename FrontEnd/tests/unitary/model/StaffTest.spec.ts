import { describe, it, expect } from 'vitest';
import { Staff, StaffStatus } from '../../../src/model/Staff';
import { Qualification } from '../../../src/model/Qualifications';
import { FullWeek } from '../../../src/model/values/OperationalWindow';
import type { OperationalWindow } from '../../../src/model/values/OperationalWindow';

const validOperationalWindow: OperationalWindow = FullWeek();

describe('Constructor', () => {
	it.each([
		['123456789', 'Staff Name', 'staff@example.com', '910000000'],
		['MEC001', 'Another Staff', 'another@example.com', '920000000'],
		['EMP123', 'Test Staff', 'test@example.com', '930000000']
	])('should create staff without qualifications - %s', (mechanographicNumber, name, email, phoneNumber) => {
		const staff = new Staff({
			mechanographicNumber,
			name,
			email,
			phoneNumber,
			status: StaffStatus.Available,
			operationalWindow: validOperationalWindow,
			qualifications: []
		});

		expect(staff).toBeDefined();
		expect(staff.mechanographicNumber).toBe(mechanographicNumber);
		expect(staff.name).toBe(name);
		expect(staff.email).toBe(email);
		expect(staff.phoneNumber).toBe(phoneNumber);
	});
});

describe('UpdateStatus', () => {
	it('should update status with valid status successfully', () => {
		const staff = new Staff({
			mechanographicNumber: 'MEC001',
			name: 'Name',
			email: 'a@b.com',
			phoneNumber: '900000000',
			status: StaffStatus.Available,
			operationalWindow: validOperationalWindow,
			qualifications: []
		});

		staff.updateStatus(StaffStatus.TemporarilyReassigned);

		expect(staff.status).toBe(StaffStatus.TemporarilyReassigned);
	});

	it('should throw error when updating with invalid status', () => {
		const staff = new Staff({
			mechanographicNumber: 'MEC002',
			name: 'Name',
			email: 'a@b.com',
			phoneNumber: '900000000',
			status: StaffStatus.Available,
			operationalWindow: validOperationalWindow,
			qualifications: []
		});

		expect(() => staff.updateStatus(999)).toThrow('Trying to update to invalid Status.');
	});
});

describe('Deactivate', () => {
	it('should set status to Unavailable and isActive to false', () => {
		const staff = new Staff({
			mechanographicNumber: 'MEC003',
			name: 'Name',
			email: 'a@b.com',
			phoneNumber: '900000000',
			status: StaffStatus.Available,
			operationalWindow: validOperationalWindow,
			qualifications: []
		});

		staff.deactivate();

		expect(staff.status).toBe(StaffStatus.Unavailable);
		expect(staff.isActive).toBe(false);
	});
});

describe('Update', () => {
	it('should change properties correctly', () => {
		const staff = new Staff({
			mechanographicNumber: 'MEC004',
			name: 'OldName',
			email: 'old@a.com',
			phoneNumber: '900000001',
			status: StaffStatus.Available,
			operationalWindow: validOperationalWindow,
			qualifications: []
		});

		const newQual = new Qualification({
			idCode: 'Q1',
			qualificationName: 'Qualification'
		});
		const qualifications = [newQual];

		staff.update('NewName', 'new@a.com', '900000002', StaffStatus.Available, FullWeek(), qualifications);

		expect(staff.name).toBe('NewName');
		expect(staff.email).toBe('new@a.com');
		expect(staff.phoneNumber).toBe('900000002');
		expect(staff.status).toBe(StaffStatus.Available);
		expect(staff.qualifications).toHaveLength(1);
	});
});

describe('ToDto', () => {
	it('should return correct DTO', () => {
		const staff = new Staff({
			mechanographicNumber: 'MEC005',
			name: 'Name',
			email: 'a@b.com',
			phoneNumber: '900000000',
			status: StaffStatus.Available,
			operationalWindow: validOperationalWindow,
			qualifications: []
		});

		const dto = staff.toDto();

		expect(dto.mechanographicNumber).toBe(staff.mechanographicNumber);
		expect(dto.name).toBe(staff.name);
		expect(dto.email).toBe(staff.email);
		expect(dto.phoneNumber).toBe(staff.phoneNumber);
		expect(dto.status).toBe(staff.status);
	});
});
