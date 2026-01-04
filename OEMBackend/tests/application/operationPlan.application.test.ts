import request from 'supertest';
import { Express } from 'express';
import mongoose from 'mongoose';
import {
    createTestApp,
    connectTestDatabase,
    disconnectTestDatabase,
    clearTestDatabase
} from './setup';
import { OperationPlanModel } from '../../src/schemas/operationPlanSchema';
import { TaskCategoryModel } from '../../src/schemas/taskCategories';

describe('OperationPlan Application Tests', () => {
    let app: Express;
    let testTaskCategoryId: mongoose.Types.ObjectId;
    let testTaskCategoryCode: string;

    beforeAll(async () => {
        await connectTestDatabase();
        app = createTestApp();
    });

    afterAll(async () => {
        await disconnectTestDatabase();
    });

    beforeEach(async () => {
        await clearTestDatabase();
        
        // Create a required task category for operations
        testTaskCategoryId = new mongoose.Types.ObjectId();
        testTaskCategoryCode = 'LOAD';
        await TaskCategoryModel.create({
            _id: testTaskCategoryId,
            category: testTaskCategoryCode,
            name: 'Loading',
            description: 'Container loading operations'
        });
    });

    describe('GET /operation-plans', () => {
        beforeEach(async () => {
            // Seed test operation plans
            const plans = [
                {
                    _id: new mongoose.Types.ObjectId(),
                    date: new Date('2026-01-15T00:00:00Z'),
                    dock: 'D1',
                    relatedVVN: 'VVN-001',
                    operationSchedule: [
                        {
                            operationType: testTaskCategoryId,
                            startTime: new Date('2026-01-15T08:00:00Z'),
                            endTime: new Date('2026-01-15T10:00:00Z'),
                            resources: [
                                { name: 'Crane-1', type: 'Crane' }
                            ],
                            payload: {
                                containerId: 'CONT-001',
                                storageLocation: 'A1'
                            }
                        }
                    ],
                    metadata: {
                        createdBy: 'test@example.com',
                        createdAt: new Date('2026-01-14T00:00:00Z'),
                        algorithmUsed: 'greedy'
                    }
                },
                {
                    _id: new mongoose.Types.ObjectId(),
                    date: new Date('2026-01-16T00:00:00Z'),
                    dock: 'D2',
                    relatedVVN: 'VVN-002',
                    operationSchedule: [
                        {
                            operationType: testTaskCategoryId,
                            startTime: new Date('2026-01-16T09:00:00Z'),
                            endTime: new Date('2026-01-16T11:00:00Z'),
                            resources: [
                                { name: 'Crane-2', type: 'Crane' }
                            ],
                            payload: {
                                containerId: 'CONT-002',
                                storageLocation: 'B1'
                            }
                        }
                    ],
                    metadata: {
                        createdBy: 'test@example.com',
                        createdAt: new Date('2026-01-15T00:00:00Z'),
                        algorithmUsed: 'genetic'
                    }
                },
                {
                    _id: new mongoose.Types.ObjectId(),
                    date: new Date('2026-01-17T00:00:00Z'),
                    dock: 'D1',
                    relatedVVN: 'VVN-003',
                    operationSchedule: [],
                    metadata: {
                        createdBy: 'test@example.com',
                        createdAt: new Date('2026-01-16T00:00:00Z'),
                        algorithmUsed: 'greedy'
                    }
                }
            ];
            await OperationPlanModel.insertMany(plans);
        });

        it('should return paginated operation plans', async () => {
            const response = await request(app)
                .get('/operation-plans')
                .query({ pageNumber: 1, pageSize: 10 })
                .expect('Content-Type', /json/)
                .expect(200);

            expect(response.body).toHaveProperty('items');
            expect(response.body).toHaveProperty('pageNumber');
            expect(response.body).toHaveProperty('pageSize');
            expect(response.body).toHaveProperty('pageCount');
            expect(response.body.items).toHaveLength(3);
        });

        it('should filter operation plans by start date', async () => {
            const response = await request(app)
                .get('/operation-plans')
                .query({ 
                    startDate: '2026-01-16T00:00:00Z',
                    pageNumber: 1, 
                    pageSize: 10 
                })
                .expect(200);

            expect(response.body.items.length).toBeGreaterThanOrEqual(1);
            response.body.items.forEach((plan: any) => {
                expect(new Date(plan.date).getTime()).toBeGreaterThanOrEqual(new Date('2026-01-16T00:00:00Z').getTime());
            });
        });

        it('should filter operation plans by end date', async () => {
            const response = await request(app)
                .get('/operation-plans')
                .query({ 
                    endDate: '2026-01-16T00:00:00Z',
                    pageNumber: 1, 
                    pageSize: 10 
                })
                .expect(200);

            expect(response.body.items.length).toBeGreaterThanOrEqual(1);
            response.body.items.forEach((plan: any) => {
                expect(new Date(plan.date).getTime()).toBeLessThanOrEqual(new Date('2026-01-16T00:00:00Z').getTime());
            });
        });

        it('should respect pagination limits', async () => {
            const response = await request(app)
                .get('/operation-plans')
                .query({ pageNumber: 1, pageSize: 2 })
                .expect(200);

            expect(response.body.items).toHaveLength(2);
        });
    });

    describe('GET /operation-plans/by-date', () => {
        beforeEach(async () => {
            // Seed test operation plans with specific dates
            const plans = [
                {
                    _id: new mongoose.Types.ObjectId(),
                    date: new Date('2026-01-15T00:00:00Z'),
                    dock: 'D1',
                    relatedVVN: 'VVN-001',
                    operationSchedule: [],
                    metadata: {
                        createdBy: 'test@example.com',
                        createdAt: new Date(),
                        algorithmUsed: 'greedy'
                    }
                },
                {
                    _id: new mongoose.Types.ObjectId(),
                    date: new Date('2026-01-15T00:00:00Z'),
                    dock: 'D2',
                    relatedVVN: 'VVN-002',
                    operationSchedule: [],
                    metadata: {
                        createdBy: 'test@example.com',
                        createdAt: new Date(),
                        algorithmUsed: 'greedy'
                    }
                },
                {
                    _id: new mongoose.Types.ObjectId(),
                    date: new Date('2026-01-16T00:00:00Z'),
                    dock: 'D1',
                    relatedVVN: 'VVN-003',
                    operationSchedule: [],
                    metadata: {
                        createdBy: 'test@example.com',
                        createdAt: new Date(),
                        algorithmUsed: 'genetic'
                    }
                }
            ];
            await OperationPlanModel.insertMany(plans);
        });

        it('should return operation plans grouped by date', async () => {
            const response = await request(app)
                .get('/operation-plans/by-date')
                .expect('Content-Type', /json/)
                .expect(200);

            expect(Array.isArray(response.body)).toBe(true);
            expect(response.body.length).toBeGreaterThanOrEqual(2);
            
            // Check structure
            response.body.forEach((group: any) => {
                expect(group).toHaveProperty('date');
                expect(group).toHaveProperty('plans');
                expect(Array.isArray(group.plans)).toBe(true);
            });

            // Verify grouping
            const jan15Group = response.body.find((g: any) => g.date === '2026-01-15');
            if (jan15Group) {
                expect(jan15Group.plans.length).toBeGreaterThanOrEqual(2);
            }
        });
    });

    describe('GET /operation-plans/:id', () => {
        let testPlanId: string;

        beforeEach(async () => {
            const plan = await OperationPlanModel.create({
                date: new Date('2026-01-15T00:00:00Z'),
                dock: 'D1',
                relatedVVN: 'VVN-001',
                operationSchedule: [
                    {
                        operationType: testTaskCategoryId,
                        startTime: new Date('2026-01-15T08:00:00Z'),
                        endTime: new Date('2026-01-15T10:00:00Z'),
                        resources: [
                            { name: 'Crane-1', type: 'Crane' }
                        ],
                        payload: {
                            containerId: 'CONT-001',
                            storageLocation: 'A1'
                        }
                    }
                ],
                metadata: {
                    createdBy: 'test@example.com',
                    createdAt: new Date(),
                    algorithmUsed: 'greedy'
                }
            });
            testPlanId = plan._id.toString();
        });

        it('should return operation plan by ID', async () => {
            const response = await request(app)
                .get(`/operation-plans/${testPlanId}`)
                .expect('Content-Type', /json/)
                .expect(200);

            expect(response.body.id).toBe(testPlanId);
            expect(response.body.dock).toBe('D1');
            expect(response.body.relatedVVN).toBe('VVN-001');
            expect(response.body.operationSchedule).toHaveLength(1);
        });

        it('should return 404 for non-existent operation plan ID', async () => {
            const fakeId = new mongoose.Types.ObjectId().toString();
            const response = await request(app)
                .get(`/operation-plans/${fakeId}`)
                .expect(404);

            expect(response.body).toHaveProperty('message');
        });

        it('should return 500 for invalid operation plan ID format', async () => {
            const response = await request(app)
                .get('/operation-plans/invalid-id')
                .expect(500);

            expect(response.body).toHaveProperty('message');
        });
    });

    describe('PATCH /operation-plans/:id', () => {
        let testPlanId: string;

        beforeEach(async () => {
            const plan = await OperationPlanModel.create({
                date: new Date('2026-01-15T00:00:00Z'),
                dock: 'D1',
                relatedVVN: 'VVN-001',
                operationSchedule: [
                    {
                        operationType: testTaskCategoryId,
                        startTime: new Date('2026-01-15T08:00:00Z'),
                        endTime: new Date('2026-01-15T10:00:00Z'),
                        resources: [
                            { name: 'Crane-1', type: 'Crane' }
                        ],
                        payload: {
                            containerId: 'CONT-001',
                            storageLocation: 'A1'
                        }
                    }
                ],
                metadata: {
                    createdBy: 'test@example.com',
                    createdAt: new Date(),
                    algorithmUsed: 'greedy'
                }
            });
            testPlanId = plan._id.toString();
        });

        it('should update operation plan dock', async () => {
            const updates = {
                dock: 'D2'
            };

            const response = await request(app)
                .patch(`/operation-plans/${testPlanId}`)
                .send(updates)
                .expect('Content-Type', /json/)
                .expect(200);

            expect(response.body.dock).toBe('D2');

            // Verify it was actually updated in the database
            const updated = await OperationPlanModel.findById(testPlanId);
            expect(updated?.dock).toBe('D2');
        });

        it('should update operation plan relatedVVN', async () => {
            const updates = {
                relatedVVN: 'VVN-UPDATED'
            };

            const response = await request(app)
                .patch(`/operation-plans/${testPlanId}`)
                .send(updates)
                .expect(200);

            expect(response.body.relatedVVN).toBe('VVN-UPDATED');
        });

        it('should return 500 when updating non-existent plan', async () => {
            const fakeId = new mongoose.Types.ObjectId().toString();
            const updates = { dock: 'D2' };

            const response = await request(app)
                .patch(`/operation-plans/${fakeId}`)
                .send(updates)
                .expect(500);

            expect(response.body).toHaveProperty('message');
        });
    });

    describe('POST /operation-plans/regenerate', () => {
        it('should queue regeneration request', async () => {
            const regenerateRequest = {
                day: '2026-01-20',
                algorithm: 'greedy',
                daysAhead: 1
            };

            const response = await request(app)
                .post('/operation-plans/regenerate')
                .set('Authorization', 'Bearer test-token')
                .send(regenerateRequest)
                .expect('Content-Type', /json/)
                .expect(200);

            expect(response.body).toHaveProperty('message');
            expect(response.body.message).toContain('queued');
        });

        it('should return 400 when missing required parameters', async () => {
            const incompleteRequest = {
                algorithm: 'greedy'
                // missing 'day'
            };

            const response = await request(app)
                .post('/operation-plans/regenerate')
                .send(incompleteRequest)
                .expect(400);

            expect(response.body).toHaveProperty('message');
        });

        it('should handle different algorithms', async () => {
            const algorithms = ['greedy', 'genetic', 'heuristic'];

            for (const algorithm of algorithms) {
                const regenerateRequest = {
                    day: '2026-01-20',
                    algorithm: algorithm,
                    daysAhead: 1
                };

                const response = await request(app)
                    .post('/operation-plans/regenerate')
                    .set('Authorization', 'Bearer test-token')
                    .send(regenerateRequest)
                    .expect(200);

                expect(response.body).toHaveProperty('message');
            }
        });
    });

    describe('Integration: Full workflow', () => {
        it('should create, retrieve, and update an operation plan', async () => {
            // Create operation plan data
            const planData = {
                date: new Date('2026-01-15T00:00:00Z'),
                dock: 'D1',
                relatedVVN: 'VVN-WORKFLOW',
                operationSchedule: [
                    {
                        operationType: testTaskCategoryId,
                        startTime: new Date('2026-01-15T08:00:00Z'),
                        endTime: new Date('2026-01-15T10:00:00Z'),
                        resources: [
                            { name: 'Crane-1', type: 'Crane' }
                        ],
                        payload: {
                            containerId: 'CONT-WORKFLOW',
                            storageLocation: 'A1'
                        }
                    }
                ],
                metadata: {
                    createdBy: 'workflow@example.com',
                    createdAt: new Date(),
                    algorithmUsed: 'greedy'
                }
            };

            // Create the plan directly in database for this test
            const created = await OperationPlanModel.create(planData);
            const planId = created._id.toString();

            // Retrieve the plan
            const getResponse = await request(app)
                .get(`/operation-plans/${planId}`)
                .expect(200);

            expect(getResponse.body.id).toBe(planId);
            expect(getResponse.body.dock).toBe('D1');

            // Update the plan
            const updateResponse = await request(app)
                .patch(`/operation-plans/${planId}`)
                .send({ dock: 'D3' })
                .expect(200);

            expect(updateResponse.body.dock).toBe('D3');

            // Verify final state
            const finalPlan = await OperationPlanModel.findById(planId);
            expect(finalPlan?.dock).toBe('D3');
        });
    });
});
