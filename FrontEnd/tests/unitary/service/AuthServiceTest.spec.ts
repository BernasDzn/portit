import { describe, it, expect, beforeEach, vi } from 'vitest';
import { AuthService } from '@/service/AuthService';
import type { IHttpService } from '@/service/IService/IHttpService';
import type { AppJWTResponse } from '@/service/IService/IAuthService';

// Mock IHttpService
const mockHttp = {
    post: vi.fn(),
    get: vi.fn(),
} as unknown as IHttpService;

// Mock window.google.accounts.id
const mockInit = vi.fn();
const mockRenderButton = vi.fn();

(global as any).window = {
    google: {
        accounts: {
            id: {
                initialize: mockInit,
                renderButton: mockRenderButton
            }
        }
    },
    document: {
        getElementById: vi.fn(() => ({}))
    }
};

describe('AuthService', () => {

    let service: AuthService;

    beforeEach(() => {
        vi.clearAllMocks();
        service = new AuthService(mockHttp);
    });

    // ----------------------------------------------------------------------
    // getAppJWTToken
    // ----------------------------------------------------------------------

    it('should call POST /auth/login/google and return JWT response', async () => {

        const fakeResponse: AppJWTResponse = {
            user: {
                id: '1',
                name: 'Test',
                email: 'a@a.com',
                picture: 'pic',
                role: 0
            },
            token: 'abc123',
            expiresIn: 3600
        };

        mockHttp.post = vi.fn().mockResolvedValue({ data: fakeResponse });

        const result = await service.getAppJWTToken('myToken');

        expect(mockHttp.post).toHaveBeenCalledWith(
            '/auth/login/google',
            JSON.stringify({ token: 'myToken' })
        );
        expect(result).toEqual(fakeResponse);
    });

    // ----------------------------------------------------------------------
    // activateUser
    // ----------------------------------------------------------------------

    it('should activate user by calling POST /SystemUser/activate-with-token', async () => {

        mockHttp.post = vi.fn().mockResolvedValue({});

        await service.activateUser('mail@test.com', 'abc', 'raw_id_token');

        expect(mockHttp.post).toHaveBeenCalledWith(
            '/SystemUser/activate-with-token?emailAddress=mail%40test.com&token=abc',
            { idToken: 'raw_id_token' }
        );
    });

    // ----------------------------------------------------------------------
    // initGoogleSignIn
    // ----------------------------------------------------------------------

    it('should call google.accounts.id.initialize and renderButton', () => {

        const callback = vi.fn();
        const errorCallback = vi.fn();

        service.initGoogleSignIn(callback, errorCallback);

        expect(mockInit).toHaveBeenCalled();
        expect(mockRenderButton).toHaveBeenCalled();
    });

    // ----------------------------------------------------------------------
    // initGoogleLoginAccountActivation
    // ----------------------------------------------------------------------

    it('should initialize google sign-in for activation', () => {

        const callback = vi.fn();
        const errorCallback = vi.fn();

        service.initGoogleLoginAccountActivation(callback, errorCallback);

        expect(mockInit).toHaveBeenCalled();
    });

    // ----------------------------------------------------------------------
    // whoAmI
    // ----------------------------------------------------------------------

    it('should return parsed User when /auth/me succeeds', async () => {

        mockHttp.get = vi.fn().mockResolvedValue({
            data: {
                sub: 'user1',
                name: 'John',
                email: 'john@test.com',
                picture: 'avatar.png',
                role: 2
            }
        });

        const user = await service.whoAmI();

        expect(user).toEqual({
            id: 'user1',
            name: 'John',
            email: 'john@test.com',
            avatar: 'avatar.png',
            role: 2
        });
    });

    // ----------------------------------------------------------------------
    // logout
    // ----------------------------------------------------------------------

    it('should call POST /auth/logout', async () => {

        mockHttp.post = vi.fn().mockResolvedValue({});

        await service.logout();

        expect(mockHttp.post).toHaveBeenCalledWith('/auth/logout', {});
    });

});
