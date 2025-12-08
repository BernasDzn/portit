import { Router } from 'express';
import { authMiddleware } from '../middlewares/authMiddleware';
import { authzMiddleware } from '../middlewares/authzMiddleware';
import { UserRole } from '../domain/user';
import { acceptRequest, getQueueState, rejectRequest, scheduleRequest } from '../controllers/schedulingRequestController';

const router = Router();

router.get(
	'/request',
	[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
	scheduleRequest
);

router.get(
	'/queueState',
	[authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
	getQueueState
);

router.post(
    '/acceptRequest',
    [authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
    acceptRequest
);

router.post(
    '/rejectRequest',
    [authMiddleware, authzMiddleware(UserRole.Administrator, UserRole.LogisticsOperator)],
    rejectRequest
);

export default router;