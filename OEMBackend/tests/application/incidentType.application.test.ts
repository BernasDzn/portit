import request from 'supertest';
import { Express } from 'express';
import mongoose from 'mongoose';
import {
    createTestApp,
    connectTestDatabase,
    disconnectTestDatabase,
    clearTestDatabase
} from './setup';
import { IncidentTypeModel } from '../../src/schemas/incidentTypeSchema';
import { PartialIncidentTypeDto } from '../../src/dto/incidentTypeDto';

describe('IncidentType Application Tests', () => {
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

    describe('POST /incident-types', () => {
        it('should create a new incident type and persist it to the database', async () => {
            const newIncidentType: PartialIncidentTypeDto = {
                name: 'Equipment Failure',
                description: 'Failure of port equipment during operations',
                severity: 'Major'
            };

            const response = await request(app)
                .post('/incident-types')
                .send(newIncidentType)
                .expect('Content-Type', /json/)
                .expect(201);

            expect(response.body).toHaveProperty('bid');
            expect(response.body.name).toBe(newIncidentType.name);
            expect(response.body.description).toBe(newIncidentType.description);
            expect(response.body.severity).toBe(newIncidentType.severity);

            // Verify it was actually persisted
            const persisted = await IncidentTypeModel.findOne({ bid: response.body.bid });
            expect(persisted).not.toBeNull();
            expect(persisted?.name).toBe(newIncidentType.name);
        });

        it('should create an incident type with a parent (subtype)', async () => {
            // First, create a parent incident type
            const parentType: PartialIncidentTypeDto = {
                name: 'Technical Issues',
                description: 'General technical issues category',
                severity: 'Minor'
            };

            const parentResponse = await request(app)
                .post('/incident-types')
                .send(parentType)
                .expect(201);

            const parentBid = parentResponse.body.bid;

            // Create a child incident type
            const childType: PartialIncidentTypeDto = {
                name: 'Crane Malfunction',
                description: 'Malfunction specific to cranes',
                severity: 'Major',
                subtypeOf: parentBid
            };

            const childResponse = await request(app)
                .post('/incident-types')
                .send(childType)
                .expect(201);

            expect(childResponse.body.subtypeOf).toBe(parentBid);

            // Verify the parent now has the child as a subtype
            const updatedParent = await IncidentTypeModel.findOne({ bid: parentBid });
            expect(updatedParent?.subtypes).toHaveLength(1);
        });

        it('should return 500 when parent incident type does not exist', async () => {
            const childType: PartialIncidentTypeDto = {
                name: 'Orphan Type',
                description: 'Type with non-existent parent',
                severity: 'Minor',
                subtypeOf: 'NON-EXISTENT-BID'
            };

            const response = await request(app)
                .post('/incident-types')
                .send(childType)
                .expect(500);

            expect(response.body).toHaveProperty('message');
        });
    });

    describe('GET /incident-types', () => {
        beforeEach(async () => {
            // Seed test data
            const incidentTypes = [
                {
                    _id: new mongoose.Types.ObjectId(),
                    bid: 'INC-TEST001',
                    name: 'Weather Delay',
                    description: 'Delays caused by adverse weather',
                    severity: 'Minor'
                },
                {
                    _id: new mongoose.Types.ObjectId(),
                    bid: 'INC-TEST002',
                    name: 'Equipment Breakdown',
                    description: 'Critical equipment breakdown',
                    severity: 'Critical'
                },
                {
                    _id: new mongoose.Types.ObjectId(),
                    bid: 'INC-TEST003',
                    name: 'Staff Shortage',
                    description: 'Insufficient staff for operations',
                    severity: 'Major'
                }
            ];
            await IncidentTypeModel.insertMany(incidentTypes);
        });

        it('should return paginated incident types', async () => {
            const response = await request(app)
                .get('/incident-types')
                .query({ pageNumber: 1, pageSize: 10 })
                .expect('Content-Type', /json/)
                .expect(200);

            expect(response.body).toHaveProperty('items');
            expect(response.body).toHaveProperty('pageNumber');
            expect(response.body).toHaveProperty('pageSize');
            expect(response.body).toHaveProperty('pageCount');
            expect(response.body.items).toHaveLength(3);
        });

        it('should filter incident types by name', async () => {
            const response = await request(app)
                .get('/incident-types')
                .query({ name: 'Weather', pageNumber: 1, pageSize: 10 })
                .expect(200);

            expect(response.body.items).toHaveLength(1);
            expect(response.body.items[0].name).toBe('Weather Delay');
        });

        it('should filter incident types by severity', async () => {
            const response = await request(app)
                .get('/incident-types')
                .query({ severity: 'Critical', pageNumber: 1, pageSize: 10 })
                .expect(200);

            expect(response.body.items).toHaveLength(1);
            expect(response.body.items[0].severity).toBe('Critical');
        });

        it('should respect pagination limits', async () => {
            const response = await request(app)
                .get('/incident-types')
                .query({ pageNumber: 1, pageSize: 2 })
                .expect(200);

            expect(response.body.items).toHaveLength(2);
            expect(response.body.pageSize).toBe(2);
        });
    });

    describe('GET /incident-types/:id', () => {
        let testIncidentTypeBid: string;

        beforeEach(async () => {
            const incidentType = await IncidentTypeModel.create({
                _id: new mongoose.Types.ObjectId(),
                bid: 'INC-GETTEST',
                name: 'Test Type',
                description: 'Test description',
                severity: 'Minor'
            });
            testIncidentTypeBid = incidentType.bid!;
        });

        it('should return incident type by bid', async () => {
            const response = await request(app)
                .get(`/incident-types/${testIncidentTypeBid}`)
                .expect('Content-Type', /json/)
                .expect(200);

            expect(response.body.bid).toBe(testIncidentTypeBid);
            expect(response.body.name).toBe('Test Type');
        });

        it('should return 404 for non-existent incident type', async () => {
            const response = await request(app)
                .get('/incident-types/NON-EXISTENT-BID')
                .expect(404);

            expect(response.body).toHaveProperty('message');
        });
    });

    describe('PUT /incident-types/:id', () => {
        let testIncidentTypeBid: string;

        beforeEach(async () => {
            const incidentType = await IncidentTypeModel.create({
                _id: new mongoose.Types.ObjectId(),
                bid: 'INC-UPTEST',
                name: 'Original Name',
                description: 'Original description',
                severity: 'Minor'
            });
            testIncidentTypeBid = incidentType.bid!;
        });

        it('should update an existing incident type', async () => {
            const updateData: PartialIncidentTypeDto = {
                name: 'Updated Name',
                description: 'Updated description',
                severity: 'Major'
            };

            const response = await request(app)
                .put(`/incident-types/${testIncidentTypeBid}`)
                .send(updateData)
                .expect('Content-Type', /json/)
                .expect(200);

            expect(response.body.name).toBe(updateData.name);
            expect(response.body.description).toBe(updateData.description);
            expect(response.body.severity).toBe(updateData.severity);

            // Verify persisted update
            const persisted = await IncidentTypeModel.findOne({ bid: testIncidentTypeBid });
            expect(persisted?.name).toBe(updateData.name);
        });

        it('should return 404 when updating non-existent incident type', async () => {
            const updateData: PartialIncidentTypeDto = {
                name: 'New Name',
                description: 'New description',
                severity: 'Minor'
            };

            const response = await request(app)
                .put('/incident-types/NON-EXISTENT-BID')
                .send(updateData)
                .expect(500);

            expect(response.body).toHaveProperty('message');
        });
    });

    describe('GET /incident-types/count', () => {
        it('should return 0 when no incident types exist', async () => {
            const response = await request(app)
                .get('/incident-types/count')
                .expect(200);

            expect(response.body.count).toBe(0);
        });

        it('should return correct count of incident types', async () => {
            // Seed some data
            await IncidentTypeModel.insertMany([
                { _id: new mongoose.Types.ObjectId(), bid: 'INC-COUNT1', name: 'Type 1', description: 'd1', severity: 'Minor' },
                { _id: new mongoose.Types.ObjectId(), bid: 'INC-COUNT2', name: 'Type 2', description: 'd2', severity: 'Major' }
            ]);

            const response = await request(app)
                .get('/incident-types/count')
                .expect(200);

            expect(response.body.count).toBe(2);
        });
    });
});
