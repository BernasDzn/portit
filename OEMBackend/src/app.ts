import { errorHandler } from './api/middlewares/errorHandler';
import { swaggerSpec } from './config/swagger';
import { bootstrap } from './bootstrap';
import express from 'express';
import swaggerUi from 'swagger-ui-express';
import mongoose from 'mongoose';
import config from './config/config';
import cookieParser from 'cookie-parser';

import routes from './api';

const app = express();
app.use(cookieParser());
app.use(express.json());

// Connect to database
mongoose.connect(config.mongoUri, {})
    .then(async () => {
        console.log('Connected to MongoDB');

        if (config.shouldBootstrap){

            await mongoose.connection.dropDatabase();
            console.log('Database dropped for bootstrapping');

            await bootstrap();
            console.log('Database bootstrapped');
        }
    })
    .catch(err => console.error('MongoDB connection error:', err));

// Swagger documentation
app.use('/swagger', swaggerUi.serve, swaggerUi.setup(swaggerSpec));

// Routes
app.use(routes());

// Global error handler (should be after routes)
app.use(errorHandler);

export default app;