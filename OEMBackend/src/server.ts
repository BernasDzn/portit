import mongoose from "mongoose";
import config from "./config/config";
import { createApp } from "./app";
import { bootstrap } from "./bootstrap";

const app = createApp();

mongoose.connect(config.mongoUri)
	.then(async () => {
		console.log("Connected to MongoDB");

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
