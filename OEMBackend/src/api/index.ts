import { Router } from 'express';
import operationPlan from './routes/operationPlanRoutes';
import scheduleRequest from './routes/scheduleRequestRoutes';
import incidentType from './routes/incidentTypeRoutes';

export default () => {
	const app = Router();

	operationPlan(app);
	scheduleRequest(app);
	incidentType(app);
	
	return app
}