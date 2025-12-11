import { Router } from 'express';
import { authMiddleware } from '../middlewares/authMiddleware';
import { authzMiddleware } from '../middlewares/authzMiddleware';
import { UserRole } from '../domain/user';
import { getNotificationWithoutPlan, getPlanById, getPlans, getPlansByDate } from '../controllers/operationPlanController';

const router = Router();

router.get(
	'/',
	[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
	getPlans
);

router.get(
	'/by-date',
	[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
	getPlansByDate
);

router.get(
	'/notifications-without-plan', 
	[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)], 
	getNotificationWithoutPlan
);

router.get(
	'/:id',
	[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
	getPlanById
);

export default router;