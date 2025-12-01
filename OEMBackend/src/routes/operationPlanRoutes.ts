import { Router } from 'express';
import { authMiddleware } from '../middlewares/authMiddleware';
import { getPlanById, getPlans, getPlansByDateRange, groupPlansByDate } from '../controllers/operationPlanController';
import { authzMiddleware } from '../middlewares/authzMiddleware';
import { UserRole } from '../domain/dto/userDto';

const router = Router();

router.get('/', [authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)], getPlans);
router.get('/group', [authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)], groupPlansByDate);
router.get('/range', [authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)], getPlansByDateRange);
router.get('/:id', [authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)], getPlanById);

export default router;