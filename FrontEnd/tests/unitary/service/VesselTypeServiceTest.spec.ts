import { describe, it, expect, vi, beforeEach } from 'vitest';
import { VesselTypeService } from '@/service/VesselTypeService';
import type { IHttpService, Response } from '@/service/IService/IHttpService';
import type { VesselType } from '@/model/VesselType';
import type { VesselTypeDto } from '@/model/dto/VesselTypeDto';
import type { Page } from '@/model/Page';

describe('VesselTypeService', () => {
  let vesselTypeService: VesselTypeService;
  let mockHttpService: IHttpService;

  const mockVesselTypeDto: VesselTypeDto = {
    name: 'Container Ship',
    description: 'Large container cargo vessel',
    maxNumberOfRows: 10,
    maxNumberOfBays: 20,
    maxNumberOfTiers: 8,
    capacity: 1600,
    physicalCharacteristics: { length: 350, depth: 60, draft: 25 },
  };

  const mockVesselTypeWithToDto: VesselType = {
    name: 'Container Ship',
    description: 'Large container cargo vessel',
    maxNumberOfRows: 10,
    maxNumberOfBays: 20,
    maxNumberOfTiers: 8,
    capacity: 1600,
    physicalCharacteristics: { length: 350, depth: 60, draft: 25 },
    toDto: () => mockVesselTypeDto,
  } as VesselType;

  const mockVesselType: VesselType = {
    name: 'Container Ship',
    description: 'Large container cargo vessel',
    maxNumberOfRows: 10,
    maxNumberOfBays: 20,
    maxNumberOfTiers: 8,
    capacity: 1600,
    physicalCharacteristics: { length: 350, depth: 60, draft: 25 },
  } as VesselType;

  const mockPage: Page<VesselType> = {
    items: [mockVesselType],
    pageNumber: 1,
    pageSize: 10,
    pageCount: 1,
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

    vesselTypeService = new VesselTypeService(mockHttpService);
  });

  describe('getVesselTypes', () => {
    it('should return OK with list of vessel types when no filter is provided', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockPage,
      } as Response<Page<VesselType>>);

      const result = await vesselTypeService.getVesselTypes();

      expect(mockHttpService.get).toHaveBeenCalledWith('/VesselType/filter');
      expect(result).toEqual(mockPage);
    });

    it('should return OK with filtered list of vessel types when filter is provided', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockPage,
      } as Response<Page<VesselType>>);

      const result = await vesselTypeService.getVesselTypes({
        filter: { name: 'Container' },
        pageNumber: 1,
        pageSize: 10,
      });

      expect(mockHttpService.get).toHaveBeenCalledWith('/VesselType/filter?Name=Container&PageNumber=1&PageSize=10');
      expect(result).toEqual(mockPage);
    });

    it('should throw error when getVesselTypes fails', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Network error'));

      await expect(vesselTypeService.getVesselTypes()).rejects.toThrow('Network error');
    });
  });

  describe('getVesselTypeByName', () => {
    it('should return vessel type when found', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockVesselType,
      } as Response<VesselType>);

      const result = await vesselTypeService.getVesselTypeByName('Container Ship');

      expect(mockHttpService.get).toHaveBeenCalledWith('/VesselType/Container Ship');
      expect(result).toEqual(mockVesselType);
    });

    it('should throw error when vessel type is not found', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Not Found'));

      await expect(vesselTypeService.getVesselTypeByName('INVALID')).rejects.toThrow('Not Found');
    });
  });

  describe('createVesselType', () => {
    it('should return created vessel type with valid data', async () => {
      vi.mocked(mockHttpService.post).mockResolvedValue({
        status: 201,
        statusText: 'Created',
        data: mockVesselType,
      } as Response<VesselType>);

      const result = await vesselTypeService.createVesselType(mockVesselTypeWithToDto);

      expect(mockHttpService.post).toHaveBeenCalledWith('/VesselType', mockVesselTypeDto);
      expect(result).toEqual(mockVesselType);
    });

    it('should throw error when vessel type already exists (Conflict)', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Conflict: Vessel type already exists'));

      await expect(vesselTypeService.createVesselType(mockVesselTypeWithToDto)).rejects.toThrow('Conflict: Vessel type already exists');
    });

    it('should throw error when validation fails (Bad Request)', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Bad Request: Invalid data'));

      await expect(vesselTypeService.createVesselType(mockVesselTypeWithToDto)).rejects.toThrow('Bad Request: Invalid data');
    });

    it('should throw error when entity not found during creation', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Not Found: Related entity missing'));

      await expect(vesselTypeService.createVesselType(mockVesselTypeWithToDto)).rejects.toThrow('Not Found: Related entity missing');
    });

    it('should throw error on internal server error', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Internal Server Error'));

      await expect(vesselTypeService.createVesselType(mockVesselTypeWithToDto)).rejects.toThrow('Internal Server Error');
    });
  });

  describe('updateVesselType', () => {
    it('should return updated vessel type with valid data', async () => {
      vi.mocked(mockHttpService.put).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockVesselType,
      } as Response<VesselType>);

      const result = await vesselTypeService.updateVesselType(mockVesselTypeWithToDto);

      expect(mockHttpService.put).toHaveBeenCalledWith(`/VesselType/${mockVesselTypeDto.name}`, mockVesselTypeDto);
      expect(result).toEqual(mockVesselType);
    });

    it('should throw error when vessel type not found during update', async () => {
      vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Not Found'));

      await expect(vesselTypeService.updateVesselType(mockVesselTypeWithToDto)).rejects.toThrow('Not Found');
    });

    it('should throw error on validation error during update', async () => {
      vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Bad Request: Invalid update data'));

      await expect(vesselTypeService.updateVesselType(mockVesselTypeWithToDto)).rejects.toThrow('Bad Request: Invalid update data');
    });

    it('should throw error on internal server error during update', async () => {
      vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Internal Server Error'));

      await expect(vesselTypeService.updateVesselType(mockVesselTypeWithToDto)).rejects.toThrow('Internal Server Error');
    });
  });

  describe('getNumberOfVesselTypes', () => {
    it('should return the total number of vessel types', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: 8,
      } as Response<number>);

      const result = await vesselTypeService.getNumberOfVesselTypes();

      expect(mockHttpService.get).toHaveBeenCalledWith('/VesselType/count');
      expect(result).toBe(8);
    });

    it('should throw error when count request fails', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Server Error'));

      await expect(vesselTypeService.getNumberOfVesselTypes()).rejects.toThrow('Server Error');
    });
  });
});
