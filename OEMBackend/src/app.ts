import express from 'express';
import swaggerUi from 'swagger-ui-express';
import itemRoutes from './routes/itemRoutes';
import { errorHandler } from './middlewares/errorHandler';
import { swaggerSpec } from './config/swagger';
import mongoose from 'mongoose';
import config from './config/config';

const app = express();
app.use(express.json());

// Connect to database
mongoose.connect(config.mongoUri, {})
    .then(() => console.log('MongoDB connected'))
    .catch(err => console.error('MongoDB connection error:', err));

// Swagger documentation
app.use('/api-docs', swaggerUi.serve, swaggerUi.setup(swaggerSpec));

// Routes
app.use('/api/items', itemRoutes);

// Global error handler (should be after routes)
app.use(errorHandler);

export default app;