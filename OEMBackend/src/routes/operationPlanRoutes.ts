import { Router } from 'express';
import { authMiddleware } from '../middlewares/authMiddleware';
import { OperationPlanController } from '../controllers/operationPlanController'
import { authzMiddleware } from '../middlewares/authzMiddleware';
import { UserRole } from '../domain/user';

const router = Router();
const controller = new OperationPlanController();

router.get(
	'/',
	//[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
	controller.getPlans.bind(controller)
);

router.get(
	'/by-date',
	//[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
	controller.getPlansByDate.bind(controller)
);

export default router;