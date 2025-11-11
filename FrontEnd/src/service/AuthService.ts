import { inject, injectable } from 'inversify';
import { TYPES } from '@/inversify/types';
import type { AppJWTResponse, IAuthService } from './IService/IAuthService';
import type { IHttpService } from './IService/IHttpService';
import type { User } from '@/model/User';

@injectable()
export class AuthService implements IAuthService {

    googleClientId: string;

    constructor(
        @inject(TYPES.api)
        private http: IHttpService
    ) {
        // The google app id
        this.googleClientId = '28670621917-0p4e3s7it08to15g7c591b4vjtvo9eaq.apps.googleusercontent.com';
    }

    async getAppJWTToken(token: string): Promise<AppJWTResponse> {

        let res = await this.http.post('/auth/login/google', JSON.stringify({ token: token }));
        return res.data as AppJWTResponse;
    }

    async activateUser(email: string, token: string, sub: string): Promise<void> {
        // The API expects a POST to /SystemUser/activate-with-token with the Google id_token
        // in the request body. The `sub` parameter here is actually the raw Google id_token
        // returned by the Google client (response.credential). Send it as { idToken }.
        const body = { idToken: sub };
        await this.http.post(
            `/SystemUser/activate-with-token?emailAddress=${encodeURIComponent(email)}&token=${encodeURIComponent(token)}`,
            body
        );
    }

    initGoogleSignIn(callback: Function, errorCallback: Function) {

        // Handle the google response
        const handleCredentialResponse = async (response: any) => {

            try {

                const idToken = response.credential;
                const res: AppJWTResponse = await this.getAppJWTToken(idToken);

                // Pass the response to the callback
                callback(res);

            } catch (error) {
                errorCallback(error);
            }
        };

        console.log('Initializing Google Sign-In');
        window.google.accounts.id.initialize({
            client_id: this.googleClientId,
            callback: handleCredentialResponse
        })

        window.google.accounts.id.renderButton(
            document.getElementById('google-signin-btn'),
            { theme: 'outline', size: 'large' }
        )
    }

    initGoogleLoginAccountActivation(callback: Function, errorCallback: Function) {
        console.log('Initializing Google Sign-In for Account Activation');
        window.google.accounts.id.initialize({
            client_id: this.googleClientId,
            // For activation we pass the raw Google response (contains credential/id_token)
            callback: (response: any) => {
                try {
                    callback(response);
                }
                catch (error) {
                    errorCallback(error);
                }
            }
        })
        // Render the button into the activation page so users can click to sign-in
        // This mirrors initGoogleSignIn which renders a visible button.
        try {
            const btnContainer = document.getElementById('google-signin-btn');
            if (btnContainer && window.google?.accounts?.id?.renderButton) {
                window.google.accounts.id.renderButton(
                    btnContainer,
                    { theme: 'outline', size: 'large' }
                );
            }
        } catch (e) {
            // Do not throw — rendering may fail in some contexts; initialization still works
            console.warn('Failed to render Google Sign-In button for activation page.', e);
        }
    }

    async whoAmI(): Promise<User> {
        try {
            let res: any = await this.http.get('/auth/me');
            if (res && res.data) {
                return {
                    id: res.data.sub,
                    name: res.data.name,
                    email: res.data.email,
                    avatar: res.data.picture,
                    role: res.data.role
                }
            }
        } catch (err) {
            console.debug('whoAmI failed or returned no data; treating as unauthenticated', err);
        }
        return null;
    }

    async logout(): Promise<void> {
        await this.http.post('/auth/logout', {});
    }
   
}