import { describe, it, expect, vi, beforeEach } from 'vitest';
import { VesselService } from '@/service/VesselService';
import type { IHttpService, Response } from '@/service/IService/IHttpService';
import type { Vessel } from '@/model/Vessel';
import type { VesselCreateDto } from '@/model/dto/VesselDto';
import type { Page } from '@/model/Page';

describe('VesselService', () => {
  let vesselService: VesselService;
  let mockHttpService: IHttpService;

  const mockVesselDto: VesselCreateDto = {
    name: 'Test Vessel',
    imoNumber: 'IMO1234567',
    type: 'Container Ship',
    owner: '123456789',
    length: 200.0,
    depth: 32.5,
    draft: 12.5,
  };

  const mockVesselWithToDto: Vessel = {
    name: 'Test Vessel',
    imoNumber: 'IMO1234567',
    type: { name: 'Container Ship' },
    owner: { taxNumber: '123456789' },
    physicalCharacteristics: { length: 200.0, depth: 32.5, draft: 12.5 },
    toDto: () => mockVesselDto,
  } as Vessel;

  const mockVessel: Vessel = {
    name: 'Test Vessel',
    imoNumber: 'IMO1234567',
    type: { name: 'Container Ship' },
    owner: { taxNumber: '123456789' },
    physicalCharacteristics: { length: 200.0, depth: 32.5, draft: 12.5 },
  } as Vessel;

  const mockPage: Page<Vessel> = {
    items: [mockVessel],
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

    vesselService = new VesselService(mockHttpService);
  });

  describe('getVessels', () => {
    it('should return OK with list of vessels when no filter is provided', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockPage,
      } as Response<Page<Vessel>>);

      const result = await vesselService.getVessels();

      expect(mockHttpService.get).toHaveBeenCalledWith('/Vessel/filter');
      expect(result).toEqual(mockPage);
    });

    it('should return OK with filtered list of vessels when filter is provided', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockPage,
      } as Response<Page<Vessel>>);

      const result = await vesselService.getVessels({
        filter: { name: 'Test' },
        pageNumber: 1,
        pageSize: 10,
      });

      expect(mockHttpService.get).toHaveBeenCalledWith('/Vessel/filter?Name=Test&PageNumber=1&PageSize=10');
      expect(result).toEqual(mockPage);
    });

    it('should throw error when getVessels fails', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Network error'));

      await expect(vesselService.getVessels()).rejects.toThrow('Network error');
    });
  });

  describe('getVesselByIMO', () => {
    it('should return vessel when found', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockVessel,
      } as Response<Vessel>);

      const result = await vesselService.getVesselByIMO('IMO1234567');

      expect(mockHttpService.get).toHaveBeenCalledWith('/Vessel/IMO1234567');
      expect(result).toEqual(mockVessel);
    });

    it('should throw error when vessel is not found', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Not Found'));

      await expect(vesselService.getVesselByIMO('INVALID')).rejects.toThrow('Not Found');
    });
  });

  describe('getVesselByOwner', () => {
    it('should return list of vessels for owner', async () => {
      const vessels = [mockVessel];
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: vessels,
      } as Response<Vessel[]>);

      const result = await vesselService.getVesselByOwner('owner@example.com');

      expect(mockHttpService.get).toHaveBeenCalledWith('/Vessel/owner/owner@example.com');
      expect(result).toEqual(vessels);
    });

    it('should throw error when get by owner fails', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Error'));

      await expect(vesselService.getVesselByOwner('owner@example.com')).rejects.toThrow('Error');
    });
  });

  describe('createVessel', () => {
    it('should return created vessel with valid data', async () => {
      vi.mocked(mockHttpService.post).mockResolvedValue({
        status: 201,
        statusText: 'Created',
        data: mockVessel,
      } as Response<Vessel>);

      const result = await vesselService.createVessel(mockVesselWithToDto);

      expect(mockHttpService.post).toHaveBeenCalledWith('/Vessel', mockVesselDto);
      expect(result).toEqual(mockVessel);
    });

    it('should throw error when vessel already exists (Conflict)', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Conflict: Vessel already exists'));

      await expect(vesselService.createVessel(mockVesselWithToDto)).rejects.toThrow('Conflict: Vessel already exists');
    });

    it('should throw error when validation fails (Bad Request)', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Bad Request: Invalid data'));

      await expect(vesselService.createVessel(mockVesselWithToDto)).rejects.toThrow('Bad Request: Invalid data');
    });

    it('should throw error when entity not found during creation', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Not Found: Related entity missing'));

      await expect(vesselService.createVessel(mockVesselWithToDto)).rejects.toThrow('Not Found: Related entity missing');
    });

    it('should throw error on internal server error', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Internal Server Error'));

      await expect(vesselService.createVessel(mockVesselWithToDto)).rejects.toThrow('Internal Server Error');
    });
  });

  describe('updateVessel', () => {
    it('should return updated vessel with valid data', async () => {
      vi.mocked(mockHttpService.put).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockVessel,
      } as Response<Vessel>);

      const result = await vesselService.updateVessel(mockVesselWithToDto);

      expect(mockHttpService.put).toHaveBeenCalledWith(`/Vessel/${mockVesselDto.imoNumber}`, mockVesselDto);
      expect(result).toEqual(mockVessel);
    });

    it('should throw error when vessel not found during update', async () => {
      vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Not Found'));

      await expect(vesselService.updateVessel(mockVesselWithToDto)).rejects.toThrow('Not Found');
    });

    it('should throw error on validation error during update', async () => {
      vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Bad Request: Invalid update data'));

      await expect(vesselService.updateVessel(mockVesselWithToDto)).rejects.toThrow('Bad Request: Invalid update data');
    });

    it('should throw error on internal server error during update', async () => {
      vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Internal Server Error'));

      await expect(vesselService.updateVessel(mockVesselWithToDto)).rejects.toThrow('Internal Server Error');
    });
  });

  describe('getNumberOfVessels', () => {
    it('should return the total number of vessels', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: 42,
      } as Response<number>);

      const result = await vesselService.getNumberOfVessels();

      expect(mockHttpService.get).toHaveBeenCalledWith('/Vessel/count');
      expect(result).toBe(42);
    });

    it('should throw error when count request fails', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Server Error'));

      await expect(vesselService.getNumberOfVessels()).rejects.toThrow('Server Error');
    });
  });
});
