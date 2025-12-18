import { Router, Request, Response, NextFunction } from 'express';
import { authMiddleware } from '../middlewares/authMiddleware';
import { authzMiddleware } from '../middlewares/authzMiddleware';
import { UserRole } from '../../domain/user';
import Container from 'typedi';
import OperationPlanController from '../../controllers/operationPlanController';

const route = Router();

export default (app: Router) => {
	app.use('/operation-plans', route);

	const getCtrl = () => Container.get(OperationPlanController);

	route.get(
		'/',
		[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
		(req: Request, res: Response, next: NextFunction) => getCtrl().getPlans(req, res, next)
	);

	route.get(
		'/by-date',
		[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
		(req: Request, res: Response, next: NextFunction) => getCtrl().getPlansByDate(req, res, next)
	);

	route.get(
		'/notifications-without-plan',
		[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
		(req: Request, res: Response, next: NextFunction) => getCtrl().getNotificationWithoutPlan(req, res, next)
	);
	
	route.get(
		'/:id',
		[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
		(req: Request, res: Response, next: NextFunction) => getCtrl().getPlanById(req, res, next)
	);

	route.post(
		'/regenerate',
		[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
		(req: Request, res: Response, next: NextFunction) => getCtrl().regeneratePlansForDay(req, res, next)
	);

	route.patch(
		'/:id',
		[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
		(req: Request, res: Response, next: NextFunction) => getCtrl().updateOperationPlan(req, res, next)
	);
	
}