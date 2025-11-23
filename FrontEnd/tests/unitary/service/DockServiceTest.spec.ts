import { describe, it, expect, vi, beforeEach } from 'vitest';
import { DockService } from '@/service/DockService';
import type { IHttpService, Response } from '@/service/IService/IHttpService';
import type { Dock } from '@/model/Dock';
import type { DockDto } from '@/model/dto/DockDto';
import type { Page } from '@/model/Page';

describe('DockService', () => {
  let dockService: DockService;
  let mockHttpService: IHttpService;

  const mockDockDto: DockDto = {
    code: 'DOCK01',
    name: 'Main Dock',
    location: 'North Harbor',
    physicalCharacteristics: { length: 300, depth: 50, draft: 20 },
    supportedVesselTypes: ['Container Ship'],
  };

  const mockDockWithToDto: Dock = {
    code: 'DOCK01',
    name: 'Main Dock',
    location: 'North Harbor',
    physicalCharacteristics: { length: 300, depth: 50, draft: 20 },
    supportedVesselTypes: [{ name: 'Container Ship' }],
    toDto: () => mockDockDto,
  } as Dock;

  const mockDock: Dock = {
    code: 'DOCK01',
    name: 'Main Dock',
    location: 'North Harbor',
    physicalCharacteristics: { length: 300, depth: 50, draft: 20 },
    supportedVesselTypes: [{ name: 'Container Ship' }],
  } as Dock;

  const mockPage: Page<Dock> = {
    items: [mockDock],
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

    dockService = new DockService(mockHttpService);
  });

  describe('getDocks', () => {
    it('should return OK with list of docks when no filter is provided', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockPage,
      } as Response<Page<Dock>>);

      const result = await dockService.getDocks();

      expect(mockHttpService.get).toHaveBeenCalledWith('/Dock/filter');
      expect(result).toEqual(mockPage);
    });

    it('should return OK with filtered list of docks when filter is provided', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockPage,
      } as Response<Page<Dock>>);

      const result = await dockService.getDocks({
        filter: { dockName: 'Main' },
        pageNumber: 1,
        pageSize: 10,
      });

      expect(mockHttpService.get).toHaveBeenCalledWith('/Dock/filter?DockName=Main&PageNumber=1&PageSize=10');
      expect(result).toEqual(mockPage);
    });

    it('should throw error when getDocks fails', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Network error'));

      await expect(dockService.getDocks()).rejects.toThrow('Network error');
    });
  });

  describe('getDockByCode', () => {
    it('should return dock when found', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockDock,
      } as Response<Dock>);

      const result = await dockService.getDockByCode('DOCK01');

      expect(mockHttpService.get).toHaveBeenCalledWith('/Dock/DOCK01');
      expect(result).toEqual(mockDock);
    });

    it('should throw error when dock is not found', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Not Found'));

      await expect(dockService.getDockByCode('INVALID')).rejects.toThrow('Not Found');
    });
  });

  describe('createDock', () => {
    it('should return created dock with valid data', async () => {
      vi.mocked(mockHttpService.post).mockResolvedValue({
        status: 201,
        statusText: 'Created',
        data: mockDock,
      } as Response<Dock>);

      const result = await dockService.createDock(mockDockWithToDto);

      expect(mockHttpService.post).toHaveBeenCalledWith('/Dock', mockDockDto);
      expect(result).toEqual(mockDock);
    });

    it('should throw error when dock already exists (Conflict)', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Conflict: Dock already exists'));

      await expect(dockService.createDock(mockDockWithToDto)).rejects.toThrow('Conflict: Dock already exists');
    });

    it('should throw error when validation fails (Bad Request)', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Bad Request: Invalid data'));

      await expect(dockService.createDock(mockDockWithToDto)).rejects.toThrow('Bad Request: Invalid data');
    });

    it('should throw error when entity not found during creation', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Not Found: Related entity missing'));

      await expect(dockService.createDock(mockDockWithToDto)).rejects.toThrow('Not Found: Related entity missing');
    });

    it('should throw error on internal server error', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Internal Server Error'));

      await expect(dockService.createDock(mockDockWithToDto)).rejects.toThrow('Internal Server Error');
    });
  });

  describe('updateDock', () => {
    it('should return updated dock with valid data', async () => {
      vi.mocked(mockHttpService.put).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockDock,
      } as Response<Dock>);

      const result = await dockService.updateDock(mockDockWithToDto);

      expect(mockHttpService.put).toHaveBeenCalledWith(`/Dock/${mockDockDto.code}`, mockDockDto);
      expect(result).toEqual(mockDock);
    });

    it('should throw error when dock not found during update', async () => {
      vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Not Found'));

      await expect(dockService.updateDock(mockDockWithToDto)).rejects.toThrow('Not Found');
    });

    it('should throw error on validation error during update', async () => {
      vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Bad Request: Invalid update data'));

      await expect(dockService.updateDock(mockDockWithToDto)).rejects.toThrow('Bad Request: Invalid update data');
    });

    it('should throw error on internal server error during update', async () => {
      vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Internal Server Error'));

      await expect(dockService.updateDock(mockDockWithToDto)).rejects.toThrow('Internal Server Error');
    });
  });

  describe('getNumberOfDocks', () => {
    it('should return the total number of docks', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: 15,
      } as Response<number>);

      const result = await dockService.getNumberOfDocks();

      expect(mockHttpService.get).toHaveBeenCalledWith('/Dock/count');
      expect(result).toBe(15);
    });

    it('should throw error when count request fails', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Server Error'));

      await expect(dockService.getNumberOfDocks()).rejects.toThrow('Server Error');
    });
  });
});
