import mongoose from "mongoose";
import config from "./config/config";
import { createApp } from "./app";
import { bootstrap } from "./bootstrap";
import { connectToDatabase } from "./config/database";

const app = createApp();

// Use in-memory database for test:e2e mode (when RUN_MODE=local and DISABLE_AUTH=true)
const useInMemory = process.env.RUN_MODE === 'local' && process.env.DISABLE_AUTH === 'true';

connectToDatabase(config.mongoUri, useInMemory)
	.then(async () => {
		if (config.shouldBootstrap) {
			await mongoose.connection.dropDatabase();
			console.log("Database dropped for bootstrapping");

			await bootstrap();
			console.log("Database bootstrapped");
		}

		app.listen(config.port, () => {
			console.log(`Server running on http://localhost:${config.port}`);
			console.log(`Swagger docs available at http://localhost:${config.port}/swagger`);
		});
	})
	.catch(err => console.error("MongoDB connection error:", err));
