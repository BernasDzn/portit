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
	parent: {
		type: mongoose.Schema.Types.ObjectId,
		ref: "IncidentType",
		required: false
	},
	children: [{
		type: mongoose.Schema.Types.ObjectId,
		ref: "IncidentType",
		required: false
	}]
});

export const IncidentTypeModel = mongoose.model("IncidentType", IncidentTypeSchema);