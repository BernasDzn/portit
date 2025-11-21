import { describe, it, expect, beforeEach, vi } from 'vitest';
import { SchedulingService } from '@/service/SchedulingService';
import type { IHttpService } from '@/service/IService/IHttpService';
import type { Schedule } from '@/model/values/Schedule';

// -------------------------------------------------------------
// Mock IHttpService
// -------------------------------------------------------------
const mockHttp = {
    getWithoutCredentials: vi.fn(),
} as unknown as IHttpService;

// -------------------------------------------------------------
// Mock jsPDF
// -------------------------------------------------------------
const mockText = vi.fn();
const mockSetFontSize = vi.fn();
const mockSetFont = vi.fn();
const mockOutput = vi.fn(() => new ArrayBuffer(8)); // Fake PDF bytes

vi.mock('jspdf', () => {
    return {
        default: vi.fn().mockImplementation(() => ({
            text: mockText,
            setFontSize: mockSetFontSize,
            setFont: mockSetFont,
            output: mockOutput,
        }))
    };
});

// -------------------------------------------------------------
// Mock useSession composable
// -------------------------------------------------------------
vi.mock('@/composables/session', () => {
    return {
        useSession: () => ({
            authenticatedUser: { name: 'Tiago' }
        })
    };
});

describe('SchedulingService', () => {

    let service: SchedulingService;

    beforeEach(() => {
        vi.clearAllMocks();
        service = new SchedulingService(mockHttp);
    });

    // ----------------------------------------------------------------------
    // calculateDateOffset
    // ----------------------------------------------------------------------

    it('should calculate date offset in hours', () => {
        const base = new Date('2024-01-01T00:00:00Z');
        const result = service.calculateDateOffset(base, 5);

        expect(result.getUTCHours()).toBe(5);
    });

    // ----------------------------------------------------------------------
    // generateSchedulePDF
    // ----------------------------------------------------------------------

    it('should generate a PDF and return Uint8Array', async () => {
        const schedule: Schedule = {
            status: 'success',
            comment: '',
            data: [
                {
                    name: 'Ship A',
                    loading_enter_time: 1,
                    loading_exit_time: 5
                }
            ],
            metrics: {
                algorithm: 'test-alg',
                totalDelay: 2,
                computationTime: 0.01,
                vesselCount: 1
            }
        };

        const date = new Date('2024-01-01');
        
        const result = await service.generateSchedulePDF(schedule, date, 'Dock-1');

        expect(mockSetFontSize).toHaveBeenCalled();
        expect(mockText).toHaveBeenCalled();
        expect(mockOutput).toHaveBeenCalledWith('arraybuffer');
        expect(result).toBeInstanceOf(Uint8Array);
    });

    // ----------------------------------------------------------------------
    // scheduleForDay
    // ----------------------------------------------------------------------

    it('should call API with correct URL and format response', async () => {
        const fakeApiResp = {
            data: {
                status: 'success',
                data: [{ name: 'Ship A' }],
                metrics: { algorithm: 'myAlg' }
            }
        };

        mockHttp.getWithoutCredentials = vi.fn().mockResolvedValue(fakeApiResp);

        const date = new Date('2024-01-05');

        const result = await service.scheduleForDay(date, 'DockX', 'Alg1', 3);

        expect(mockHttp.getWithoutCredentials).toHaveBeenCalledWith(
            `/prolog/schedule?day=2024-01-05&dock=DockX&alg=Alg1&daysAhead=3`
        );

        expect(result).toEqual({
            status: 'success',
            comment: '',
            data: [{ name: 'Ship A' }],
            metrics: { algorithm: 'myAlg' }
        });
    });

    it('should fall back to defaults when API returns empty', async () => {
        mockHttp.getWithoutCredentials = vi.fn().mockResolvedValue({ data: null });

        const date = new Date('2024-01-05');

        const result = await service.scheduleForDay(date, 'D1', 'A1');

        expect(result).toEqual({
            status: 'success',
            comment: '',
            data: [],
            metrics: undefined
        });
    });

});
