import mongoose from "mongoose";

const IncidentSchema = new mongoose.Schema(
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

		type: {
			type: mongoose.Schema.Types.ObjectId,
			ref: "IncidentType",
			required: true
		},

		startTime: {
			type: Date,
			required: true
		},

		endTime: {
			type: Date,
			required: false
		},

		severity: {
			type: String,
			enum: ['Minor', 'Major', 'Critical'],
			required: true
		},

		description: {
			type: String,
			required: true
		},

		createdBy: {
			type: String,
			required: true
		},

		affectedVVECodes: [{
			type: String,
			required: false
		}]
	},
	{
		timestamps: true
	}
);

export const IncidentModel = mongoose.model("Incident", IncidentSchema);