import request from 'supertest';
import { Express } from 'express';
import {
    createTestApp,
    connectTestDatabase,
    disconnectTestDatabase,
    clearTestDatabase
} from './setup';
import { TaskCategoryModel } from '../../src/schemas/taskCategories';
import { CreateTaskCategoryDto } from '../../src/dto/taskCategoryDto';

describe('TaskCategory Application Tests', () => {
    let app: Express;

    beforeAll(async () => {
        await connectTestDatabase();
        app = createTestApp();
    });

    afterAll(async () => {
        await disconnectTestDatabase();
    });

    beforeEach(async () => {
        await clearTestDatabase();
    });

    describe('POST /task-categories', () => {
        it('should create a new task category and persist it to the database', async () => {
            const newCategory: CreateTaskCategoryDto = {
                category: 'MAINT',
                name: 'Maintenance',
                description: 'Maintenance operations for port equipment'
            };

            const response = await request(app)
                .post('/task-categories')
                .send(newCategory)
                .expect('Content-Type', /json/)
                .expect(201);

            expect(response.body.category).toBe(newCategory.category);
            expect(response.body.name).toBe(newCategory.name);
            expect(response.body.description).toBe(newCategory.description);

            // Verify it was actually persisted
            const persisted = await TaskCategoryModel.findOne({ category: newCategory.category });
            expect(persisted).not.toBeNull();
            expect(persisted?.name).toBe(newCategory.name);
        });

        it('should return error when creating duplicate category code', async () => {
            const category: CreateTaskCategoryDto = {
                category: 'UNIQUE',
                name: 'First Category',
                description: 'First description'
            };

            // Create first category
            await request(app)
                .post('/task-categories')
                .send(category)
                .expect(201);

            // Try to create duplicate
            const duplicateCategory: CreateTaskCategoryDto = {
                category: 'UNIQUE',
                name: 'Duplicate Category',
                description: 'Duplicate description'
            };

            const response = await request(app)
                .post('/task-categories')
                .send(duplicateCategory)
                .expect(500);

            expect(response.body).toHaveProperty('message');
        });
    });

    describe('GET /task-categories', () => {
        beforeEach(async () => {
            // Seed test data
            const categories = [
                { category: 'BERTH', name: 'Berthing', description: 'Vessel berthing procedures' },
                { category: 'LOAD', name: 'Loading', description: 'Container loading procedures' },
                { category: 'UNLOAD', name: 'Unloading', description: 'Container unloading procedures' }
            ];
            await TaskCategoryModel.insertMany(categories);
        });

        it('should return all task categories with pagination', async () => {
            const response = await request(app)
                .get('/task-categories')
                .query({ pageNumber: 0, pageSize: 10 })
                .expect('Content-Type', /json/)
                .expect(200);

            expect(response.body).toHaveProperty('items');
            expect(response.body).toHaveProperty('pageNumber');
            expect(response.body).toHaveProperty('pageSize');
            expect(response.body.items).toHaveLength(3);
        });

        it('should filter task categories by name', async () => {
            const response = await request(app)
                .get('/task-categories')
                .query({ name: 'Load', pageNumber: 0, pageSize: 10 })
                .expect(200);

            // Should match 'Loading' and potentially 'Unloading'
            expect(response.body.items.length).toBeGreaterThanOrEqual(1);
        });

        it('should respect pagination limits', async () => {
            const response = await request(app)
                .get('/task-categories')
                .query({ pageNumber: 0, pageSize: 2 })
                .expect(200);

            expect(response.body.items).toHaveLength(2);
        });
    });

    describe('GET /task-categories/code/:code', () => {
        beforeEach(async () => {
            await TaskCategoryModel.create({
                category: 'INSPECT',
                name: 'Inspection',
                description: 'Cargo inspection procedures'
            });
        });

        it('should return task category by code', async () => {
            const response = await request(app)
                .get('/task-categories/code/INSPECT')
                .expect('Content-Type', /json/)
                .expect(200);

            expect(response.body.category).toBe('INSPECT');
            expect(response.body.name).toBe('Inspection');
        });

        it('should return 404 for non-existent category code', async () => {
            const response = await request(app)
                .get('/task-categories/code/NOEXIST')
                .expect(404);

            expect(response.body).toHaveProperty('message');
        });
    });

    describe('PUT /task-categories/:id', () => {
        beforeEach(async () => {
            await TaskCategoryModel.create({
                category: 'UPD_TEST',
                name: 'Original Name',
                description: 'Original description'
            });
        });

        it('should update an existing task category', async () => {
            const updateData: CreateTaskCategoryDto = {
                category: 'UPD_TEST',
                name: 'Updated Name',
                description: 'Updated description'
            };

            const response = await request(app)
                .put('/task-categories/UPD_TEST')
                .send(updateData)
                .expect('Content-Type', /json/)
                .expect(200);

            expect(response.body.name).toBe(updateData.name);
            expect(response.body.description).toBe(updateData.description);

            // Verify persisted update
            const persisted = await TaskCategoryModel.findOne({ category: 'UPD_TEST' });
            expect(persisted?.name).toBe(updateData.name);
        });

        it('should return error when updating non-existent category', async () => {
            const updateData: CreateTaskCategoryDto = {
                category: 'NOEXIST',
                name: 'New Name',
                description: 'New description'
            };

            const response = await request(app)
                .put('/task-categories/NOEXIST')
                .send(updateData)
                .expect(500);

            expect(response.body).toHaveProperty('message');
        });
    });
});
