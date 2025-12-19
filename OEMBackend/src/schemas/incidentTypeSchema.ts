import mongoose from "mongoose";

const IncidentTypeSchema = new mongoose.Schema(
	{
		_id: { // force use of _id field so there's no problems with references
			type: mongoose.Schema.Types.ObjectId,
			required: true
		},

		bid: {
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
			enum: ['Minor', 'Major', 'Critical'],
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
	},
	{
		timestamps: true
	}
);

export const IncidentTypeModel = mongoose.model("IncidentType", IncidentTypeSchema);