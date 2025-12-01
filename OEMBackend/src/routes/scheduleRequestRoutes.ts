import { Router } from 'express';
import { authMiddleware } from '../middlewares/authMiddleware';
import { authzMiddleware } from '../middlewares/authzMiddleware';
import { UserRole } from '../domain/dto/userDto';
import { scheduleRequest } from '../controllers/schedulingRequestController';

const router = Router();

router.get('/request', [authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)], scheduleRequest);

export default router;