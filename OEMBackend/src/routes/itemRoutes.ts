import { Router } from 'express';
import {
  createItem,
  getItems,
  getItemById,
  updateItem,
  deleteItem,
} from '../controllers/itemController';
import { authMiddleware } from '../middlewares/authMiddleware';

const router = Router();

router.get('/', [authMiddleware], getItems);
router.get('/:id', [authMiddleware], getItemById);
router.post('/', [authMiddleware], createItem);
router.put('/:id', [authMiddleware], updateItem);
router.delete('/:id', [authMiddleware], deleteItem);

export default router;