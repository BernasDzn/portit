import { NextFunction, Request, Response, Router } from 'express';
import { authMiddleware } from '../middlewares/authMiddleware';
import { authzMiddleware } from '../middlewares/authzMiddleware';
import { UserRole } from '../../domain/user';
import Container from 'typedi';
import { TaskCategoryController } from '../../controllers/taskCategoryController';

const route = Router();

export default (app: Router) => {
	app.use('/task-categories', route);

	const getCtrl = () => Container.get(TaskCategoryController);

	route.get(
		'/',
		[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
		(req: Request, res: Response, next: NextFunction) => getCtrl().getAllCategories(req, res, next)
	);

	route.get(
		'/code/:code',
		[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
		(req: Request, res: Response, next: NextFunction) => getCtrl().getCategoryByCode(req, res, next)
	);

	route.post(
		'/',
		[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
		(req: Request, res: Response, next: NextFunction) => getCtrl().createCategory(req, res, next)
	);
	
	route.put(
		'/:id',
		[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
		(req: Request, res: Response, next: NextFunction) => getCtrl().updateCategory(req, res, next)
	);

}