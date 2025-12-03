import swaggerJSDoc from 'swagger-jsdoc';
import path from 'path';

const swaggerOptions: swaggerJSDoc.Options = {
  definition: {
    openapi: '3.0.0',
    info: {
      title: 'OEM Backend API',
      version: '1.0.0',
      description: 'API documentation for OEM Backend',
    },
    servers: [
      {
        url: 'http://localhost:4000',
        description: 'Development server',
      },
    ],
  },
  // Path to the API routes with JSDoc comments
  apis: [
    path.join(process.cwd(), 'src/controllers/*.ts'),
    path.join(process.cwd(), 'src/routes/*.ts'),
  ],
};

export const swaggerSpec = swaggerJSDoc(swaggerOptions);
