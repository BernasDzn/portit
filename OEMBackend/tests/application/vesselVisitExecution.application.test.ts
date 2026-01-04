import request from 'supertest';
import { Express } from 'express';
import mongoose from 'mongoose';
import {
    createTestApp,
    connectTestDatabase,
    disconnectTestDatabase,
    clearTestDatabase
} from './setup';
import { VesselVisitExecutionModel } from '../../src/schemas/vesselVisitExecutionSchema';
import { OperationPlanModel } from '../../src/schemas/operationPlanSchema';
import { TaskCategoryModel } from '../../src/schemas/taskCategories';

describe('VesselVisitExecution Application Tests', () => {
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

    describe('GET /vessel-visit-executions/count', () => {
        it('should return 0 when no VVEs exist', async () => {
            const response = await request(app)
                .get('/vessel-visit-executions/count')
                .expect(200);

            expect(response.body.count).toBe(0);
        });

        it('should return correct count of VVEs', async () => {
            // Seed some VVEs
            await VesselVisitExecutionModel.insertMany([
                {
                    code: 'VVE-PORTO-1',
                    relatedVVN: 'VVN-001',
                    operationsExecuted: [],
                    status: 'Open',
                    dateOpen: new Date(),
                    createdBy: 'test@example.com'
                },
                {
                    code: 'VVE-PORTO-2',
                    relatedVVN: 'VVN-002',
                    operationsExecuted: [],
                    status: 'Open',
                    dateOpen: new Date(),
                    createdBy: 'test@example.com'
                }
            ]);

            const response = await request(app)
                .get('/vessel-visit-executions/count')
                .expect(200);

            expect(response.body.count).toBe(2);
        });
    });

    describe('GET /vessel-visit-executions', () => {
        beforeEach(async () => {
            // Seed test VVEs
            const vves = [
                {
                    code: 'VVE-PORTO-1',
                    relatedVVN: 'VVN-001',
                    operationsExecuted: [],
                    status: 'Open',
                    dateOpen: new Date('2024-01-15T08:00:00Z'),
                    createdBy: 'user1@example.com'
                },
                {
                    code: 'VVE-PORTO-2',
                    relatedVVN: 'VVN-002',
                    operationsExecuted: [],
                    status: 'Closed',
                    dateOpen: new Date('2024-01-14T08:00:00Z'),
                    dateClosed: new Date('2024-01-14T18:00:00Z'),
                    createdBy: 'user2@example.com'
                },
                {
                    code: 'VVE-PORTO-3',
                    relatedVVN: 'VVN-003',
                    operationsExecuted: [],
                    status: 'Open',
                    dateOpen: new Date('2024-01-16T08:00:00Z'),
                    createdBy: 'user1@example.com'
                }
            ];
            await VesselVisitExecutionModel.insertMany(vves);
        });

        it('should return paginated VVEs', async () => {
            const response = await request(app)
                .get('/vessel-visit-executions')
                .query({ pageNumber: 1, pageSize: 10 })
                .expect('Content-Type', /json/)
                .expect(200);

            expect(response.body).toHaveProperty('items');
            expect(response.body).toHaveProperty('pageNumber');
            expect(response.body).toHaveProperty('pageSize');
            expect(response.body.items).toHaveLength(3);
        });

        it('should filter VVEs by status', async () => {
            const response = await request(app)
                .get('/vessel-visit-executions')
                .query({ status: 'Closed', pageNumber: 1, pageSize: 10 })
                .expect(200);

            expect(response.body.items).toHaveLength(1);
            expect(response.body.items[0].status).toBe('Closed');
        });

        it('should filter VVEs by date range', async () => {
            const response = await request(app)
                .get('/vessel-visit-executions')
                .query({
                    dateStart: '2024-01-15T00:00:00Z',
                    dateEnd: '2024-01-15T23:59:59Z',
                    pageNumber: 1,
                    pageSize: 10
                })
                .expect(200);

            expect(response.body.items.length).toBeGreaterThanOrEqual(1);
        });

        it('should respect pagination limits', async () => {
            const response = await request(app)
                .get('/vessel-visit-executions')
                .query({ pageNumber: 1, pageSize: 2 })
                .expect(200);

            expect(response.body.items).toHaveLength(2);
        });
    });

    describe('GET /vessel-visit-executions/:relatedVVN', () => {
        beforeEach(async () => {
            await VesselVisitExecutionModel.create({
                code: 'VVE-GETTEST',
                relatedVVN: 'VVN-GETTEST',
                operationsExecuted: [],
                status: 'Open',
                dateOpen: new Date(),
                createdBy: 'test@example.com'
            });
        });

        it('should return VVE by relatedVVN', async () => {
            const response = await request(app)
                .get('/vessel-visit-executions/VVN-GETTEST')
                .expect('Content-Type', /json/)
                .expect(200);

            expect(response.body.code).toBe('VVE-GETTEST');
            expect(response.body.relatedVVN).toBe('VVN-GETTEST');
            expect(response.body.status).toBe('Open');
        });

        it('should return 500 for non-existent VVE', async () => {
            const response = await request(app)
                .get('/vessel-visit-executions/NON-EXISTENT')
                .expect(500);

            expect(response.body).toHaveProperty('message');
        });
    });

    describe('POST /vessel-visit-executions/:relatedVVN/open', () => {
        beforeEach(async () => {
            // First seed required task categories
            const categoryId = new mongoose.Types.ObjectId();
            await TaskCategoryModel.insertMany([
                { _id: categoryId, category: 'BERTH', name: 'Berthing', description: 'Vessel berthing' },
                { category: 'LOAD', name: 'Loading', description: 'Container loading' },
                { category: 'UNLOAD', name: 'Unloading', description: 'Container unloading' }
            ]);

            // Create an operation plan that is required to open a VVE
            await OperationPlanModel.create({
                relatedVVN: 'VVN-OPEN-TEST',
                date: new Date('2024-01-15'),
                dock: 'DOCK-001',
                operationSchedule: [],
                metadata: {
                    createdBy: 'test@example.com',
                    createdAt: new Date(),
                    algorithmUsed: 'greedy'
                }
            });
        });

        it('should open a VVE for an existing operation plan', async () => {
            const response = await request(app)
                .post('/vessel-visit-executions/VVN-OPEN-TEST/open')
                .expect(201);

            expect(response.body).toHaveProperty('code');
            expect(response.body.code).toMatch(/^VVE-PORTO-/);
            expect(response.body.relatedVVN).toBe('VVN-OPEN-TEST');
            expect(response.body.status).toBe('Open');

            // Verify it was persisted
            const persisted = await VesselVisitExecutionModel.findOne({ relatedVVN: 'VVN-OPEN-TEST' });
            expect(persisted).not.toBeNull();
        });

        it('should return error when operation plan does not exist', async () => {
            const response = await request(app)
                .post('/vessel-visit-executions/NON-EXISTENT-VVN/open')
                .expect(500);

            expect(response.body).toHaveProperty('message');
            expect(response.body.message).toContain('not found');
        });

        it('should return error when VVE was already opened for this VVN', async () => {
            // First, open it
            await request(app)
                .post('/vessel-visit-executions/VVN-OPEN-TEST/open')
                .expect(201);

            // Try to open again
            const response = await request(app)
                .post('/vessel-visit-executions/VVN-OPEN-TEST/open')
                .expect(500);

            expect(response.body).toHaveProperty('message');
            expect(response.body.message).toContain('already opened');
        });
    });

    describe('PUT /vessel-visit-executions/:relatedVVN/close', () => {
        beforeEach(async () => {
            // Create a VVE that is open with no pending operations
            await VesselVisitExecutionModel.create({
                code: 'VVE-CLOSE-TEST',
                relatedVVN: 'VVN-CLOSE-TEST',
                operationsExecuted: [], // No pending operations
                status: 'Open',
                dateOpen: new Date('2024-01-15T08:00:00Z'),
                createdBy: 'test@example.com'
            });
        });

        it('should close an open VVE', async () => {
            const response = await request(app)
                .put('/vessel-visit-executions/VVN-CLOSE-TEST/close')
                .expect(200);

            expect(response.body.status).toBe('Closed');
            expect(response.body.dateClosed).toBeDefined();

            // Verify persisted
            const persisted = await VesselVisitExecutionModel.findOne({ relatedVVN: 'VVN-CLOSE-TEST' });
            expect(persisted?.status).toBe('Closed');
        });

        it('should return error when VVE does not exist', async () => {
            const response = await request(app)
                .put('/vessel-visit-executions/NON-EXISTENT-VVN/close')
                .expect(500);

            expect(response.body).toHaveProperty('message');
        });

        it('should return error when VVE is already closed', async () => {
            // First close it
            await request(app)
                .put('/vessel-visit-executions/VVN-CLOSE-TEST/close')
                .expect(200);

            // Try to close again
            const response = await request(app)
                .put('/vessel-visit-executions/VVN-CLOSE-TEST/close')
                .expect(500);

            expect(response.body).toHaveProperty('message');
            expect(response.body.message).toContain('already closed');
        });
    });

    describe('PUT /vessel-visit-executions/:relatedVVN/berth', () => {
        beforeEach(async () => {
            await VesselVisitExecutionModel.create({
                code: 'VVE-BERTH-TEST',
                relatedVVN: 'VVN-BERTH-TEST',
                operationsExecuted: [],
                status: 'Open',
                dateOpen: new Date(),
                createdBy: 'test@example.com'
            });
        });

        it('should update berth details of a VVE', async () => {
            const berthData = {
                dock: 'DOCK-A1',
                berthTime: new Date('2024-01-15T10:00:00Z').toISOString()
            };

            const response = await request(app)
                .put('/vessel-visit-executions/VVN-BERTH-TEST/berth')
                .send(berthData)
                .expect(200);

            expect(response.body.dock).toBe(berthData.dock);
            expect(response.body.berthTime).toBeDefined();
        });

        it('should return error for non-existent VVE', async () => {
            const berthData = {
                dock: 'DOCK-A1',
                berthTime: new Date().toISOString()
            };

            const response = await request(app)
                .put('/vessel-visit-executions/NON-EXISTENT-VVN/berth')
                .send(berthData)
                .expect(500);

            expect(response.body).toHaveProperty('message');
        });
    });
});
