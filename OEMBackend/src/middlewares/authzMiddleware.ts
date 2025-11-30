import { Request, Response, NextFunction } from "express";

export const authzMiddleware = (...allowedRoles: string[]) => {
    return (req: Request, res: Response, next: NextFunction) => {

        if (!req.user) return res.status(401).json({ message: 'Unauthorized' });
        const userRole = req.user?.user_role;

        if (userRole === undefined || !allowedRoles.includes(userRole)) {
            return res.status(403).json({ message: 'Forbidden' });
        }
        next();
    };
}