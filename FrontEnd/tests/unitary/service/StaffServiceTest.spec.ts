import { describe, it, expect, vi, beforeEach } from 'vitest';
import { StaffService } from '@/service/StaffService';
import type { IHttpService, Response } from '@/service/IService/IHttpService';
import type { Staff } from '@/model/Staff';
import type { StaffDto } from '@/model/dto/StaffDto';
import type { Page } from '@/model/Page';

describe('StaffService', () => {
  let staffService: StaffService;
  let mockHttpService: IHttpService;

  const mockStaffDto: StaffDto = {
    mechanographicNumber: 'STF000001',
    name: 'John Doe',
    email: 'john.doe@example.com',
    phoneNumber: '900000000',
    status: 1,
    operationalWindow: { shifts: [] },
    qualificationsCodes: ['Q001', 'Q002'],
  };

  const mockStaffWithToDto: Staff = {
    mechanographicNumber: 'STF000001',
    name: 'John Doe',
    email: 'john.doe@example.com',
    phoneNumber: '900000000',
    status: 1,
    operationalWindow: {},
    qualifications: [],
    toDto: () => mockStaffDto,
  } as Staff;

  const mockStaff: Staff = {
    mechanographicNumber: 'STF000001',
    name: 'John Doe',
    email: 'john.doe@example.com',
    phoneNumber: '900000000',
    status: 1,
    operationalWindow: {},
    qualifications: [],
  } as Staff;

  const mockPage: Page<Staff> = {
    items: [mockStaff],
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

    staffService = new StaffService(mockHttpService);
  });

  describe('getStaffs', () => {
    it('should return OK with list of staffs when no filter is provided', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockPage,
      } as Response<Page<Staff>>);

      const result = await staffService.getStaffs();

      expect(mockHttpService.get).toHaveBeenCalledWith('/Staff/filter');
      expect(result).toEqual(mockPage);
    });

    it('should return OK with filtered list of staffs when filter is provided', async () => {
      const filter = {
        filter: {
          mechanographicNumber: 'STF000001',
          name: 'John',
          email: 'john@example.com',
          status: 1,
          phoneNumber: '900000000',
        },
        pageNumber: 1,
        pageSize: 10,
      };

      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockPage,
      } as Response<Page<Staff>>);

      const result = await staffService.getStaffs(filter);

      expect(mockHttpService.get).toHaveBeenCalledWith(
        '/Staff/filter?MechanographicNumber=STF000001&Name=John&Email=john@example.com&Status=1&PhoneNumber=900000000&PageNumber=1&PageSize=10'
      );
      expect(result).toEqual(mockPage);
    });

    it('should throw error when getStaffs fails', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Network error'));

      await expect(staffService.getStaffs()).rejects.toThrow('Network error');
    });
  });

  describe('getStaffByMechanographicNumber', () => {
    it('should return staff when found', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockPage,
      } as Response<Page<Staff>>);

      const result = await staffService.getStaffByMechanographicNumber('STF000001');

      expect(mockHttpService.get).toHaveBeenCalledWith('/Staff/filter?MechanographicNumber=STF000001');
      expect(result).toEqual(mockStaff);
    });

    it('should throw error when staff is not found', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Not found'));

      await expect(staffService.getStaffByMechanographicNumber('INVALID')).rejects.toThrow('Not found');
    });
  });

  describe('createStaff', () => {
    it('should return created staff with valid data', async () => {
      vi.mocked(mockHttpService.post).mockResolvedValue({
        status: 201,
        statusText: 'Created',
        data: mockStaff,
      } as Response<Staff>);

      const result = await staffService.createStaff(mockStaffWithToDto);

      expect(mockHttpService.post).toHaveBeenCalledWith('/Staff', mockStaffDto);
      expect(result).toEqual(mockStaff);
    });

    it('should throw error when staff already exists (Conflict)', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Conflict: Staff already exists'));

      await expect(staffService.createStaff(mockStaffWithToDto)).rejects.toThrow('Conflict: Staff already exists');
    });

    it('should throw error when validation fails (Bad Request)', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Bad Request: Invalid data'));

      await expect(staffService.createStaff(mockStaffWithToDto)).rejects.toThrow('Bad Request: Invalid data');
    });

    it('should throw error when entity not found during creation', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Not Found: Related entity missing'));

      await expect(staffService.createStaff(mockStaffWithToDto)).rejects.toThrow('Not Found: Related entity missing');
    });

    it('should throw error on internal server error', async () => {
      vi.mocked(mockHttpService.post).mockRejectedValue(new Error('Internal Server Error'));

      await expect(staffService.createStaff(mockStaffWithToDto)).rejects.toThrow('Internal Server Error');
    });
  });

  describe('deactivateStaff', () => {
    it('should successfully deactivate a staff member', async () => {
      vi.mocked(mockHttpService.delete).mockResolvedValue({
        status: 204,
        statusText: 'No Content',
        data: undefined,
      } as Response<void>);

      await staffService.deactivateStaff('STF000001');

      expect(mockHttpService.delete).toHaveBeenCalledWith('/Staff/STF000001');
    });

    it('should throw error when staff not found during deactivation', async () => {
      vi.mocked(mockHttpService.delete).mockRejectedValue(new Error('Not Found'));

      await expect(staffService.deactivateStaff('INVALID')).rejects.toThrow('Not Found');
    });

    it('should throw error on validation error during deactivation', async () => {
      vi.mocked(mockHttpService.delete).mockRejectedValue(new Error('Bad Request: Cannot deactivate'));

      await expect(staffService.deactivateStaff('STF000001')).rejects.toThrow('Bad Request: Cannot deactivate');
    });

    it('should throw error on internal server error during deactivation', async () => {
      vi.mocked(mockHttpService.delete).mockRejectedValue(new Error('Internal Server Error'));

      await expect(staffService.deactivateStaff('STF000001')).rejects.toThrow('Internal Server Error');
    });
  });

  describe('updateStaff', () => {
    it('should return updated staff with valid data', async () => {
      vi.mocked(mockHttpService.put).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: mockStaff,
      } as Response<Staff>);

      const result = await staffService.updateStaff(mockStaffWithToDto);

      expect(mockHttpService.put).toHaveBeenCalledWith('/Staff/STF000001', mockStaffDto);
      expect(result).toEqual(mockStaff);
    });

    it('should throw error when staff not found during update', async () => {
      vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Not Found'));

      await expect(staffService.updateStaff(mockStaffWithToDto)).rejects.toThrow('Not Found');
    });

    it('should throw error on validation error during update', async () => {
      vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Bad Request: Invalid update data'));

      await expect(staffService.updateStaff(mockStaffWithToDto)).rejects.toThrow('Bad Request: Invalid update data');
    });

    it('should throw error on internal server error during update', async () => {
      vi.mocked(mockHttpService.put).mockRejectedValue(new Error('Internal Server Error'));

      await expect(staffService.updateStaff(mockStaffWithToDto)).rejects.toThrow('Internal Server Error');
    });
  });

  describe('getNumberOfStaffs', () => {
    it('should return the total number of staff members', async () => {
      vi.mocked(mockHttpService.get).mockResolvedValue({
        status: 200,
        statusText: 'OK',
        data: 42,
      } as Response<number>);

      const result = await staffService.getNumberOfStaffs();

      expect(mockHttpService.get).toHaveBeenCalledWith('/Staff/count');
      expect(result).toBe(42);
    });

    it('should throw error when count request fails', async () => {
      vi.mocked(mockHttpService.get).mockRejectedValue(new Error('Internal Server Error'));

      await expect(staffService.getNumberOfStaffs()).rejects.toThrow('Internal Server Error');
    });
  });
});
