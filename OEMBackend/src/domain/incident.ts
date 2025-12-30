import mongoose, { ObjectId } from "mongoose";
import { Entity } from "../core/domain/entity";
import IncidentType from "./incidentType";
import VesselVisitExecution from "./vesselVisitExecution";

export type IncidentSeverity = 'Minor' | 'Major' | 'Critical';

interface IncidentProps {
	bid?: string;
	type: IncidentType;
	startTime: Date; // within the time frame the Incident is "Active"
	endTime?: Date; // end time can be undefined in case the incident is ongoing
	severity: IncidentSeverity; // could be different from type according to situation
	description: string; // free text description
	createdBy: string; // email address of creator
	affectedVVECodes?: VesselVisitExecution[];
}

export default class Incident extends Entity<IncidentProps> {
	get bid(): string { return this.props.bid ? this.props.bid : ""; }
	get type(): IncidentType { return this.props.type; }
	get startTime(): Date { return this.props.startTime; }
	get endTime(): Date | undefined { return this.props.endTime; }
	get severity(): string { return this.props.severity; }
	get description(): string { return this.props.description; }
	get createdBy(): string { return this.props.createdBy; }
	get affectedVVECodes(): VesselVisitExecution[] | undefined { return this.props.affectedVVECodes; }
	
	set type(value: IncidentType) { this.props.type = value; }
	set startTime(value: Date) { this.props.startTime = value; }
	set endTime(value: Date | undefined) { this.props.endTime = value; }
	set severity(value: IncidentSeverity) { this.props.severity = value; }
	set description(value: string) { this.props.description = value; }
	set affectedVVECodes(value: VesselVisitExecution[] | undefined) { this.props.affectedVVECodes = value; }

	constructor(props: IncidentProps, mongoId?: mongoose.Types.ObjectId) {
		super(props, mongoId);
		if(!props.bid) { props.bid = this.newUniqueIncidentId(); }
	}

	private newUniqueIncidentId() : string{
		const timestamp = Date.now().toString(16).substring(4, 7).toUpperCase();
		const random = Math.random().toString(16).substring(2, 8).toUpperCase();
		return "INC-" + timestamp + random;
	}

}