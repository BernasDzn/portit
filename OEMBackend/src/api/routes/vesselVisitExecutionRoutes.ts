import { NextFunction, Request, Response, Router } from 'express';
import { authMiddleware } from '../middlewares/authMiddleware';
import { authzMiddleware } from '../middlewares/authzMiddleware';
import { UserRole } from '../../domain/user';
import Container from 'typedi';
import { TaskCategoryController } from '../../controllers/taskCategoryController';
import { VesselVisitExecutionController } from '../../controllers/vesselVisitExecutionController';

const route = Router();

export default (app: Router) => {
	app.use('/vessel-visit-executions', route);

	const getCtrl = () => Container.get(VesselVisitExecutionController);

	route.post(
		'/:relatedVVN/open',
		[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
		(req: Request, res: Response, next: NextFunction) => getCtrl().openVesselVisitExecution(req, res, next)
	);

    route.put(
        '/:relatedVVN/close',
        [authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
        (req: Request, res: Response, next: NextFunction) => getCtrl().closeVesselVisitExecution(req, res, next)
    );

    route.put(
        '/:relatedVVN/operations/start',
        [authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
        (req: Request, res: Response, next: NextFunction) => getCtrl().startOperation(req, res, next)
    )

    route.put(
        '/:relatedVVN/operations/:operationId/complete',
        [authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
        (req: Request, res: Response, next: NextFunction) => getCtrl().completeOperation(req, res, next)
    )

    route.get(
        '/:relatedVVN',
        [authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
        (req: Request, res: Response, next: NextFunction) => getCtrl().getVesselVisitExecution(req, res, next)
    );

    route.get(
        '/',
        [authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
        (req: Request, res: Response, next: NextFunction) => getCtrl().getAllVesselVisitExecutions(req, res, next)
    );
}