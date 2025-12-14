import { Router } from 'express';
import operationPlan from './routes/operationPlanRoutes';
import scheduleRequest from './routes/scheduleRequestRoutes';
import complementaryTask from './routes/complementaryTaskRoutes';
import incidentType from './routes/incidentTypeRoutes';

export default () => {
	const app = Router();

	operationPlan(app);
	scheduleRequest(app);
	complementaryTask(app);
	incidentType(app);
	
	return app
}