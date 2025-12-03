import { Router } from 'express';
import { authMiddleware } from '../middlewares/authMiddleware';
import { getPlanById, getPlans, getPlansByDateRange, groupPlansByDate, getNotificationWithoutPlan } from '../controllers/operationPlanController';
import { authzMiddleware } from '../middlewares/authzMiddleware';
import { UserRole } from '../domain/dto/userDto';

const router = Router();

router.get('/', [authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)], getPlans);
router.get('/group', [authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)], groupPlansByDate);
router.get('/range', [authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)], getPlansByDateRange);
router.get('/notifications-without-plan', [authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)], getNotificationWithoutPlan);
router.get('/:id', [authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)], getPlanById);

export default router;