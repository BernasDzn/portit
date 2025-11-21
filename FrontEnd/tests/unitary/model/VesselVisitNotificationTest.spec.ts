import { describe, it, expect, beforeEach } from 'vitest';
import { VesselVisitNotification, NotificationDecision } from '../../../src/model/VesselVisitNotification';
import { VesselVisitNotificationStatus, NotificationDecisionStatus } from '../../../src/model/dto/VesselVisitNotificationDto';
import type { Representative } from '../../../src/model/Representative';
import { Vessel } from '../../../src/model/Vessel';
import { VesselType } from '../../../src/model/VesselType';
import type { ShippingAgentOrganization } from '../../../src/model/ShippingAgentOrganization';
import type { CrewDetails, CargoManifestItem } from '../../../src/model/dto/VesselVisitNotificationDto';

describe('VesselVisitNotification', () => {
	let representative: Representative;
	let vessel: Vessel;
	let crewDetails: CrewDetails;
	let loadCargoManifest: CargoManifestItem[];
	let unloadCargoManifest: CargoManifestItem[];

	beforeEach(() => {
		const vesselType = new VesselType({
			name: 'Panamax',
			description: 'Max size for Panama Canal',
			maxNumberOfRows: 20,
			maxNumberOfBays: 10,
			maxNumberOfTiers: 5,
			physicalCharacteristics: { length: 300, depth: 15, draft: 12 }
		});

		const owner: ShippingAgentOrganization = {
			name: 'Maersk',
			altNames: ['A major shipping company'],
			taxNumber: 'PT252252252',
			address: {
				id: '1',
				street: '123 Ocean Drive',
				city: 'Copenhagen',
				country: 'Denmark',
				zipCode: '2100'
			},
			representatives: [{
				name: 'rep',
				citizenshipId: '123456789',
				emailAddress: 'email@email.com',
				phone: '4512345678'
			}]
		};

		representative = {
			name: 'rep',
			citizenshipId: '123456789',
			emailAddress: 'email@email.com',
			phone: '4512345678'
		};

		vessel = new Vessel({
			name: 'Test Vessel',
			imoNumber: 'IMO1234567',
			type: vesselType,
			owner: owner,
			physicalCharacteristics: { length: 200, depth: 10, draft: 8 }
		});

		crewDetails = {
			captain: { value: 'Ana Costa' },
			totalCrewMembers: 3,
			safetyOfficers: [
				{ citizenId: 'CITIZEN001', name: 'John Doe', nationality: 'US' },
				{ citizenId: 'CITIZEN002', name: 'Jane Smith', nationality: 'GB' }
			]
		};

		unloadCargoManifest = [
			{
				position: { row: '10', bay: '5', tier: '10' },
				storageAreaCode: 'YARD1',
				container: {
					containerNumber: 'CMAU2468103',
					cargoType: 0,
					description: 'chilly yummy food'
				}
			},
			{
				position: { row: '12', bay: '6', tier: '8' },
				storageAreaCode: 'YARD2',
				container: {
					containerNumber: 'ABCD1234560',
					cargoType: 1,
					description: 'various electronic items'
				}
			}
		];

		loadCargoManifest = [
			{
				position: { row: '14', bay: '7', tier: '9' },
				storageAreaCode: 'YARD1',
				container: {
					containerNumber: 'MSKU1234565',
					cargoType: 1,
					description: 'assorted electronics'
				}
			},
			{
				position: { row: '16', bay: '8', tier: '6' },
				storageAreaCode: 'YARD2',
				container: {
					containerNumber: 'TGHU7654320',
					cargoType: 6,
					description: 'industrial machinery parts'
				}
			}
		];
	});

	describe('Constructor', () => {
		it.each([
			['2024-07-01T10:00:00Z', '2024-07-01T12:00:00Z', false, undefined],
			['2024-08-15T14:30:00Z', '2024-08-15T16:30:00Z', true, 'Requires special handling'],
			['2024-09-20T08:00:00Z', '2024-09-20T10:00:00Z', false, 'Fragile cargo'],
			['2024-10-05T09:15:00Z', '2024-10-05T11:45:00Z', true, undefined]
		])('should create notification successfully - %s to %s', (arrivalStr, departureStr, isCargoHazardous, specialRequirements) => {
			const arrival = new Date(arrivalStr);
			const departure = new Date(departureStr);

			const notification = new VesselVisitNotification({
				notificationId: 'VVN001',
				status: VesselVisitNotificationStatus.InProgress,
				expectedArrival: arrival,
				expectedDeparture: departure,
				isCargoHazardous,
				specialRequirements,
				crewDetails,
				loadCargoManifest,
				unloadCargoManifest,
				vessel,
				submitter: representative
			});

			expect(notification).toBeDefined();
			expect(notification.notificationId).toBe('VVN001');
			expect(notification.expectedArrival).toEqual(arrival);
			expect(notification.expectedDeparture).toEqual(departure);
			expect(notification.isCargoHazardous).toBe(isCargoHazardous);
		});

		it.each([
			['', 'Notification ID cannot be null or empty.'],
		])('should throw error with invalid notification ID - %s', (notificationId, expectedError) => {
			expect(() => new VesselVisitNotification({
				notificationId,
				status: VesselVisitNotificationStatus.InProgress,
				expectedArrival: new Date('2024-07-01T10:00:00Z'),
				expectedDeparture: new Date('2024-07-01T12:00:00Z'),
				isCargoHazardous: false,
				vessel,
				submitter: representative
			})).toThrow(expectedError);
		});

		it('should throw error when vessel is null', () => {
			expect(() => new VesselVisitNotification({
				notificationId: 'VVN001',
				status: VesselVisitNotificationStatus.InProgress,
				expectedArrival: new Date('2024-07-01T10:00:00Z'),
				expectedDeparture: new Date('2024-07-01T12:00:00Z'),
				isCargoHazardous: false,
				vessel: null as any,
				submitter: representative
			})).toThrow('Vessel cannot be null.');
		});

		it('should throw error when submitter is null', () => {
			expect(() => new VesselVisitNotification({
				notificationId: 'VVN001',
				status: VesselVisitNotificationStatus.InProgress,
				expectedArrival: new Date('2024-07-01T10:00:00Z'),
				expectedDeparture: new Date('2024-07-01T12:00:00Z'),
				isCargoHazardous: false,
				vessel,
				submitter: null as any
			})).toThrow('Submitter cannot be null.');
		});

		it('should throw error when arrival is after departure', () => {
			expect(() => new VesselVisitNotification({
				notificationId: 'VVN001',
				status: VesselVisitNotificationStatus.InProgress,
				expectedArrival: new Date('2024-07-01T12:00:00Z'),
				expectedDeparture: new Date('2024-07-01T10:00:00Z'),
				isCargoHazardous: false,
				vessel,
				submitter: representative
			})).toThrow('Expected arrival must be before expected departure.');
		});
	});

	describe('Submit', () => {
		it('should submit valid notification', () => {
			const notification = new VesselVisitNotification({
				notificationId: 'VVN001',
				status: VesselVisitNotificationStatus.InProgress,
				expectedArrival: new Date('2024-07-01T10:00:00Z'),
				expectedDeparture: new Date('2024-07-01T12:00:00Z'),
				isCargoHazardous: false,
				crewDetails,
				vessel,
				submitter: representative
			});

			notification.submit();

			expect(notification.status).toBe(VesselVisitNotificationStatus.ApprovalPending);
		});

		it('should throw error when submitting hazardous cargo without safety officers', () => {
			const notification = new VesselVisitNotification({
				notificationId: 'VVN001',
				status: VesselVisitNotificationStatus.InProgress,
				expectedArrival: new Date('2024-07-01T10:00:00Z'),
				expectedDeparture: new Date('2024-07-01T12:00:00Z'),
				isCargoHazardous: true,
				crewDetails: undefined,
				vessel,
				submitter: representative
			});

			expect(() => notification.submit()).toThrow('Notifications with hazardous cargo must include at least one safety officer in the crew details.');
		});

		it('should throw error when submitting already submitted notification', () => {
			const notification = new VesselVisitNotification({
				notificationId: 'VVN001',
				status: VesselVisitNotificationStatus.InProgress,
				expectedArrival: new Date('2024-07-01T10:00:00Z'),
				expectedDeparture: new Date('2024-07-01T12:00:00Z'),
				isCargoHazardous: false,
				crewDetails,
				vessel,
				submitter: representative
			});

			notification.submit();

			expect(() => notification.submit()).toThrow('Only notifications in progress can be submitted.');
		});
	});

	describe('Update', () => {
		it('should update notification with valid data', () => {
			const notification = new VesselVisitNotification({
				notificationId: 'VVN001',
				status: VesselVisitNotificationStatus.InProgress,
				expectedArrival: new Date('2024-07-01T10:00:00Z'),
				expectedDeparture: new Date('2024-07-01T12:00:00Z'),
				isCargoHazardous: true,
				crewDetails,
				vessel,
				submitter: representative
			});

			notification.update(
				new Date('2024-07-05T10:00:00Z'),
				new Date('2024-07-10T12:00:00Z'),
				false
			);

			expect(notification.expectedArrival).toEqual(new Date('2024-07-05T10:00:00Z'));
			expect(notification.expectedDeparture).toEqual(new Date('2024-07-10T12:00:00Z'));
			expect(notification.isCargoHazardous).toBe(false);
		});

		it('should throw error when updating approval pending notification', () => {
			const notification = new VesselVisitNotification({
				notificationId: 'VVN001',
				status: VesselVisitNotificationStatus.InProgress,
				expectedArrival: new Date('2024-07-01T10:00:00Z'),
				expectedDeparture: new Date('2024-07-01T12:00:00Z'),
				isCargoHazardous: false,
				crewDetails,
				vessel,
				submitter: representative
			});

			notification.submit();

			expect(() => notification.update(
				new Date('2024-07-05T12:00:00Z'),
				new Date('2024-07-10T10:00:00Z'),
				false
			)).toThrow('Only notifications in progress can be updated.');
		});

		it('should throw error when updating with arrival after departure', () => {
			const notification = new VesselVisitNotification({
				notificationId: 'VVN001',
				status: VesselVisitNotificationStatus.InProgress,
				expectedArrival: new Date('2024-07-01T10:00:00Z'),
				expectedDeparture: new Date('2024-07-01T12:00:00Z'),
				isCargoHazardous: false,
				crewDetails,
				vessel,
				submitter: representative
			});

			expect(() => notification.update(
				new Date('2024-07-10T12:00:00Z'),
				new Date('2024-07-05T10:00:00Z'),
				false
			)).toThrow('Expected arrival must be before expected departure.');
		});
	});

	describe('AddDecision', () => {
		it.each([
			[NotificationDecisionStatus.Accepted, undefined],
			[NotificationDecisionStatus.Rejected, 'Insufficient safety measures']
		])('should add valid decision - %s', (status, reason) => {
			const notification = new VesselVisitNotification({
				notificationId: 'VVN001',
				status: VesselVisitNotificationStatus.InProgress,
				expectedArrival: new Date('2024-07-01T10:00:00Z'),
				expectedDeparture: new Date('2024-07-01T12:00:00Z'),
				isCargoHazardous: true,
				crewDetails,
				vessel,
				submitter: representative
			});

			notification.submit();

			const decision = new NotificationDecision({
				status,
				decisionDate: new Date(),
				officerEmail: 'officer@email.com',
				assignedDockCode: status === NotificationDecisionStatus.Accepted ? 'DCK003' : undefined,
				reason,
				isFinal: true
			});

			notification.addDecision(decision);

			expect(notification.notificationDecisions).toContain(decision);
		});

		it('should throw error when adding null decision', () => {
			const notification = new VesselVisitNotification({
				notificationId: 'VVN001',
				status: VesselVisitNotificationStatus.InProgress,
				expectedArrival: new Date('2024-07-01T10:00:00Z'),
				expectedDeparture: new Date('2024-07-01T12:00:00Z'),
				isCargoHazardous: true,
				crewDetails,
				vessel,
				submitter: representative
			});

			notification.submit();

			expect(() => notification.addDecision(null as any)).toThrow('Decision cannot be null.');
		});

		it('should throw error when adding decision to in-progress notification', () => {
			const notification = new VesselVisitNotification({
				notificationId: 'VVN001',
				status: VesselVisitNotificationStatus.InProgress,
				expectedArrival: new Date('2024-07-01T10:00:00Z'),
				expectedDeparture: new Date('2024-07-01T12:00:00Z'),
				isCargoHazardous: false,
				crewDetails,
				vessel,
				submitter: representative
			});

			const decision = new NotificationDecision({
				status: NotificationDecisionStatus.Accepted,
				decisionDate: new Date(),
				officerEmail: 'officer@email.com',
				assignedDockCode: 'DCK003',
				isFinal: true
			});

			expect(() => notification.addDecision(decision)).toThrow('Decisions can only be added to notifications pending approval.');
		});

		it('should throw error when adding decision with older date', () => {
			const notification = new VesselVisitNotification({
				notificationId: 'VVN001',
				status: VesselVisitNotificationStatus.InProgress,
				expectedArrival: new Date('2024-07-01T10:00:00Z'),
				expectedDeparture: new Date('2024-07-01T12:00:00Z'),
				isCargoHazardous: true,
				crewDetails,
				vessel,
				submitter: representative
			});

			notification.submit();

			const firstDecision = new NotificationDecision({
				status: NotificationDecisionStatus.Accepted,
				decisionDate: new Date(),
				officerEmail: 'officer@email.com',
				assignedDockCode: 'DCK003',
				isFinal: false
			});

			notification.addDecision(firstDecision);

			const olderDecision = new NotificationDecision({
				status: NotificationDecisionStatus.Accepted,
				decisionDate: new Date(Date.now() - 3600000), // 1 hour earlier
				officerEmail: 'officer@email.com',
				assignedDockCode: 'DCK003',
				isFinal: true
			});

			expect(() => notification.addDecision(olderDecision)).toThrow('A newer decision has already been made.');
		});

		it('should close notification when final decision is added', () => {
			const notification = new VesselVisitNotification({
				notificationId: 'VVN001',
				status: VesselVisitNotificationStatus.InProgress,
				expectedArrival: new Date('2024-07-01T10:00:00Z'),
				expectedDeparture: new Date('2024-07-01T12:00:00Z'),
				isCargoHazardous: false,
				crewDetails,
				vessel,
				submitter: representative
			});

			notification.submit();

			const decision = new NotificationDecision({
				status: NotificationDecisionStatus.Accepted,
				decisionDate: new Date(),
				officerEmail: 'officer@email.com',
				assignedDockCode: 'DCK003',
				isFinal: true
			});

			notification.addDecision(decision);

			expect(notification.status).toBe(VesselVisitNotificationStatus.Decided);
		});

		it('should return to InProgress when rejected decision is added', () => {
			const notification = new VesselVisitNotification({
				notificationId: 'VVN001',
				status: VesselVisitNotificationStatus.InProgress,
				expectedArrival: new Date('2024-07-01T10:00:00Z'),
				expectedDeparture: new Date('2024-07-01T12:00:00Z'),
				isCargoHazardous: false,
				crewDetails,
				vessel,
				submitter: representative
			});

			notification.submit();

			const decision = new NotificationDecision({
				status: NotificationDecisionStatus.Rejected,
				decisionDate: new Date(),
				officerEmail: 'officer@email.com',
				reason: 'Insufficient documentation',
				isFinal: false
			});

			notification.addDecision(decision);

			expect(notification.status).toBe(VesselVisitNotificationStatus.InProgress);
		});
	});

	describe('GetLatestDecision', () => {

		it('should return undefined when no decisions exist', () => {
			const notification = new VesselVisitNotification({
				notificationId: 'VVN001',
				status: VesselVisitNotificationStatus.InProgress,
				expectedArrival: new Date('2024-07-01T10:00:00Z'),
				expectedDeparture: new Date('2024-07-01T12:00:00Z'),
				isCargoHazardous: false,
				vessel,
				submitter: representative
			});

			expect(notification.getLatestDecision()).toBeUndefined();
		});
	});

	describe('ToDto', () => {
		it('should return correct DTO', () => {
			const notification = new VesselVisitNotification({
				notificationId: 'VVN001',
				status: VesselVisitNotificationStatus.InProgress,
				expectedArrival: new Date('2024-07-01T10:00:00Z'),
				expectedDeparture: new Date('2024-07-01T12:00:00Z'),
				isCargoHazardous: false,
				specialRequirements: 'Test requirements',
				crewDetails,
				loadCargoManifest,
				unloadCargoManifest,
				vessel,
				submitter: representative
			});

			const dto = notification.toDto();

			expect(dto.notificationId).toBe('VVN001');
			expect(dto.expectedArrival).toBe(notification.expectedArrival.toISOString());
			expect(dto.expectedDeparture).toBe(notification.expectedDeparture.toISOString());
			expect(dto.isCargoHazardous).toBe(false);
			expect(dto.specialRequirements).toBe('Test requirements');
			expect(dto.vesselImoNumber).toBe(vessel.imoNumber);
		});
	});
});

describe('NotificationDecision', () => {
	it('should create decision successfully', () => {
		const decision = new NotificationDecision({
			status: NotificationDecisionStatus.Accepted,
			decisionDate: new Date('2024-07-01T10:00:00Z'),
			officerEmail: 'officer@email.com',
			assignedDockCode: 'DCK001',
			reason: 'All requirements met',
			isFinal: true
		});

		expect(decision.status).toBe(NotificationDecisionStatus.Accepted);
		expect(decision.officerEmail).toBe('officer@email.com');
		expect(decision.assignedDockCode).toBe('DCK001');
		expect(decision.isFinal).toBe(true);
	});

	it('should return correct DTO', () => {
		const decision = new NotificationDecision({
			status: NotificationDecisionStatus.Rejected,
			decisionDate: new Date('2024-07-01T10:00:00Z'),
			officerEmail: 'officer@email.com',
			reason: 'Missing documents',
			isFinal: false
		});

		const dto = decision.toDto();

		expect(dto.status).toBe(NotificationDecisionStatus.Rejected);
		expect(dto.officerID).toBe('officer@email.com');
		expect(dto.reason).toBe('Missing documents');
		expect(dto.isFinal).toBe(false);
	});
});
