export interface AppJWTResponse {
    user: {
      id: string;
      name: string;
      email: string;
      picture: string;
    };
    token: string,
    expiresIn: number;
}

export interface IAuthService {
    initGoogleSignIn(callback: Function): void;
    getAppJWTToken(token: string): Promise<AppJWTResponse>;

    whoAmI(): Promise<AppJWTResponse>;
}