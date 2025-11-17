import { describe, it, expect, vi, beforeEach } from 'vitest';
import { PhysicalResourceService } from '@/service/PhysicalResourceService';
import type { IHttpService, Response } from '@/service/IService/IHttpService';

import type {
  STSCraneDto,
  YardCraneDto,
  TruckDto,
} from '@/model/dto/PhysicalResourceDto';

import type {
  PhysicalResource,
  STSCrane,
  YardCrane,
  Truck,
  PhysicalResourceFilter,
} from '@/model/PhysicalResource';

import type { Page, Filter } from '@/model/Page';

describe('PhysicalResourceService', () => {
  let service: PhysicalResourceService;
  let mockHttp: IHttpService;

  const mockPage: Page<any> = {
    items: [{ code: 'RC001' }],
    pageNumber: 1,
    pageSize: 10,
    pageCount: 1,
  };

  const mockSTS: STSCrane = {
    code: 'STS001',
  } as STSCrane;

  const mockYard: YardCrane = {
    code: 'YRD001',
  } as YardCrane;

  const mockTruck: Truck = {
    code: 'TRK001',
  } as Truck;

  const mockSTSDto: STSCraneDto = {
    code: 'STS001',
    description: 'STS Crane 1',
    status: 1,
    setupTimeInMinutes: 30,
    operationalWindow: { shifts: [] },
    qualificationsCodes: [],
    liftingCapacity: 100,
    servingDockCode: 'D1',
    containersPerHour: 20,
  };

  const mockYardDto: YardCraneDto = {
    code: 'YRD001',
    description: 'Yard Crane 1',
    status: 1,
    setupTimeInMinutes: 20,
    operationalWindow: { shifts: [] },
    qualificationsCodes: [],
    liftingCapacity: 50,
    containersPerHour: 10,
  };

  const mockTruckDto: TruckDto = {
    code: 'TRK001',
    description: 'Truck A',
    status: 1,
    setupTimeInMinutes: 5,
    operationalWindow: { shifts: [] },
    qualificationsCodes: [],
    maxLoadCapacity: 200,
    averageSpeed: 60,
    containersPerTrip: 2,
  };

  beforeEach(() => {
    mockHttp = {
      get: vi.fn(),
      post: vi.fn(),
      put: vi.fn(),
      delete: vi.fn(),
      patch: vi.fn(),
      getWithoutCredentials: vi.fn(),
    };
    service = new PhysicalResourceService(mockHttp);
  });

  // ------------------------------------------------------------
  // getPhysicalResources
  // ------------------------------------------------------------
  describe('getPhysicalResources', () => {
    it('should return OK with list when no filter is provided', async () => {
      vi.mocked(mockHttp.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockPage
      } as Response<Page<any>>);

      const result = await service.getPhysicalResources();

      expect(mockHttp.get).toHaveBeenCalledWith('/PhysicalResource/filter');
      expect(result).toEqual(mockPage);
    });

    it('should return filtered list when filter is provided', async () => {
      const filter: Filter<PhysicalResourceFilter> = {
        filter: {
          Code: 'STS001',
          Description: 'Crane',
          Status: 1,
          Type: 0
        },
        pageNumber: 1,
        pageSize: 10
      };

      vi.mocked(mockHttp.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockPage
      });

      const result = await service.getPhysicalResources(filter);

      expect(mockHttp.get).toHaveBeenCalledWith(
        '/PhysicalResource/filter?Code=STS001&Description=Crane&Status=1&Type=0&PageNumber=1&PageSize=10'
      );
      expect(result).toEqual(mockPage);
    });

    it('should throw on failure', async () => {
      vi.mocked(mockHttp.get).mockRejectedValue(new Error('Failed request'));

      await expect(service.getPhysicalResources()).rejects.toThrow('Failed request');
    });
  });

  // ------------------------------------------------------------
  // getPhysicalResourceById
  // ------------------------------------------------------------
  describe('getPhysicalResourceById', () => {
    it('should return the resource', async () => {
      vi.mocked(mockHttp.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockSTS
      });

      const result = await service.getPhysicalResourceById('STS001');

      expect(mockHttp.get).toHaveBeenCalledWith('/PhysicalResource/STS001');
      expect(result).toEqual(mockSTS);
    });

    it('should throw if not found', async () => {
      vi.mocked(mockHttp.get).mockRejectedValue(new Error('Not found'));

      await expect(service.getPhysicalResourceById('XXX')).rejects.toThrow('Not found');
    });
  });

  // ------------------------------------------------------------
  // deactivatePhysicalResource
  // ------------------------------------------------------------
  describe('deactivatePhysicalResource', () => {
    it('should deactivate successfully', async () => {
      vi.mocked(mockHttp.delete).mockResolvedValue({
        status: 204,
        statusText: 'No Content',
        data: undefined
      });

      await service.deactivatePhysicalResource('STS001');

      expect(mockHttp.delete).toHaveBeenCalledWith('/PhysicalResource/STS001');
    });

    it('should throw on error', async () => {
      vi.mocked(mockHttp.delete).mockRejectedValue(new Error('Failed to delete'));

      await expect(service.deactivatePhysicalResource('STS001'))
        .rejects.toThrow('Failed to delete');
    });
  });

  // ------------------------------------------------------------
  // ADD methods
  // ------------------------------------------------------------
  describe('addSTSCrane', () => {
    it('should add STS crane', async () => {
      vi.mocked(mockHttp.post).mockResolvedValue({
        status: 201,
        statusText: 'Created',
        data: mockSTS
      });

      const result = await service.addSTSCrane(mockSTSDto);

      expect(mockHttp.post).toHaveBeenCalledWith('/PhysicalResource/AddSTSCrane', mockSTSDto);
      expect(result).toEqual(mockSTS);
    });

    it('should throw on error', async () => {
      vi.mocked(mockHttp.post).mockRejectedValue(new Error('Bad request'));

      await expect(service.addSTSCrane(mockSTSDto)).rejects.toThrow('Bad request');
    });
  });

  describe('addYardCrane', () => {
    it('should add yard crane', async () => {
      vi.mocked(mockHttp.post).mockResolvedValue({
        status: 201,
        statusText: 'Created',
        data: mockYard
      });

      const result = await service.addYardCrane(mockYardDto);

      expect(mockHttp.post).toHaveBeenCalledWith('/PhysicalResource/AddYardCrane', mockYardDto);
      expect(result).toEqual(mockYard);
    });

    it('should throw on failure', async () => {
      vi.mocked(mockHttp.post).mockRejectedValue(new Error('Failure'));

      await expect(service.addYardCrane(mockYardDto)).rejects.toThrow('Failure');
    });
  });

  describe('addTruck', () => {
    it('should add truck', async () => {
      vi.mocked(mockHttp.post).mockResolvedValue({
        status: 201,
        statusText: 'Created',
        data: mockTruck
      });

      const result = await service.addTruck(mockTruckDto);

      expect(mockHttp.post).toHaveBeenCalledWith('/PhysicalResource/AddTruck', mockTruckDto);
      expect(result).toEqual(mockTruck);
    });

    it('should throw on error', async () => {
      vi.mocked(mockHttp.post).mockRejectedValue(new Error('Error'));

      await expect(service.addTruck(mockTruckDto)).rejects.toThrow('Error');
    });
  });

  // ------------------------------------------------------------
  // UPDATE methods
  // ------------------------------------------------------------
  describe('updateSTSCrane', () => {
    it('should update STS crane', async () => {
      vi.mocked(mockHttp.put).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockSTS
      });

      const result = await service.updateSTSCrane(mockSTSDto);

      expect(mockHttp.put).toHaveBeenCalledWith(
        `/PhysicalResource/UpdateSTSCrane/${mockSTSDto.code}`,
        mockSTSDto
      );
      expect(result).toEqual(mockSTS);
    });

    it('should throw on error', async () => {
      vi.mocked(mockHttp.put).mockRejectedValue(new Error('Failed'));

      await expect(service.updateSTSCrane(mockSTSDto)).rejects.toThrow('Failed');
    });
  });

  describe('updateYardCrane', () => {
    it('should update yard crane', async () => {
      vi.mocked(mockHttp.put).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockYard
      });

      const result = await service.updateYardCrane(mockYardDto);

      expect(mockHttp.put).toHaveBeenCalledWith(
        `/PhysicalResource/UpdateYardCrane/${mockYardDto.code}`,
        mockYardDto
      );
      expect(result).toEqual(mockYard);
    });

    it('should throw on error', async () => {
      vi.mocked(mockHttp.put).mockRejectedValue(new Error('Err'));

      await expect(service.updateYardCrane(mockYardDto)).rejects.toThrow('Err');
    });
  });

  describe('updateTruck', () => {
    it('should update truck', async () => {
      vi.mocked(mockHttp.put).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockTruck
      });

      const result = await service.updateTruck(mockTruckDto);

      expect(mockHttp.put).toHaveBeenCalledWith(
        `/PhysicalResource/UpdateTruck/${mockTruckDto.code}`,
        mockTruckDto
      );
      expect(result).toEqual(mockTruck);
    });

    it('should throw on failure', async () => {
      vi.mocked(mockHttp.put).mockRejectedValue(new Error('Error updating'));

      await expect(service.updateTruck(mockTruckDto))
        .rejects.toThrow('Error updating');
    });
  });

  // ------------------------------------------------------------
  // getNumberOfPhysicalResources
  // ------------------------------------------------------------
  describe('getNumberOfPhysicalResources', () => {
    it('should return the total count', async () => {
      vi.mocked(mockHttp.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: 99
      });

      const result = await service.getNumberOfPhysicalResources();

      expect(mockHttp.get).toHaveBeenCalledWith('/PhysicalResource/count');
      expect(result).toBe(99);
    });

    it('should throw on error', async () => {
      vi.mocked(mockHttp.get).mockRejectedValue(new Error('Server error'));

      await expect(service.getNumberOfPhysicalResources())
        .rejects.toThrow('Server error');
    });
  });

});
