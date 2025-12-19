import mongoose from "mongoose";

const IncidentTypeSchema = new mongoose.Schema({
	id: {
		type: String,
		required: true,
		unique: true
	},
	name: {
		type: String,
		required: true
	},
	description: {
		type: String,
		required: true
	},
	severity: {
		type: String,
		required: true
	},
	subtypeOf: {
		type: mongoose.Schema.Types.ObjectId,
		ref: "IncidentType",
		required: false
	},
	subtypes: [{
		type: mongoose.Schema.Types.ObjectId,
		ref: "IncidentType",
		required: false
	}]
});

export const IncidentTypeModel = mongoose.model("IncidentType", IncidentTypeSchema);