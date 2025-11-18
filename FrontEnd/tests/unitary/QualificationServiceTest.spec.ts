import { describe, it, expect, vi, beforeEach } from 'vitest';
import { QualificationService } from '@/service/QualificationService';
import type { IHttpService, Response } from '@/service/IService/IHttpService';
import type { Qualification } from '@/model/Qualifications';
import type { QualificationDto } from '@/model/dto/QualificationDto';
import type { Page } from '@/model/Page';

describe('QualificationService', () => {
  let qualificationService: QualificationService;
  let mockHttpService: IHttpService;

  const mockQualificationDto: QualificationDto = {
    idCode: 'Q001',
    qualificationName: 'First Aid',
  };

  const mockQualification: Qualification = {
    idCode: 'Q001',
    qualificationName: 'First Aid',
  } as Qualification;

  const mockPage: Page<Qualification> = {
    items: [mockQualification],
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

    qualificationService = new QualificationService(mockHttpService);
  });

  // -------------------------------------------------
  // getQualifications
  // -------------------------------------------------
  describe('getQualifications', () => {
    it('should return OK with a list of qualifications when no filter is provided', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockPage,
      } as Response<Page<Qualification>>);

      const result = await qualificationService.getQualifications();

      expect(mockHttpService.get).toHaveBeenCalledWith('/Qualification/filter');
      expect(result).toEqual(mockPage);
    });

    it('should return OK with filtered list of qualifications when filter is provided', async () => {
      const filter = {
        filter: {
          idCode: 'Q001',
          qualificationName: 'Aid',
        },
        pageNumber: 1,
        pageSize: 10,
      };

      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockPage,
      } as Response<Page<Qualification>>);

      const result = await qualificationService.getQualifications(filter);

      expect(mockHttpService.get).toHaveBeenCalledWith(
        '/Qualification/filter?Code=Q001&QualificationName=Aid&PageNumber=1&PageSize=10'
      );
      expect(result).toEqual(mockPage);
    });

    it('should throw error when getQualifications fails', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Network error'));

      await expect(qualificationService.getQualifications()).rejects.toThrow('Network error');
    });
  });

  // -------------------------------------------------
  // getQualificationById
  // -------------------------------------------------
  describe('getQualificationById', () => {
    it('should return qualification when found', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockQualification,
      } as Response<Qualification>);

      const result = await qualificationService.getQualificationById('Q001');

      expect(mockHttpService.get).toHaveBeenCalledWith('/Qualification/Q001');
      expect(result).toEqual(mockQualification);
    });

    it('should throw error when not found', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Not found'));

      await expect(qualificationService.getQualificationById('INVALID')).rejects.toThrow('Not found');
    });
  });

  // -------------------------------------------------
  // addQualification
  // -------------------------------------------------
  describe('addQualification', () => {
    it('should return created qualification', async () => {
      vi.mocked(mockHttpService.post).mockResolvedValue({
        status: 201,
        statusText: 'Created',
        data: mockQualification,
      } as Response<Qualification>);

      const result = await qualificationService.addQualification(mockQualificationDto);

      expect(mockHttpService.post).toHaveBeenCalledWith('/Qualification', mockQualificationDto);
      expect(result).toEqual(mockQualification);
    });

    it('should throw error on conflict', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Conflict: Already exists'));

      await expect(qualificationService.addQualification(mockQualificationDto))
        .rejects.toThrow('Conflict: Already exists');
    });

    it('should throw validation error', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Bad Request: Invalid data'));

      await expect(qualificationService.addQualification(mockQualificationDto))
        .rejects.toThrow('Bad Request: Invalid data');
    });

    it('should throw internal server error', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Internal Server Error'));

      await expect(qualificationService.addQualification(mockQualificationDto))
        .rejects.toThrow('Internal Server Error');
    });
  });

  // -------------------------------------------------
  // updateQualification
  // -------------------------------------------------
  describe('updateQualification', () => {
    it('should return updated qualification', async () => {
      vi.mocked(mockHttpService.put).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockQualification,
      } as Response<Qualification>);

      const result = await qualificationService.updateQualification(mockQualificationDto);

      expect(mockHttpService.put).toHaveBeenCalledWith('/Qualification/Q001', mockQualificationDto);
      expect(result).toEqual(mockQualification);
    });

    it('should throw error when not found', async () => {
      vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Not Found'));

      await expect(qualificationService.updateQualification(mockQualificationDto))
        .rejects.toThrow('Not Found');
    });

    it('should throw validation error', async () => {
      vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Bad Request: Invalid update'));

      await expect(qualificationService.updateQualification(mockQualificationDto))
        .rejects.toThrow('Bad Request: Invalid update');
    });

    it('should throw internal server error', async () => {
      vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Internal Server Error'));

      await expect(qualificationService.updateQualification(mockQualificationDto))
        .rejects.toThrow('Internal Server Error');
    });
  });

  // -------------------------------------------------
  // getNumberOfQualifications
  // -------------------------------------------------
  describe('getNumberOfQualifications', () => {
    it('should return total number of qualifications', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: 12,
      } as Response<number>);

      const result = await qualificationService.getNumberOfQualifications();

      expect(mockHttpService.get).toHaveBeenCalledWith('/Qualification/count');
      expect(result).toBe(12);
    });

    it('should throw error when count fails', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Internal Server Error'));

      await expect(qualificationService.getNumberOfQualifications()).rejects.toThrow('Internal Server Error');
    });
  });
});
