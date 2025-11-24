import { Request, Response, NextFunction } from 'express';
import { itemService } from '../services/itemService';

/**
 * @swagger
 * /api/items:
 *   post:
 *     tags: [Items]
 *     requestBody:
 *       content:
 *         application/json:
 *           schema:
 *             properties:
 *               name:
 *                 type: string
 *     responses:
 *       201:
 *         description: Item created
 */
export const createItem = (req: Request, res: Response, next: NextFunction) => {
  try {
    const { name } = req.body;
    const newItem = itemService.create(name);
    res.status(201).json(newItem);
  } catch (error) {
    next(error);
  }
};

/**
 * @swagger
 * /api/items:
 *   get:
 *     tags: [Items]
 *     responses:
 *       200:
 *         description: List of items
 */
export const getItems = (req: Request, res: Response, next: NextFunction) => {
  try {
    const items = itemService.getAll();
    res.json(items);
  } catch (error) {
    next(error);
  }
};

/**
 * @swagger
 * /api/items/{id}:
 *   get:
 *     tags: [Items]
 *     parameters:
 *       - in: path
 *         name: id
 *         required: true
 *     responses:
 *       200:
 *         description: Item details
 */
export const getItemById = (req: Request, res: Response, next: NextFunction) => {
  try {
    if (!req.params.id) {
      res.status(400).json({ message: 'Item ID is required' });
      return;
    }
    const id = parseInt(req.params.id, 10);
    const item = itemService.getById(id);
    if (!item) {
      res.status(404).json({ message: 'Item not found' });
      return;
    }
    res.json(item);
  } catch (error) {
    next(error);
  }
};

/**
 * @swagger
 * /api/items/{id}:
 *   put:
 *     tags: [Items]
 *     parameters:
 *       - in: path
 *         name: id
 *         required: true
 *     requestBody:
 *       content:
 *         application/json:
 *           schema:
 *             properties:
 *               name:
 *                 type: string
 *     responses:
 *       200:
 *         description: Item updated
 */
export const updateItem = (req: Request, res: Response, next: NextFunction) => {
  try {
    if (!req.params.id) {
      res.status(400).json({ message: 'Item ID is required' });
      return;
    }
    const id = parseInt(req.params.id, 10);
    const { name } = req.body;
    const updatedItem = itemService.update(id, name);
    if (!updatedItem) {
      res.status(404).json({ message: 'Item not found' });
      return;
    }
    res.json(updatedItem);
  } catch (error) {
    next(error);
  }
};

/**
 * @swagger
 * /api/items/{id}:
 *   delete:
 *     tags: [Items]
 *     parameters:
 *       - in: path
 *         name: id
 *         required: true
 *     responses:
 *       200:
 *         description: Item deleted
 */
export const deleteItem = (req: Request, res: Response, next: NextFunction) => {
  try {
    if (!req.params.id) {
      res.status(400).json({ message: 'Item ID is required' });
      return;
    }
    const id = parseInt(req.params.id, 10);
    const deletedItem = itemService.delete(id);
    if (!deletedItem) {
      res.status(404).json({ message: 'Item not found' });
      return;
    }
    res.json(deletedItem);
  } catch (error) {
    next(error);
  }
};