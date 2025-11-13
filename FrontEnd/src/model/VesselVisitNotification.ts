import type { VesselVisitNotificationDto, CrewDetails, CargoManifestItem, Person } from './dto/VesselVisitNotificationDto';
import { VesselVisitNotificationStatus, NotificationDecisionStatus } from './dto/VesselVisitNotificationDto';
import type { Representative } from './Representative';
import { Vessel } from './Vessel';

export class VesselVisitNotification {
    readonly notificationId: string;
    readonly status: VesselVisitNotificationStatus;
    readonly expectedArrival: Date;
    readonly expectedDeparture: Date;
    readonly isCargoHazardous: boolean;
    readonly specialRequirements?: string;
    readonly crewDetails?: CrewDetails;
    readonly loadCargoManifest: CargoManifestItem[];
    readonly unloadCargoManifest: CargoManifestItem[];
    readonly vessel: Vessel;
    readonly submitter: Representative;

    constructor(params: {
        notificationId: string;
        status: VesselVisitNotificationStatus;
        expectedArrival: Date;
        expectedDeparture: Date;
        isCargoHazardous: boolean;
        specialRequirements?: string;
        crewDetails?: CrewDetails;
        loadCargoManifest?: CargoManifestItem[];
        unloadCargoManifest?: CargoManifestItem[];
        vessel: Vessel;
        submitter: Representative;
    }) {
        this.notificationId = params.notificationId;
        this.status = params.status;
        this.expectedArrival = params.expectedArrival;
        this.expectedDeparture = params.expectedDeparture;
        this.isCargoHazardous = params.isCargoHazardous;
        this.specialRequirements = params.specialRequirements;
        this.crewDetails = params.crewDetails;
        this.loadCargoManifest = params.loadCargoManifest ?? [];
        this.unloadCargoManifest = params.unloadCargoManifest ?? [];
        this.vessel = params.vessel;
        this.submitter = params.submitter;
    }

    toDto(): VesselVisitNotificationDto {
        return {
            notificationId: this.notificationId,
            expectedArrival: this.expectedArrival.toISOString(),
            expectedDeparture: this.expectedDeparture.toISOString(),
            isCargoHazardous: this.isCargoHazardous,
            specialRequirements: this.specialRequirements,
            crewDetails: this.crewDetails,
            loadCargoManifest: this.loadCargoManifest,
            unloadCargoManifest: this.unloadCargoManifest,
            vesselImoNumber: (this.vessel as any).imoNumber ?? ''
        };
    }
}

export class NotificationDecision {
    readonly status: NotificationDecisionStatus;
    readonly reason?: string;
    readonly decisionDate: Date;
    readonly officerID?: number;
    readonly assignedDockCode?: string;
    readonly isFinal: boolean;

    constructor(params: {
        status: NotificationDecisionStatus;
        reason?: string;
        decisionDate: Date;
        officerID?: number;
        assignedDockCode?: string;
        isFinal: boolean;
    }) {
        this.status = params.status;
        this.reason = params.reason;
        this.decisionDate = params.decisionDate;
        this.officerID = params.officerID;
        this.assignedDockCode = params.assignedDockCode;
        this.isFinal = params.isFinal;
    }

    toDto() {
        return {
            status: this.status,
            reason: this.reason,
            decisionDate: this.decisionDate,
            officerID: this.officerID,
            assignedDockCode: this.assignedDockCode,
            isFinal: this.isFinal
        };
    }
}