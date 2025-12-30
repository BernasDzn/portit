import type { IncidentDto } from "./dto/IncidentDto";
import { IncidentType } from "./IncidentType";

export type IncidentSeverity = 'Minor' | 'Major' | 'Critical';

export class Incident {
    bid: string;
    type: IncidentType;
    startTime: Date;
    endTime?: Date;
    severity: IncidentSeverity;
    description: string;
    createdBy: string;
    affectedVVECodes?: string[];

    constructor(
        bid: string,
        type: IncidentType,
        startTime: Date,
        severity: IncidentSeverity,
        description: string,
        createdBy: string,
        endTime?: Date,
        affectedVVECodes?: string[]
    ) {
        this.bid = bid;
        this.type = type;
        this.startTime = startTime;
        this.endTime = endTime;
        this.severity = severity;
        this.description = description;
        this.createdBy = createdBy;
        this.affectedVVECodes = affectedVVECodes;
    }

    validate() {
        if (!this.bid) {
            throw new Error("Incident must have a business ID");
        }
        if (!this.type) {
            throw new Error("Incident must have a type");
        }
        if (!this.startTime) {
            throw new Error("Incident must have a start time");
        }
        if (this.endTime && this.endTime < this.startTime) {
            throw new Error("End time cannot be before start time");
        }
    }

    isOngoing(): boolean {
        return this.endTime === undefined;
    }

    isActive(): boolean {
        const now = new Date();
        return this.startTime <= now && (this.endTime === undefined || this.endTime >= now);
    }

    getDuration(): number | undefined {
        if (!this.endTime) {
            return undefined;
        }
        return this.endTime.getTime() - this.startTime.getTime();
    }

    hasAffectedVVECodes(): boolean {
        return this.affectedVVECodes !== undefined && this.affectedVVECodes.length > 0;
    }

    toDto(): IncidentDto {
        return {
            type: this.type.bid,
            startTime: this.startTime.toISOString(),
            endTime: this.endTime?.toISOString(),
            severity: this.severity,
            description: this.description,
            affectedVVECodes: this.affectedVVECodes
        };
    }
}