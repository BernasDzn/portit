// The user representation from the database
export interface SystemUser {
    sub: string;
    emailAddress: string;
    isActive: boolean;
    role: number;
}

// The user representation from a token
export interface User {
    id: string; // A google sub
    email: string;
    name?: string;
    avatar?: string;
    role: number;
}