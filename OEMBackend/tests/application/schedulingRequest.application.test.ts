import request from 'supertest';
import { Express } from 'express';
import mongoose from 'mongoose';
import {
    createTestApp,
    connectTestDatabase,
    disconnectTestDatabase,
    clearTestDatabase
} from './setup';
import { ScheduleQueue } from '../../src/schemas/scheduleQueue';

describe('SchedulingRequest Application Tests', () => {
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

    describe('GET /schedule/request', () => {
        it('should queue a scheduling request successfully', async () => {
            const response = await request(app)
                .get('/schedule/request')
                .query({
                    day: '2026-01-20',
                    alg: 'greedy',
                    daysAhead: 1
                })
                .expect('Content-Type', /json/)
                .expect(200);

            expect(response.body).toHaveProperty('message');
            expect(response.body.message).toContain('queued');
            expect(response.body.message).toContain('number');

            // Verify it was persisted in the queue
            const queueItems = await ScheduleQueue.find({});
            expect(queueItems.length).toBeGreaterThanOrEqual(1);
            
            const lastItem = queueItems[queueItems.length - 1];
            expect(lastItem.requestData.day).toBe('2026-01-20');
            expect(lastItem.requestData.alg).toBe('greedy');
        });

        it('should return 400 when missing required parameter: day', async () => {
            const response = await request(app)
                .get('/schedule/request')
                .query({
                    alg: 'greedy',
                    daysAhead: 1
                })
                .expect(400);

            expect(response.body).toHaveProperty('message');
            expect(response.body.message).toContain('Missing required query parameters');
        });

        it('should return 400 when missing required parameter: alg', async () => {
            const response = await request(app)
                .get('/schedule/request')
                .query({
                    day: '2026-01-20',
                    daysAhead: 1
                })
                .expect(400);

            expect(response.body).toHaveProperty('message');
            expect(response.body.message).toContain('Missing required query parameters');
        });

        it('should handle different algorithms', async () => {
            const algorithms = ['greedy', 'genetic', 'heuristic'];

            for (const algorithm of algorithms) {
                const response = await request(app)
                    .get('/schedule/request')
                    .query({
                        day: '2026-01-21',
                        alg: algorithm,
                        daysAhead: 1
                    })
                    .expect(200);

                expect(response.body).toHaveProperty('message');
            }

            // Verify all were queued
            const queueItems = await ScheduleQueue.find({});
            expect(queueItems.length).toBe(algorithms.length);
        });

        it('should increment queue position for multiple requests', async () => {
            // First request
            const response1 = await request(app)
                .get('/schedule/request')
                .query({
                    day: '2026-01-20',
                    alg: 'greedy',
                    daysAhead: 1
                })
                .expect(200);

            expect(response1.body.message).toContain('number 1');

            // Second request
            const response2 = await request(app)
                .get('/schedule/request')
                .query({
                    day: '2026-01-21',
                    alg: 'genetic',
                    daysAhead: 1
                })
                .expect(200);

            expect(response2.body.message).toContain('number 2');
        });

        it('should default daysAhead parameter when not provided', async () => {
            const response = await request(app)
                .get('/schedule/request')
                .query({
                    day: '2026-01-20',
                    alg: 'greedy'
                })
                .expect(200);

            expect(response.body).toHaveProperty('message');
            
            // Verify default daysAhead is set to 1 (as per controller logic)
            const queueItem = await ScheduleQueue.findOne({});
            expect(queueItem?.requestData.daysAhead).toBe(1);
        });
    });

    describe('GET /schedule/queueState', () => {
        beforeEach(async () => {
            // Seed test queue items
            await ScheduleQueue.insertMany([
                {
                    requestData: {
                        day: '2026-01-20',
                        alg: 'greedy',
                        daysAhead: 1
                    },
                    status: 'pending',
                    priority: 0,
                    issuer: 'user1@example.com',
                    requestedAt: new Date('2026-01-04T10:00:00Z')
                },
                {
                    requestData: {
                        day: '2026-01-21',
                        alg: 'genetic',
                        daysAhead: 1
                    },
                    status: 'in-progress',
                    priority: 0,
                    issuer: 'user2@example.com',
                    requestedAt: new Date('2026-01-04T10:05:00Z')
                },
                {
                    requestData: {
                        day: '2026-01-22',
                        alg: 'heuristic',
                        daysAhead: 1
                    },
                    status: 'completed',
                    priority: 0,
                    issuer: 'user3@example.com',
                    requestedAt: new Date('2026-01-04T10:10:00Z'),
                    result: {
                        data: [],
                        date: '2026-01-22',
                        metrics: []
                    }
                }
            ]);
        });

        it('should return the current queue state', async () => {
            const response = await request(app)
                .get('/schedule/queueState')
                .expect('Content-Type', /json/)
                .expect(200);

            expect(Array.isArray(response.body)).toBe(true);
            expect(response.body.length).toBe(3);
        });

        it('should include all queue item properties', async () => {
            const response = await request(app)
                .get('/schedule/queueState')
                .expect(200);

            const queueItem = response.body[0];
            expect(queueItem).toHaveProperty('requestData');
            expect(queueItem).toHaveProperty('status');
            expect(queueItem).toHaveProperty('issuer');
            expect(queueItem).toHaveProperty('requestedAt');
        });

        it('should show different statuses correctly', async () => {
            const response = await request(app)
                .get('/schedule/queueState')
                .expect(200);

            const statuses = response.body.map((item: any) => item.status);
            expect(statuses).toContain('pending');
            expect(statuses).toContain('in-progress');
            expect(statuses).toContain('completed');
        });

        it('should return empty array when queue is empty', async () => {
            await clearTestDatabase();

            const response = await request(app)
                .get('/schedule/queueState')
                .expect(200);

            expect(Array.isArray(response.body)).toBe(true);
            expect(response.body.length).toBe(0);
        });
    });

    describe('POST /schedule/acceptRequest', () => {
        let completedRequestId: string;
        const testIssuer = 'test@example.com';

        beforeEach(async () => {
            // Create a completed scheduling request
            const completedRequest = await ScheduleQueue.create({
                requestData: {
                    day: '2026-01-20',
                    alg: 'greedy',
                    daysAhead: 1
                },
                status: 'completed',
                priority: 0,
                issuer: testIssuer,
                requestedAt: new Date(),
                result: {
                    data: [
                        {
                            dock: 'D1',
                            schedule: [
                                {
                                    name: 'VVN-001',
                                    cranes: ['Crane-1'],
                                    unloading_enter_time: 8,
                                    unloading_exit_time: 10,
                                    loading_enter_time: 10,
                                    loading_exit_time: 12
                                }
                            ]
                        }
                    ],
                    date: '2026-01-20',
                    metrics: [
                        {
                            algorithm: 'greedy',
                            computationTime: 1.5,
                            selection: {
                                auto: true,
                                reason: 'Best performance'
                            },
                            strategy: 'minimize-delay',
                            totalDelay: 0,
                            vesselCount: 1
                        }
                    ]
                }
            });
            completedRequestId = completedRequest._id.toString();
        });

        it('should return 400 when missing request ID', async () => {
            const response = await request(app)
                .post('/schedule/acceptRequest')
                .expect(400);

            expect(response.body).toHaveProperty('message');
            expect(response.body.message).toContain('Missing required query parameter');
        });

        it('should return 500 when accepting non-existent request', async () => {
            const fakeId = new mongoose.Types.ObjectId().toString();

            const response = await request(app)
                .post('/schedule/acceptRequest')
                .query({ id: fakeId })
                .expect(500);

            expect(response.body).toHaveProperty('message');
        });

        it('should return error when accepting non-completed request', async () => {
            // Create a pending request
            const pendingRequest = await ScheduleQueue.create({
                requestData: {
                    day: '2026-01-21',
                    alg: 'genetic',
                    daysAhead: 1
                },
                status: 'pending',
                priority: 0,
                issuer: testIssuer,
                requestedAt: new Date()
            });

            const response = await request(app)
                .post('/schedule/acceptRequest')
                .query({ id: pendingRequest._id.toString() })
                .expect(500);

            expect(response.body).toHaveProperty('message');
            expect(response.body.message).toContain('not completed');
        });
    });

    describe('POST /schedule/rejectRequest', () => {
        let completedRequestId: string;
        const testIssuer = 'test@example.com';

        beforeEach(async () => {
            // Create a completed scheduling request
            const completedRequest = await ScheduleQueue.create({
                requestData: {
                    day: '2026-01-20',
                    alg: 'greedy',
                    daysAhead: 1
                },
                status: 'completed',
                priority: 0,
                issuer: testIssuer,
                requestedAt: new Date(),
                result: {
                    data: [
                        {
                            dock: 'D1',
                            schedule: []
                        }
                    ],
                    date: '2026-01-20',
                    metrics: []
                }
            });
            completedRequestId = completedRequest._id.toString();
        });

        it('should return 400 when missing request ID', async () => {
            const response = await request(app)
                .post('/schedule/rejectRequest')
                .expect(400);

            expect(response.body).toHaveProperty('message');
            expect(response.body.message).toContain('Missing required query parameter');
        });

        it('should return error when rejecting non-existent request', async () => {
            const fakeId = new mongoose.Types.ObjectId().toString();

            const response = await request(app)
                .post('/schedule/rejectRequest')
                .query({ id: fakeId })
                .expect(500);

            expect(response.body).toHaveProperty('message');
        });

        it('should return error when rejecting non-completed request', async () => {
            // Create a pending request
            const pendingRequest = await ScheduleQueue.create({
                requestData: {
                    day: '2026-01-21',
                    alg: 'genetic',
                    daysAhead: 1
                },
                status: 'in-progress',
                priority: 0,
                issuer: testIssuer,
                requestedAt: new Date()
            });

            const response = await request(app)
                .post('/schedule/rejectRequest')
                .query({ id: pendingRequest._id.toString() })
                .expect(500);

            expect(response.body).toHaveProperty('message');
            expect(response.body.message).toContain('not yet completed');
        });
    });

    describe('Integration: Complete scheduling workflow', () => {
        it('should handle full scheduling lifecycle: request -> check state -> complete', async () => {
            // Step 1: Create a scheduling request
            const requestResponse = await request(app)
                .get('/schedule/request')
                .query({
                    day: '2026-01-25',
                    alg: 'greedy',
                    daysAhead: 1
                })
                .expect(200);

            expect(requestResponse.body.message).toContain('queued');

            // Step 2: Check queue state
            const queueStateResponse = await request(app)
                .get('/schedule/queueState')
                .expect(200);

            expect(queueStateResponse.body.length).toBeGreaterThanOrEqual(1);
            const queuedItem = queueStateResponse.body.find(
                (item: any) => item.requestData.day === '2026-01-25'
            );
            expect(queuedItem).toBeDefined();
            expect(queuedItem.status).toBe('pending');

            // Step 3: Simulate completion by updating status
            const requestId = queuedItem._id;
            await ScheduleQueue.findByIdAndUpdate(requestId, {
                status: 'completed',
                result: {
                    data: [],
                    date: '2026-01-25',
                    metrics: []
                }
            });

            // Step 4: Verify status changed
            const updatedStateResponse = await request(app)
                .get('/schedule/queueState')
                .expect(200);

            const completedItem = updatedStateResponse.body.find(
                (item: any) => item._id === requestId
            );
            expect(completedItem.status).toBe('completed');
        });

        it('should maintain correct queue order for multiple requests', async () => {
            const days = ['2026-01-20', '2026-01-21', '2026-01-22'];

            // Create multiple requests
            for (const day of days) {
                await request(app)
                    .get('/schedule/request')
                    .query({
                        day: day,
                        alg: 'greedy',
                        daysAhead: 1
                    })
                    .expect(200);
            }

            // Check queue state
            const response = await request(app)
                .get('/schedule/queueState')
                .expect(200);

            expect(response.body.length).toBe(3);
            
            // Verify order by checking requestedAt timestamps
            const timestamps = response.body.map((item: any) => new Date(item.requestedAt).getTime());
            for (let i = 1; i < timestamps.length; i++) {
                expect(timestamps[i]).toBeGreaterThanOrEqual(timestamps[i - 1]);
            }
        });
    });

    describe('Error Handling', () => {
        it('should handle invalid date format gracefully', async () => {
            const response = await request(app)
                .get('/schedule/request')
                .query({
                    day: 'invalid-date',
                    alg: 'greedy',
                    daysAhead: 1
                })
                .expect(200);

            // Should still queue (validation happens later in the scheduling service)
            expect(response.body).toHaveProperty('message');
        });

        it('should handle very large daysAhead values', async () => {
            const response = await request(app)
                .get('/schedule/request')
                .query({
                    day: '2026-01-20',
                    alg: 'greedy',
                    daysAhead: 1000
                })
                .expect(200);

            expect(response.body).toHaveProperty('message');
            
            // Verify it was capped to 1 as per controller logic
            const queueItem = await ScheduleQueue.findOne({});
            expect(queueItem?.requestData.daysAhead).toBe(1);
        });

        it('should handle unknown algorithm names', async () => {
            const response = await request(app)
                .get('/schedule/request')
                .query({
                    day: '2026-01-20',
                    alg: 'unknown-algorithm',
                    daysAhead: 1
                })
                .expect(200);

            // Should still queue (validation happens in scheduling service)
            expect(response.body).toHaveProperty('message');
        });
    });
});
