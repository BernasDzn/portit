import { NextFunction, Request, Response, Router } from 'express';
import { authMiddleware } from '../middlewares/authMiddleware';
import { authzMiddleware } from '../middlewares/authzMiddleware';
import { UserRole } from '../../domain/user';
import Container from 'typedi';
import SchedulingRequestController from '../../controllers/schedulingRequestController';

const route = Router();

export default (app: Router) => {
	app.use('/schedule', route);

	const getCtrl = () => Container.get(SchedulingRequestController);

	route.get(
		'/request',
		[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
		(req: Request, res: Response, next: NextFunction) => getCtrl().scheduleRequest(req, res, next)
	);

	route.get(
		'/queueState',
		[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
		(req: Request, res: Response, next: NextFunction) => getCtrl().getQueueState(req, res, next)
	);

	route.post(
		'/acceptRequest',
		[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
		(req: Request, res: Response, next: NextFunction) => getCtrl().acceptRequest(req, res, next)
	);
	
	route.post(
		'/rejectRequest',
		[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
		(req: Request, res: Response, next: NextFunction) => getCtrl().rejectRequest(req, res, next)
	);

}