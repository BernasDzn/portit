import { Request, Response, NextFunction } from 'express';
import { verify, JwtPayload } from 'jsonwebtoken';
import config from '../config/config';
import { UserDto } from '../dto/userDto';

declare module 'express-serve-static-core' {
    interface Request {
        user?: UserDto;
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
        const decoded = verify(
            token, config.jwt.jwtSecret, {
                // issuer: config.jwt.jwtIssuer,
                // audience: config.jwt.jwtAudience
            }
        );
        req.user = {
            id: (decoded as JwtPayload).id,
            emailAddress: (decoded as any).email_address,
            name: (decoded as any).name,
            user_role: (decoded as any).user_role,
            token: token
        };

        //console.log(`Authenticated user ${req.user.emailAddress} with role ${req.user.user_role}`);

        next();
    } catch (error) {

        console.error('Token verification failed:', error);
        return res.status(400).json({ message: 'Invalid token' });
    }
};
