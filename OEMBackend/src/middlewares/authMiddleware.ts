import { Request, Response, NextFunction } from 'express';
import { verify, JwtPayload } from 'jsonwebtoken';
import config from '../config/config';

declare module 'express-serve-static-core' {
    interface Request {
        user?: string | JwtPayload;
    }
}

export const authMiddleware = (
    req: Request,
    res: Response,
    next: NextFunction
) => {

    // Try reading token from cookie first
    let token = req.cookies?.AuthToken;

    // Fallback to Authorization header
    if (!token) {
        const authHeader = req.header('Authorization');
        if (authHeader?.startsWith('Bearer ')) {
            token = authHeader.slice(7);
        } else {
            token = authHeader || null;
        }
    }

    if (!token) {
        return res.status(401).json({ message: 'Unauthorized' });
    }

    try {
        const decoded = verify(token, config.jwtSecret);
        req.user = decoded;
        next();
    } catch {
        return res.status(400).json({ message: 'Invalid token' });
    }
};
