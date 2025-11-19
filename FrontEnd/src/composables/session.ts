import type { User } from "@/model/SystemUser";

class Session {

    private static _instance: Session;

    authenticatedUser: User | null = null;
    authToken: string | null = null;
    expirationTime: number | null = null;

    private constructor() { }

    static getInstance(): Session {
        if (!Session._instance) {
            Session._instance = new Session();
        }
        return Session._instance;
    }

    isAuthenticated(): boolean {
        const session = Session.getInstance();
        return session.authenticatedUser !== null && session.authToken !== null;
    }

    setSession(user: User, token: string, expiryTime: number) {

        console.trace('Setting session for user:', user);

        this.authenticatedUser = user;
        this.authToken = token;
        
        const expiryMs = expiryTime * 60 * 1000; // Convert minutes to milliseconds
        const currentTime = new Date().getTime();
        this.expirationTime = currentTime + expiryMs;
    }

    isExpired(): boolean {
        
        if (this.expirationTime === null) {
            return true;
        }

        const currentTime = new Date().getTime();
        return currentTime >= this.expirationTime;
    }

    clearSession() {
        this.authenticatedUser = null;
        this.authToken = null;
        this.expirationTime = null;
    }
}

export const useSession = () => {
    return Session.getInstance();
}