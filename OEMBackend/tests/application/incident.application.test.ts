import request from 'supertest';
import { Express } from 'express';
import mongoose from 'mongoose';
import {
    createTestApp,
    connectTestDatabase,
    disconnectTestDatabase,
    clearTestDatabase
} from './setup';
import { IncidentModel } from '../../src/schemas/incidentSchema';
import { IncidentTypeModel } from '../../src/schemas/incidentTypeSchema';
import { CreateIncidentDto, UpdateIncidentDto } from '../../src/dto/incidentDto';

describe('Incident Application Tests', () => {
    let app: Express;
    let testIncidentTypeId: mongoose.Types.ObjectId;
    let testIncidentTypeBid: string;

    beforeAll(async () => {
        await connectTestDatabase();
        app = createTestApp();
    });

    afterAll(async () => {
        await disconnectTestDatabase();
    });

    beforeEach(async () => {
        await clearTestDatabase();
        
        // Create a required incident type for incidents
        testIncidentTypeId = new mongoose.Types.ObjectId();
        testIncidentTypeBid = 'INC-TYPE-TEST';
        await IncidentTypeModel.create({
            _id: testIncidentTypeId,
            bid: testIncidentTypeBid,
            name: 'Test Incident Type',
            description: 'A test incident type for application tests',
            severity: 'Major'
        });
    });

    describe('POST /incidents', () => {
        it('should create a new incident and persist it to the database', async () => {
            const newIncident: CreateIncidentDto = {
                type: testIncidentTypeBid,
                startTime: new Date().toISOString(),
                severity: 'Major',
                description: 'A test incident occurred during loading operations'
            };

            const response = await request(app)
                .post('/incidents')
                .send(newIncident)
                .expect('Content-Type', /json/)
                .expect(201);

            expect(response.body).toHaveProperty('bid');
            expect(response.body.bid).toMatch(/^INC-/);
            expect(response.body.description).toBe(newIncident.description);
            expect(response.body.severity).toBe(newIncident.severity);
            expect(response.body.type).toHaveProperty('bid', testIncidentTypeBid);

            // Verify it was actually persisted
            const persisted = await IncidentModel.findOne({ bid: response.body.bid });
            expect(persisted).not.toBeNull();
            expect(persisted?.description).toBe(newIncident.description);
        });

        it('should create incident with Minor severity', async () => {
            const newIncident: CreateIncidentDto = {
                type: testIncidentTypeBid,
                startTime: new Date().toISOString(),
                severity: 'Minor',
                description: 'A minor incident'
            };

            const response = await request(app)
                .post('/incidents')
                .send(newIncident)
                .expect(201);

            expect(response.body.severity).toBe('Minor');
        });

        it('should create incident with Critical severity', async () => {
            const newIncident: CreateIncidentDto = {
                type: testIncidentTypeBid,
                startTime: new Date().toISOString(),
                severity: 'Critical',
                description: 'A critical incident requiring immediate attention'
            };

            const response = await request(app)
                .post('/incidents')
                .send(newIncident)
                .expect(201);

            expect(response.body.severity).toBe('Critical');
        });

        it('should return error when incident type does not exist', async () => {
            const newIncident: CreateIncidentDto = {
                type: 'NON-EXISTENT-TYPE',
                startTime: new Date().toISOString(),
                severity: 'Major',
                description: 'An incident with invalid type'
            };

            const response = await request(app)
                .post('/incidents')
                .send(newIncident)
                .expect(500);

            expect(response.body).toHaveProperty('message');
        });
    });

    describe('GET /incidents', () => {
        beforeEach(async () => {
            // Seed test incidents
            const incidents = [
                {
                    _id: new mongoose.Types.ObjectId(),
                    bid: 'INC-TEST001',
                    type: testIncidentTypeId,
                    startTime: new Date('2024-01-15T10:00:00Z'),
                    severity: 'Minor',
                    description: 'Minor incident 1',
                    createdBy: 'test@example.com'
                },
                {
                    _id: new mongoose.Types.ObjectId(),
                    bid: 'INC-TEST002',
                    type: testIncidentTypeId,
                    startTime: new Date('2024-01-16T10:00:00Z'),
                    endTime: new Date('2024-01-16T12:00:00Z'),
                    severity: 'Major',
                    description: 'Major incident resolved',
                    createdBy: 'test@example.com'
                },
                {
                    _id: new mongoose.Types.ObjectId(),
                    bid: 'INC-TEST003',
                    type: testIncidentTypeId,
                    startTime: new Date('2024-01-17T10:00:00Z'),
                    severity: 'Critical',
                    description: 'Critical ongoing incident',
                    createdBy: 'test@example.com'
                }
            ];
            await IncidentModel.insertMany(incidents);
        });

        it('should return paginated incidents', async () => {
            const response = await request(app)
                .get('/incidents')
                .query({ pageNumber: 1, pageSize: 10 })
                .expect('Content-Type', /json/)
                .expect(200);

            expect(response.body).toHaveProperty('items');
            expect(response.body).toHaveProperty('pageNumber');
            expect(response.body).toHaveProperty('pageSize');
            expect(response.body).toHaveProperty('pageCount');
            expect(response.body.items).toHaveLength(3);
        });

        it('should filter incidents by severity', async () => {
            const response = await request(app)
                .get('/incidents')
                .query({ severity: 'Critical', pageNumber: 1, pageSize: 10 })
                .expect(200);

            expect(response.body.items).toHaveLength(1);
            expect(response.body.items[0].severity).toBe('Critical');
        });

        it('should filter resolved incidents', async () => {
            const response = await request(app)
                .get('/incidents')
                .query({ isResolved: true, pageNumber: 1, pageSize: 10 })
                .expect(200);

            expect(response.body.items).toHaveLength(1);
            expect(response.body.items[0].endTime).toBeDefined();
        });

        it('should filter unresolved incidents', async () => {
            const response = await request(app)
                .get('/incidents')
                .query({ isResolved: false, pageNumber: 1, pageSize: 10 })
                .expect(200);

            expect(response.body.items).toHaveLength(2);
            response.body.items.forEach((incident: any) => {
                expect(incident.endTime).toBeUndefined();
            });
        });

        it('should filter incidents by date range', async () => {
            const response = await request(app)
                .get('/incidents')
                .query({
                    filterStartTime: '2024-01-16T00:00:00Z',
                    filterEndTime: '2024-01-16T23:59:59Z',
                    pageNumber: 1,
                    pageSize: 10
                })
                .expect(200);

            expect(response.body.items.length).toBeGreaterThanOrEqual(1);
        });

        it('should respect pagination limits', async () => {
            const response = await request(app)
                .get('/incidents')
                .query({ pageNumber: 1, pageSize: 2 })
                .expect(200);

            expect(response.body.items).toHaveLength(2);
            expect(response.body.pageSize).toBe(2);
        });
    });

    describe('GET /incidents/:bid', () => {
        let testIncidentBid: string;

        beforeEach(async () => {
            testIncidentBid = 'INC-GETTEST';
            await IncidentModel.create({
                _id: new mongoose.Types.ObjectId(),
                bid: testIncidentBid,
                type: testIncidentTypeId,
                startTime: new Date(),
                severity: 'Minor',
                description: 'Test incident for GET by bid',
                createdBy: 'test@example.com'
            });
        });

        it('should return incident by bid', async () => {
            const response = await request(app)
                .get(`/incidents/${testIncidentBid}`)
                .expect('Content-Type', /json/)
                .expect(200);

            expect(response.body.bid).toBe(testIncidentBid);
            expect(response.body.description).toBe('Test incident for GET by bid');
        });

        it('should return 500 for non-existent incident', async () => {
            const response = await request(app)
                .get('/incidents/NON-EXISTENT-BID')
                .expect(500);

            expect(response.body).toHaveProperty('message');
        });
    });

    describe('PUT /incidents/:bid', () => {
        let testIncidentBid: string;

        beforeEach(async () => {
            testIncidentBid = 'INC-UPTEST';
            await IncidentModel.create({
                _id: new mongoose.Types.ObjectId(),
                bid: testIncidentBid,
                type: testIncidentTypeId,
                startTime: new Date('2024-01-15T10:00:00Z'),
                severity: 'Minor',
                description: 'Original incident description',
                createdBy: 'test@example.com'
            });
        });

        it('should update incident description', async () => {
            const updateData: Partial<UpdateIncidentDto> = {
                description: 'Updated incident description'
            };

            const response = await request(app)
                .put(`/incidents/${testIncidentBid}`)
                .send(updateData)
                .expect('Content-Type', /json/)
                .expect(200);

            expect(response.body.description).toBe(updateData.description);

            // Verify persisted update
            const persisted = await IncidentModel.findOne({ bid: testIncidentBid });
            expect(persisted?.description).toBe(updateData.description);
        });

        it('should update incident severity', async () => {
            const updateData: Partial<UpdateIncidentDto> = {
                severity: 'Critical'
            };

            const response = await request(app)
                .put(`/incidents/${testIncidentBid}`)
                .send(updateData)
                .expect(200);

            expect(response.body.severity).toBe('Critical');
        });

        it('should resolve incident by setting endTime', async () => {
            const endTime = new Date('2024-01-15T14:00:00Z').toISOString();
            const updateData: Partial<UpdateIncidentDto> = {
                endTime: endTime
            };

            const response = await request(app)
                .put(`/incidents/${testIncidentBid}`)
                .send(updateData)
                .expect(200);

            expect(response.body.endTime).toBeDefined();
        });

        it('should return error when endTime is before startTime', async () => {
            const updateData: Partial<UpdateIncidentDto> = {
                endTime: new Date('2024-01-14T10:00:00Z').toISOString() // Before start time
            };

            const response = await request(app)
                .put(`/incidents/${testIncidentBid}`)
                .send(updateData)
                .expect(500);

            expect(response.body).toHaveProperty('message');
        });
    });

    describe('GET /incidents/count', () => {
        it('should return 0 when no incidents exist', async () => {
            const response = await request(app)
                .get('/incidents/count')
                .expect(200);

            expect(response.body.count).toBe(0);
        });

        it('should return correct count of incidents', async () => {
            // Seed some incidents
            await IncidentModel.insertMany([
                { _id: new mongoose.Types.ObjectId(), bid: 'INC-COUNT1', type: testIncidentTypeId, startTime: new Date(), severity: 'Minor', description: 'd1', createdBy: 'test@example.com' },
                { _id: new mongoose.Types.ObjectId(), bid: 'INC-COUNT2', type: testIncidentTypeId, startTime: new Date(), severity: 'Major', description: 'd2', createdBy: 'test@example.com' }
            ]);

            const response = await request(app)
                .get('/incidents/count')
                .expect(200);

            expect(response.body.count).toBe(2);
        });
    });
});
