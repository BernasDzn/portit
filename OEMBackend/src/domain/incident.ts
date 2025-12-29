import mongoose, { ObjectId } from "mongoose";
import { Entity } from "../core/domain/entity";
import IncidentType from "./incidentType";

export type severity = 'Minor' | 'Major' | 'Critical';

interface IncidentProps {
	bid?: string;
	type: IncidentType;
	startTime: Date; // within the time frame the Incident is "Active"
	endTime?: Date; // end time can be undefined in case the incident is ongoing
	severity: severity; // could be different from type according to situation
	description: string; // free text description
	createdBy: string; // email address of creator
	affectedVVECodes?: string[];
}

export default class Incident extends Entity<IncidentProps> {
	get bid(): string { return this.props.bid ? this.props.bid : ""; }
	get type(): IncidentType { return this.props.type; }
	get startTime(): Date { return this.props.startTime; }
	get endTime(): Date | undefined { return this.props.endTime; }
	get severity(): string { return this.props.severity; }
	get description(): string { return this.props.description; }
	get createdBy(): string { return this.props.createdBy; }
	get affectedVVECodes(): string[] | undefined { return this.props.affectedVVECodes; }

	constructor(props: IncidentProps, mongoId?: mongoose.Types.ObjectId) {
		super(props, mongoId);
		if(!props.bid) { props.bid = this.newUniqueIncidentId(); }
	}

	private newUniqueIncidentId() : string{
		const timestamp = Date.now().toString(16).substring(4, 7).toUpperCase();
		const random = Math.random().toString(16).substring(2, 8).toUpperCase();
		return "INC-" + timestamp + random;
	}

	isResolved(): boolean {
		return this.props.endTime !== undefined;
	}

	withinDateRange(timeStart: Date, timeEnd?: Date) : boolean {
		if(timeEnd !== undefined){
			if(this.props.endTime !== undefined){
				return this.props.startTime >= timeStart && this.props.endTime <= timeEnd;
			}
			return this.props.startTime >= timeStart && this.props.startTime <= timeEnd;
		}
		return this.props.startTime >= timeStart;
	}

	addAffectedVVE(vveCode: string) {
		if (!this.props.affectedVVECodes) {
			this.props.affectedVVECodes = [];
		}
		if (!this.props.affectedVVECodes.includes(vveCode)) {
			this.props.affectedVVECodes.push(vveCode);
		}
	}

	removeAffectedVVE(vveCode: string) {
		if (this.props.affectedVVECodes) {
			this.props.affectedVVECodes = this.props.affectedVVECodes.filter(code => code !== vveCode);
		}
	}

}