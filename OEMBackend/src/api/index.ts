import { Router } from 'express';
import operationPlan from './routes/operationPlanRoutes';
import scheduleRequest from './routes/scheduleRequestRoutes';

export default () => {
	const app = Router();

	operationPlan(app);
	scheduleRequest(app);
	
	return app
}