import type { VesselVisitNotificationDto, CrewDetails, CargoManifestItem, Person } from './dto/VesselVisitNotificationDto';
import { VesselVisitNotificationStatus, NotificationDecisionStatus } from './dto/VesselVisitNotificationDto';
import type { Representative } from './Representative';
import { Vessel } from './Vessel';
import type { NotificationDecision as NotificationDecisionType } from './VesselVisitNotification';

export class VesselVisitNotification {
    private _notificationId: string;
    private _status: VesselVisitNotificationStatus;
    private _expectedArrival: Date;
    private _expectedDeparture: Date;
    private _isCargoHazardous: boolean;
    private _specialRequirements?: string;
    private _crewDetails?: CrewDetails;
    private _loadCargoManifest: CargoManifestItem[];
    private _unloadCargoManifest: CargoManifestItem[];
    private _vessel: Vessel;
    private _submitter: Representative;
    private _notificationDecisions: NotificationDecisionType[] = [];

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
        if (!params.notificationId) throw new Error('Notification ID cannot be null or empty.');
        if (!params.vessel) throw new Error('Vessel cannot be null.');
        if (!params.submitter) throw new Error('Submitter cannot be null.');
        if (params.expectedArrival >= params.expectedDeparture) {
            throw new Error('Expected arrival must be before expected departure.');
        }

        this._notificationId = params.notificationId;
        this._status = params.status;
        this._expectedArrival = params.expectedArrival;
        this._expectedDeparture = params.expectedDeparture;
        this._isCargoHazardous = params.isCargoHazardous;
        this._specialRequirements = params.specialRequirements;
        this._crewDetails = params.crewDetails;
        this._loadCargoManifest = params.loadCargoManifest ?? [];
        this._unloadCargoManifest = params.unloadCargoManifest ?? [];
        this._vessel = params.vessel;
        this._submitter = params.submitter;
    }

    get notificationId(): string { return this._notificationId; }
    get status(): VesselVisitNotificationStatus { return this._status; }
    get expectedArrival(): Date { return this._expectedArrival; }
    get expectedDeparture(): Date { return this._expectedDeparture; }
    get isCargoHazardous(): boolean { return this._isCargoHazardous; }
    get specialRequirements(): string | undefined { return this._specialRequirements; }
    get crewDetails(): CrewDetails | undefined { return this._crewDetails; }
    get loadCargoManifest(): CargoManifestItem[] { return this._loadCargoManifest; }
    get unloadCargoManifest(): CargoManifestItem[] { return this._unloadCargoManifest; }
    get vessel(): Vessel { return this._vessel; }
    get submitter(): Representative { return this._submitter; }
    get notificationDecisions(): NotificationDecisionType[] { return this._notificationDecisions; }

    update(
        expectedArrival: Date,
        expectedDeparture: Date,
        isCargoHazardous: boolean,
        specialRequirements?: string,
        crewDetails?: CrewDetails,
        loadCargoManifest?: CargoManifestItem[],
        unloadCargoManifest?: CargoManifestItem[]
    ): void {
        if (this._status !== VesselVisitNotificationStatus.InProgress) {
            throw new Error('Only notifications in progress can be updated.');
        }
        if (expectedArrival >= expectedDeparture) {
            throw new Error('Expected arrival must be before expected departure.');
        }

        this._expectedArrival = expectedArrival;
        this._expectedDeparture = expectedDeparture;
        this._isCargoHazardous = isCargoHazardous;
        this._specialRequirements = specialRequirements;
        this._crewDetails = crewDetails;
        if (loadCargoManifest) this._loadCargoManifest = loadCargoManifest;
        if (unloadCargoManifest) this._unloadCargoManifest = unloadCargoManifest;
    }

    submit(): void {
        if (this._status !== VesselVisitNotificationStatus.InProgress) {
            throw new Error('Only notifications in progress can be submitted.');
        }
        if (this._isCargoHazardous && (!this._crewDetails?.safetyOfficers || this._crewDetails.safetyOfficers.length === 0)) {
            throw new Error('Notifications with hazardous cargo must include at least one safety officer in the crew details.');
        }

        this._status = VesselVisitNotificationStatus.ApprovalPending;
    }

    private close(): void {
        this._status = VesselVisitNotificationStatus.Decided;
    }

    addDecision(decision: NotificationDecisionType): void {
        if (!decision) throw new Error('Decision cannot be null.');
        if (this._status !== VesselVisitNotificationStatus.ApprovalPending) {
            throw new Error('Decisions can only be added to notifications pending approval.');
        }

        const latestDecision = this.getLatestDecision();
        if (latestDecision && decision.decisionDate < latestDecision.decisionDate) {
            throw new Error('A newer decision has already been made.');
        }

        this._notificationDecisions.push(decision);

        if (decision.isFinal) {
            this.close();
        } else if (decision.status === NotificationDecisionStatus.Rejected) {
            this._status = VesselVisitNotificationStatus.InProgress;
        }
    }

    getLatestDecision(): NotificationDecisionType | undefined {
        return this._notificationDecisions
            .sort((a, b) => b.decisionDate.getTime() - a.decisionDate.getTime())[0];
    }

    toDto(): VesselVisitNotificationDto {
        return {
            notificationId: this._notificationId,
            expectedArrival: this._expectedArrival.toISOString(),
            expectedDeparture: this._expectedDeparture.toISOString(),
            isCargoHazardous: this._isCargoHazardous,
            specialRequirements: this._specialRequirements,
            crewDetails: this._crewDetails,
            loadCargoManifest: this._loadCargoManifest,
            unloadCargoManifest: this._unloadCargoManifest,
            vesselImoNumber: this._vessel.imoNumber
        };
    }
}

export class NotificationDecision {
    private _status: NotificationDecisionStatus;
    private _reason?: string;
    private _decisionDate: Date;
    private _officerEmail: string;
    private _assignedDockCode?: string;
    private _isFinal: boolean;

    constructor(params: {
        status: NotificationDecisionStatus;
        reason?: string;
        decisionDate: Date;
        officerEmail: string;
        assignedDockCode?: string;
        isFinal: boolean;
    }) {
        this._status = params.status;
        this._reason = params.reason;
        this._decisionDate = params.decisionDate;
        this._officerEmail = params.officerEmail;
        this._assignedDockCode = params.assignedDockCode;
        this._isFinal = params.isFinal;
    }

    get status(): NotificationDecisionStatus { return this._status; }
    get reason(): string | undefined { return this._reason; }
    get decisionDate(): Date { return this._decisionDate; }
    get officerEmail(): string { return this._officerEmail; }
    get assignedDockCode(): string | undefined { return this._assignedDockCode; }
    get isFinal(): boolean { return this._isFinal; }

    toDto() {
        return {
            status: this._status,
            reason: this._reason,
            decisionDate: this._decisionDate,
            officerID: this._officerEmail,
            assignedDockCode: this._assignedDockCode,
            isFinal: this._isFinal
        };
    }
}