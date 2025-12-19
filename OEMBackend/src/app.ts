import express from "express";
import cookieParser from "cookie-parser";
import { RegisterRoutes } from "./api/routes/routes";
import { errorHandler } from "./api/middlewares/errorHandler";
import swaggerUi from "swagger-ui-express";
import swaggerDoc from "./api/swagger/swagger.json";

export function createApp() {
	const app = express();

	// Global middlewares
	app.use(cookieParser());
	app.use(express.json());

	// Swagger
	app.use("/swagger", swaggerUi.serve, swaggerUi.setup(swaggerDoc));

	// Routes
	RegisterRoutes(app);

	// Error handler (after routes)
	app.use(errorHandler);

	return app;
}
