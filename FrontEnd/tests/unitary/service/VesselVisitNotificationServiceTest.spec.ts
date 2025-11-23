import { describe, it, expect, vi, beforeEach } from 'vitest';
import { VesselVisitNotificationService } from '@/service/VesselVisitNotificationService';
import type { IHttpService, Response } from '@/service/IService/IHttpService';
import type { VesselVisitNotification, NotificationDecision } from '@/model/VesselVisitNotification';
import type { 
  VesselVisitNotificationDto, 
  NotificationDecisionDto,
  VesselVisitDistributionDto 
} from '@/model/dto/VesselVisitNotificationDto';
import { VesselVisitNotificationStatus, NotificationDecisionStatus } from '@/model/dto/VesselVisitNotificationDto';
import type { Page } from '@/model/Page';

describe('VesselVisitNotificationService', () => {
  let service: VesselVisitNotificationService;
  let mockHttpService: IHttpService;

  const mockNotificationDto: VesselVisitNotificationDto = {
    notificationId: 'NOT001',
    expectedArrival: '2025-12-01T10:00:00Z',
    expectedDeparture: '2025-12-05T14:00:00Z',
    isCargoHazardous: false,
    specialRequirements: 'None',
    crewDetails: {
      captain: { value: 'John Smith' },
      totalCrewMembers: 20,
      safetyOfficers: [
        { name: 'Jane Doe', citizenID: '123456', nationality: 'USA' }
      ]
    },
    loadCargoManifest: [],
    unloadCargoManifest: [],
    vesselImoNumber: 'IMO1234567',
  };

  const mockNotificationWithToDto: VesselVisitNotification = {
    notificationId: 'NOT001',
    status: VesselVisitNotificationStatus.InProgress,
    expectedArrival: new Date('2025-12-01T10:00:00Z'),
    expectedDeparture: new Date('2025-12-05T14:00:00Z'),
    isCargoHazardous: false,
    specialRequirements: 'None',
    vessel: { imoNumber: 'IMO1234567' },
    submitter: { email: 'submitter@example.com' },
    toDto: () => mockNotificationDto,
  } as VesselVisitNotification;

  const mockNotification: VesselVisitNotification = {
    notificationId: 'NOT001',
    status: VesselVisitNotificationStatus.InProgress,
    expectedArrival: new Date('2025-12-01T10:00:00Z'),
    expectedDeparture: new Date('2025-12-05T14:00:00Z'),
    isCargoHazardous: false,
    vessel: { imoNumber: 'IMO1234567' },
    submitter: { email: 'submitter@example.com' },
  } as VesselVisitNotification;

  const mockDecisionDto: NotificationDecisionDto = {
    status: NotificationDecisionStatus.Accepted,
    reason: 'Approved',
    decisionDate: new Date('2025-11-24T10:00:00Z'),
    officerEmail: 'officer@example.com',
    assignedDockCode: 'DOCK01',
    isFinal: true,
  };

  const mockDecision: NotificationDecision = {
    status: NotificationDecisionStatus.Accepted,
    reason: 'Approved',
    decisionDate: new Date('2025-11-24T10:00:00Z'),
    officerEmail: 'officer@example.com',
    assignedDockCode: 'DOCK01',
    isFinal: true,
  } as NotificationDecision;

  const mockPage: Page<VesselVisitNotification> = {
    items: [mockNotification],
    pageNumber: 1,
    pageSize: 10,
    pageCount: 1,
  };

  const mockDistribution: VesselVisitDistributionDto = {
    pending: 5,
    accepted: 10,
    rejected: 2,
  };

  beforeEach(() => {
    mockHttpService = {
      get: vi.fn(),
      getWithoutCredentials: vi.fn(),
      post: vi.fn(),
      put: vi.fn(),
      patch: vi.fn(),
      delete: vi.fn(),
    };

    service = new VesselVisitNotificationService(mockHttpService);
  });

  describe('count', () => {
    it('should return distribution of vessel visit notifications', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockDistribution,
      } as Response<VesselVisitDistributionDto>);

      const result = await service.count();

      expect(mockHttpService.get).toHaveBeenCalledWith('/VesselVisitNotification/distribution');
      expect(result).toEqual(mockDistribution);
    });

    it('should throw error when count fails', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Server error'));

      await expect(service.count()).rejects.toThrow('Server error');
    });
  });

  describe('getVesselVisitNotifications', () => {
    it('should return paginated notifications with filter', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockPage,
      } as Response<Page<VesselVisitNotification>>);

      const result = await service.getVesselVisitNotifications({
        filter: {},
        pageNumber: 1,
        pageSize: 10,
      });

      expect(mockHttpService.get).toHaveBeenCalledWith('/VesselVisitNotification/filterPa?PageNumber=1&PageSize=10');
      expect(result).toEqual(mockPage);
    });

    it('should return notifications without filter', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockPage,
      } as Response<Page<VesselVisitNotification>>);

      const result = await service.getVesselVisitNotifications({
        filter: {},
      } as any);

      expect(mockHttpService.get).toHaveBeenCalledWith('/VesselVisitNotification/filterPa?');
      expect(result).toEqual(mockPage);
    });

    it('should throw error when get fails', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Network error'));

      await expect(service.getVesselVisitNotifications({ filter: {} } as any)).rejects.toThrow('Network error');
    });
  });

  describe('getVesselVisitNotificationsForReview', () => {
    it('should return pending notifications for review', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockPage,
      } as Response<Page<VesselVisitNotification>>);

      const result = await service.getVesselVisitNotificationsForReview({
        filter: null,
        pageNumber: 1,
        pageSize: 10,
      });

      expect(mockHttpService.get).toHaveBeenCalledWith('/VesselVisitNotification/filterPa?OnlyPending=true');
      expect(result).toEqual(mockPage);
    });

    it('should throw error when get for review fails', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Error'));

      await expect(service.getVesselVisitNotificationsForReview({ filter: null } as any)).rejects.toThrow('Error');
    });
  });

  describe('getVesselVisitNotificationsByRepresentative', () => {
    it('should return notifications by representative with all filters', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockPage,
      } as Response<Page<VesselVisitNotification>>);

      const result = await service.getVesselVisitNotificationsByRepresentative({
        filter: {
          Status: 1,
          WithReason: true,
          WithDockAssigned: false,
          Vessel: 'TestVessel',
          ExpectedArrivalFrom: new Date('2025-12-01'),
          ExpectedArrivalTo: new Date('2025-12-31'),
        },
        pageNumber: 1,
        pageSize: 20,
      });

      expect(mockHttpService.get).toHaveBeenCalled();
      expect(result).toEqual(mockPage);
    });

    it('should return notifications without filters', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockPage,
      } as Response<Page<VesselVisitNotification>>);

      const result = await service.getVesselVisitNotificationsByRepresentative();

      expect(mockHttpService.get).toHaveBeenCalledWith('/VesselVisitNotification/filter');
      expect(result).toEqual(mockPage);
    });

    it('should return empty page when get fails', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Error'));

      const result = await service.getVesselVisitNotificationsByRepresentative();

      expect(result).toEqual({ items: [], pageNumber: 1, pageSize: 0, pageCount: 0 });
    });
  });

  describe('getVesselVisitNotificationById', () => {
    it('should return notification by ID', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockNotification,
      } as Response<VesselVisitNotification>);

      const result = await service.getVesselVisitNotificationById('NOT001');

      expect(mockHttpService.get).toHaveBeenCalledWith('/VesselVisitNotification/NOT001');
      expect(result).toEqual(mockNotification);
    });

    it('should throw error when notification not found', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Not Found'));

      await expect(service.getVesselVisitNotificationById('INVALID')).rejects.toThrow('Not Found');
    });
  });

  describe('getNotificationDecisions', () => {
    it('should return decisions for a notification', async () => {
      const decisions = [mockDecision];
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: decisions,
      } as Response<NotificationDecision[]>);

      const result = await service.getNotificationDecisions('NOT001');

      expect(mockHttpService.get).toHaveBeenCalledWith('/VesselVisitNotification/decisions?vesselVisitNotificationId=NOT001');
      expect(result).toEqual(decisions);
    });

    it('should throw error when get decisions fails', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Error'));

      await expect(service.getNotificationDecisions('NOT001')).rejects.toThrow('Error');
    });
  });

  describe('createVesselVisitNotification', () => {
    it('should create notification with valid data', async () => {
      vi.mocked(mockHttpService.post).mockResolvedValue({
        status: 201,
        statusText: 'Created',
        data: mockNotification,
      } as Response<VesselVisitNotification>);

      const result = await service.createVesselVisitNotification(mockNotificationWithToDto);

      expect(mockHttpService.post).toHaveBeenCalledWith('/VesselVisitNotification', mockNotificationDto);
      expect(result).toEqual(mockNotification);
    });

    it('should throw error when notification already exists', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Conflict: Notification already exists'));

      await expect(service.createVesselVisitNotification(mockNotificationWithToDto)).rejects.toThrow('Conflict: Notification already exists');
    });

    it('should throw error when validation fails', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Bad Request: Invalid data'));

      await expect(service.createVesselVisitNotification(mockNotificationWithToDto)).rejects.toThrow('Bad Request: Invalid data');
    });

    it('should throw error on internal server error', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Internal Server Error'));

      await expect(service.createVesselVisitNotification(mockNotificationWithToDto)).rejects.toThrow('Internal Server Error');
    });
  });

  describe('createNotificationDecision', () => {
    it('should create decision for notification', async () => {
      vi.mocked(mockHttpService.post).mockResolvedValue({
        status: 201,
        statusText: 'Created',
        data: mockDecision,
      } as Response<NotificationDecision>);

      const result = await service.createNotificationDecision('NOT001', mockDecisionDto);

      expect(mockHttpService.post).toHaveBeenCalledWith(
        '/VesselVisitNotification/decisions?vesselVisitNotificationId=NOT001',
        mockDecisionDto
      );
      expect(result).toEqual(mockDecision);
    });

    it('should throw error when decision creation fails', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Bad Request'));

      await expect(service.createNotificationDecision('NOT001', mockDecisionDto)).rejects.toThrow('Bad Request');
    });
  });

  describe('updateVesselVisitNotification', () => {
    it('should update notification with valid data', async () => {
      vi.mocked(mockHttpService.put).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockNotification,
      } as Response<VesselVisitNotification>);

      const result = await service.updateVesselVisitNotification(mockNotificationWithToDto);

      expect(mockHttpService.put).toHaveBeenCalledWith(
        `/VesselVisitNotification/${mockNotificationDto.notificationId}`,
        mockNotificationDto
      );
      expect(result).toEqual(mockNotification);
    });

    it('should throw error when notification not found during update', async () => {
      vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Not Found'));

      await expect(service.updateVesselVisitNotification(mockNotificationWithToDto)).rejects.toThrow('Not Found');
    });

    it('should throw error on validation error during update', async () => {
      vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Bad Request: Invalid update data'));

      await expect(service.updateVesselVisitNotification(mockNotificationWithToDto)).rejects.toThrow('Bad Request: Invalid update data');
    });

    it('should throw error on internal server error during update', async () => {
      vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Internal Server Error'));

      await expect(service.updateVesselVisitNotification(mockNotificationWithToDto)).rejects.toThrow('Internal Server Error');
    });
  });

  describe('submitVesselVisitNotification', () => {
    it('should submit notification successfully', async () => {
      vi.mocked(mockHttpService.put).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: undefined,
      } as Response<void>);

      await service.submitVesselVisitNotification('NOT001');

      expect(mockHttpService.put).toHaveBeenCalledWith('/VesselVisitNotification/submit/NOT001', {});
    });

    it('should throw error when submit fails', async () => {
      vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Cannot submit'));

      await expect(service.submitVesselVisitNotification('NOT001')).rejects.toThrow('Cannot submit');
    });
  });

  describe('deleteDraft', () => {
    it('should delete draft notification successfully', async () => {
      vi.mocked(mockHttpService.delete).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: undefined,
      } as Response<void>);

      await service.deleteDraft('NOT001');

      expect(mockHttpService.delete).toHaveBeenCalledWith('/VesselVisitNotification?id=NOT001');
    });

    it('should throw error when delete fails', async () => {
      vi.mocked(mockHttpService.delete).mockRejectedValue(new Error('Not Found'));

      await expect(service.deleteDraft('NOT001')).rejects.toThrow('Not Found');
    });

    it('should throw error when trying to delete non-draft notification', async () => {
      vi.mocked(mockHttpService.delete).mockRejectedValue(new Error('Cannot delete submitted notification'));

      await expect(service.deleteDraft('NOT001')).rejects.toThrow('Cannot delete submitted notification');
    });
  });
});
