import { Router } from 'express';
import operationPlan from './routes/operationPlanRoutes';
import scheduleRequest from './routes/scheduleRequestRoutes';
import incidentType from './routes/incidentTypeRoutes';
import taskCategory from './routes/taskCategoryRouters';
import vesselVisitExecution from './routes/vesselVisitExecutionRoutes';

export default () => {
	const app = Router();

	operationPlan(app);
	scheduleRequest(app);
	incidentType(app);
    taskCategory(app);
    vesselVisitExecution(app);
	
	return app
}