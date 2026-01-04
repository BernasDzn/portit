import mongoose from 'mongoose';
import { MongoMemoryServer } from 'mongodb-memory-server';

let mongoServer: MongoMemoryServer | null = null;

export async function connectToDatabase(mongoUri: string, useInMemory: boolean = false): Promise<void> {
    if (useInMemory) {
        console.log('Starting in-memory MongoDB instance...');
        mongoServer = await MongoMemoryServer.create();
        const uri = mongoServer.getUri();
        await mongoose.connect(uri);
        console.log('Connected to in-memory MongoDB');
    } else {
        await mongoose.connect(mongoUri);
        console.log('Connected to MongoDB');
    }
}

export async function disconnectDatabase(): Promise<void> {
    await mongoose.disconnect();
    if (mongoServer) {
        await mongoServer.stop();
        mongoServer = null;
    }
}
