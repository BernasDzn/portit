import { Router } from 'express';
import { authMiddleware } from '../middlewares/authMiddleware';
import { getPlans } from '../controllers/operationPlanController';
import { authzMiddleware } from '../middlewares/authzMiddleware';
import { UserRole } from '../domain/dto/userDto';

const router = Router();

router.get('/', [authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)], getPlans);

export default router;