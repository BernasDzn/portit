import { Router, Request, Response, NextFunction } from 'express';
import { authMiddleware } from '../middlewares/authMiddleware';
import { authzMiddleware } from '../middlewares/authzMiddleware';
import { UserRole } from '../../domain/user';
import Container from 'typedi';
import ComplementaryTaskController from '../../controllers/complementaryTaskController';

const route = Router();

export default (app: Router) => {
	app.use('/complementary-tasks', route);

	const getCtrl = () => Container.get(ComplementaryTaskController);

	route.post(
		'/',
		[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
		(req: Request, res: Response, next: NextFunction) => getCtrl().create(req, res, next)
	);

	route.get(
		'/',
		[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
		(req: Request, res: Response, next: NextFunction) => getCtrl().getAll(req, res, next)
	);

	route.get(
		'/impacting-operations',
		[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
		(req: Request, res: Response, next: NextFunction) => getCtrl().getImpactingOperations(req, res, next)
	);

	route.get(
		'/:id',
		[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
		(req: Request, res: Response, next: NextFunction) => getCtrl().getById(req, res, next)
	);

	route.put(
		'/:id',
		[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
		(req: Request, res: Response, next: NextFunction) => getCtrl().update(req, res, next)
	);

	route.delete(
		'/:id',
		[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
		(req: Request, res: Response, next: NextFunction) => getCtrl().delete(req, res, next)
	);

	route.post(
		'/:id/complete',
		[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
		(req: Request, res: Response, next: NextFunction) => getCtrl().complete(req, res, next)
	);
};
