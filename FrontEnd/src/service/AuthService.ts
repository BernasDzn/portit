import { inject, injectable } from 'inversify';
import { TYPES } from '@/inversify/types';
import type { AppJWTResponse, IAuthService } from './IService/IAuthService';
import type { IHttpService } from './IService/IHttpService';

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

        let res = await this.http.post('/Login/google', JSON.stringify({ token: token }));
        return res.data as AppJWTResponse;
    }

    initGoogleSignIn(callback: Function) {
        
        // Handle the google response
        const handleCredentialResponse = async (response: any) => {
            const idToken = response.credential;
            const res: AppJWTResponse = await this.getAppJWTToken(idToken);
            
            // Pass the response to the callback
            callback(res);
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

    async whoAmI(): Promise<AppJWTResponse> {
        
        let res = await this.http.get('/Login/me');
        console.log('whoAmI response:', res);
        return res.data as Promise<AppJWTResponse>;
    }

    // async handleCredentialResponse(response: any) {
    //     const idToken = response.credential;
    
    //     let res = await this.getAppJWTToken(idToken);
    
    //     // if (res.ok) {
    //     //     let token = await res.text();
    //     //     token = JSON.parse(token).token;
            
    //     //     // NAO FAZER ISTO !!
    //     //     // ATENÇAO CODIGO MAL FEITO
    //     //     localStorage.setItem('authToken', token);
    //     //     window.location.href = '/';
    
    //     // } else {
    //     //     console.error('uh oh', res.status)
    //     // }
    // };
}