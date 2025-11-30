import { Router } from 'express';
import { authMiddleware } from '../middlewares/authMiddleware';
import { getPlans } from '../controllers/operationPlanController';

const router = Router();

router.get('/', getPlans);

export default router;