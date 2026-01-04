import mongoose from 'mongoose';
import { MongoMemoryServer } from 'mongodb-memory-server';
import express, { Express } from 'express';
import { RegisterRoutes } from '../../src/api/routes/routes';
import { authMiddleware } from '../../src/api/middlewares/authMiddleware';
import { errorHandler } from '../../src/api/middlewares/errorHandler';

let mongoServer: MongoMemoryServer | null = null;

/**
 * Creates a test Express application with routes registered.
 * Authentication is disabled for testing purposes.
 */
export function createTestApp(): Express {
    const app = express();
    
    // Global middlewares
    app.use(express.json());
    
    // Routes (mounted behind authentication)
    const apiRouter = express.Router();
    RegisterRoutes(apiRouter);
    app.use('/', authMiddleware, apiRouter);
    
    // Error handler (after routes)
    app.use(errorHandler);
    
    return app;
}

/**
 * Connects to an in-memory MongoDB instance for testing.
 * Each test suite gets a fresh database.
 */
export async function connectTestDatabase(): Promise<void> {
    mongoServer = await MongoMemoryServer.create();
    const uri = mongoServer.getUri();
    
    await mongoose.connect(uri);
    console.log('Connected to in-memory test MongoDB');
}

/**
 * Disconnects from the test database and stops the in-memory server.
 */
export async function disconnectTestDatabase(): Promise<void> {
    await mongoose.disconnect();
    if (mongoServer) {
        await mongoServer.stop();
        mongoServer = null;
    }
    console.log('Disconnected from in-memory test MongoDB');
}

/**
 * Clears all collections in the test database.
 * Useful for cleaning up between tests.
 */
export async function clearTestDatabase(): Promise<void> {
    const collections = mongoose.connection.collections;
    for (const key in collections) {
        await collections[key].deleteMany({});
    }
}

/**
 * Helper function to seed test data into the database.
 */
export async function seedTestData<T>(
    model: mongoose.Model<T>,
    data: Partial<T>[]
): Promise<T[]> {
    return model.insertMany(data as any) as unknown as T[];
}
