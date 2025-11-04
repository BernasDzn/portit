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

        let res = await this.http.post('/Login/google', JSON.stringify({ token: token }));
        return res.data as AppJWTResponse;
    }

    async activateUser(email: string, token: string): Promise<void> {
        // The API expects a PUT to /SystemUser/activate-with-token/
        await this.http.post(
            `/SystemUser/activate-with-token?emailAddress=${encodeURIComponent(email)}&token=${encodeURIComponent(token)}`
            , {});
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

    async whoAmI(): Promise<User> {
        
        let res = await this.http.get('/Login/me');
        console.log(res.data);
        return {
            id: res.data.sub,
            name: res.data.name,
            email: res.data.email,
            avatar: res.data.picture
        }
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