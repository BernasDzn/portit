import type { User } from "@/model/User";

export interface AppJWTResponse {
    user: {
        id: string;
        name: string;
        email: string;
        picture: string;
        role: number;
    };
    token: string,
    expiresIn: number;
}

export interface IAuthService {
    initGoogleLoginAccountActivation(callback: Function, errorCallback: Function): void;
    initGoogleSignIn(callback: Function, errorCallback: Function): void;
    getAppJWTToken(token: string): Promise<AppJWTResponse>;
    activateUser(email: string, token: string, sub: string): Promise<void>;
    whoAmI(): Promise<User>;
    logout(): Promise<void>;
}