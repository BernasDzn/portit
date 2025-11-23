import { describe, it, expect, vi, beforeEach } from 'vitest';
import { StorageAreaService } from '@/service/StorageAreaService';
import type { IHttpService, Response } from '@/service/IService/IHttpService';
import type { StorageArea } from '@/model/StorageArea';
import type { StorageAreaDto } from '@/model/dto/StorageAreaDto';
import type { Page } from '@/model/Page';

describe('StorageAreaService', () => {
  let storageAreaService: StorageAreaService;
  let mockHttpService: IHttpService;

  const mockStorageAreaDto: StorageAreaDto = {
    nameCode: 'WH001',
    location: 'A1',
    type: 1,
    capacity: 1000,
    currentOccupancy: 500,
    dockServices: [
      {
        dockCode: 'DOCK01',
        distance: 100,
        isServingDock: true,
      },
    ],
  };

  const mockStorageAreaWithToDto: StorageArea = {
    nameCode: 'WH001',
    location: 'A1',
    type: 1,
    capacity: 1000,
    currentOccupancy: 500,
    dockServices: [],
    toDto: () => mockStorageAreaDto,
  } as StorageArea;

  const mockStorageArea: StorageArea = {
    nameCode: 'WH001',
    location: 'A1',
    type: 1,
    capacity: 1000,
    currentOccupancy: 500,
    dockServices: [],
  } as StorageArea;

  const mockPage: Page<StorageArea> = {
    items: [mockStorageArea],
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

    storageAreaService = new StorageAreaService(mockHttpService);
  });

  describe('getStorageAreas', () => {
    it('should return OK with list of storage areas when no filter is provided', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockPage,
      } as Response<Page<StorageArea>>);

      const result = await storageAreaService.getStorageAreas();

      expect(mockHttpService.get).toHaveBeenCalledWith('/StorageArea/filter');
      expect(result).toEqual(mockPage);
    });

    it('should return OK with filtered list of storage areas when filter is provided', async () => {
      const filter = {
        filter: {
          nameCode: 'WH001',
        },
      };

      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockPage,
      } as Response<Page<StorageArea>>);

      const result = await storageAreaService.getStorageAreas(filter);

      expect(mockHttpService.get).toHaveBeenCalledWith('/StorageArea/filter?NameCode=WH001&');
      expect(result).toEqual(mockPage);
    });

    it('should throw error when getStorageAreas fails', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Network error'));

      await expect(storageAreaService.getStorageAreas()).rejects.toThrow('Network error');
    });
  });

  describe('getStorageAreaById', () => {
    it('should return storage area when found', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockStorageArea,
      } as Response<StorageArea>);

      const result = await storageAreaService.getStorageAreaById('WH001');

      expect(mockHttpService.get).toHaveBeenCalledWith('/StorageArea/WH001');
      expect(result).toEqual(mockStorageArea);
    });

    it('should throw error when storage area is not found', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Not found'));

      await expect(storageAreaService.getStorageAreaById('INVALID')).rejects.toThrow('Not found');
    });
  });

  describe('createStorageArea', () => {
    it('should return created storage area with valid data', async () => {
      vi.mocked(mockHttpService.post).mockResolvedValue({
        status: 201,
        statusText: 'Created',
        data: mockStorageArea,
      } as Response<StorageArea>);

      const result = await storageAreaService.createStorageArea(mockStorageAreaWithToDto);

      expect(mockHttpService.post).toHaveBeenCalledWith('/StorageArea', mockStorageAreaDto);
      expect(result).toEqual(mockStorageArea);
    });

    it('should throw error when storage area already exists (Conflict)', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Conflict: Storage area already exists'));

      await expect(storageAreaService.createStorageArea(mockStorageAreaWithToDto)).rejects.toThrow(
        'Conflict: Storage area already exists'
      );
    });

    it('should throw error when validation fails (Bad Request)', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Bad Request: Invalid data'));

      await expect(storageAreaService.createStorageArea(mockStorageAreaWithToDto)).rejects.toThrow('Bad Request: Invalid data');
    });

    it('should throw error when entity not found during creation', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Not Found: Related entity missing'));

      await expect(storageAreaService.createStorageArea(mockStorageAreaWithToDto)).rejects.toThrow(
        'Not Found: Related entity missing'
      );
    });

    it('should throw error on internal server error', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Internal Server Error'));

      await expect(storageAreaService.createStorageArea(mockStorageAreaWithToDto)).rejects.toThrow('Internal Server Error');
    });
  });

  describe('updateStorageArea', () => {
    it('should return updated storage area with valid data', async () => {
      vi.mocked(mockHttpService.put).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockStorageArea,
      } as Response<StorageArea>);

      const result = await storageAreaService.updateStorageArea(mockStorageAreaWithToDto);

      expect(mockHttpService.put).toHaveBeenCalledWith('/StorageArea/WH001', mockStorageAreaDto);
      expect(result).toEqual(mockStorageArea);
    });

    it('should throw error when storage area not found during update', async () => {
      vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Not Found'));

      await expect(storageAreaService.updateStorageArea(mockStorageAreaWithToDto)).rejects.toThrow('Not Found');
    });

    it('should throw error on validation error during update', async () => {
      vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Bad Request: Invalid update data'));

      await expect(storageAreaService.updateStorageArea(mockStorageAreaWithToDto)).rejects.toThrow(
        'Bad Request: Invalid update data'
      );
    });

    it('should throw error on internal server error during update', async () => {
      vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Internal Server Error'));

      await expect(storageAreaService.updateStorageArea(mockStorageAreaWithToDto)).rejects.toThrow('Internal Server Error');
    });
  });

  describe('deleteStorageArea', () => {
    it('should successfully delete a storage area', async () => {
      vi.mocked(mockHttpService.delete).mockResolvedValue({
        status: 204,
        statusText: 'No Content',
        data: undefined,
      } as Response<void>);

      await storageAreaService.deleteStorageArea('WH001');

      expect(mockHttpService.delete).toHaveBeenCalledWith('/StorageArea/WH001');
    });

    it('should throw error when storage area not found during deletion', async () => {
      vi.mocked(mockHttpService.delete).mockRejectedValue(new Error('Not Found'));

      await expect(storageAreaService.deleteStorageArea('INVALID')).rejects.toThrow('Not Found');
    });

    it('should throw error on validation error during deletion', async () => {
      vi.mocked(mockHttpService.delete).mockRejectedValue(new Error('Bad Request: Cannot delete'));

      await expect(storageAreaService.deleteStorageArea('WH001')).rejects.toThrow('Bad Request: Cannot delete');
    });

    it('should throw error on internal server error during deletion', async () => {
      vi.mocked(mockHttpService.delete).mockRejectedValue(new Error('Internal Server Error'));

      await expect(storageAreaService.deleteStorageArea('WH001')).rejects.toThrow('Internal Server Error');
    });
  });

  describe('getNumberOfStorageAreas', () => {
    it('should return the total number of storage areas', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: 15,
      } as Response<number>);

      const result = await storageAreaService.getNumberOfStorageAreas();

      expect(mockHttpService.get).toHaveBeenCalledWith('/StorageArea/count');
      expect(result).toBe(15);
    });

    it('should throw error when count request fails', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Internal Server Error'));

      await expect(storageAreaService.getNumberOfStorageAreas()).rejects.toThrow('Internal Server Error');
    });
  });
});
