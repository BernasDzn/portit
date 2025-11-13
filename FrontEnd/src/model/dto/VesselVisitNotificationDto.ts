export enum VesselVisitNotificationStatus {
    InProgress = 0,
    ApprovalPending = 1,
    Decided = 2
}

export enum NotificationDecisionStatus {
    Accepted = 1,
    Rejected = 2,
}

export interface VesselVisitNotificationDto {
    notificationId: string;
    expectedArrival: string;
    expectedDeparture: string;
    isCargoHazardous: boolean;
    specialRequirements?: string;
    crewDetails?: CrewDetails;
    loadCargoManifest: CargoManifestItem[];
    unloadCargoManifest: CargoManifestItem[];
    vesselImoNumber: string;
}

export interface VesselVisitNotificationFilter {
    Status: VesselVisitNotificationStatusFilter
    WithReason: boolean
    WithDockAssigned: boolean
    Vessel: string
    ExpectedArrivalFrom: Date
    ExpectedArrivalTo: Date
}

export enum VesselVisitNotificationStatusFilter{
    InProgress = 0,
    ApprovalPending = 1,
    Accepted = 2,
    Rejected = 3
}

export interface CrewDetails {
    captain: {
        value: string;
    };
    totalCrewMembers: number;
    safetyOfficers: Person[];
}

export interface CargoManifestItem {
    position: Position;
    storageAreaCode: string;
    container: Container;
}

export interface Position {
    bay: string;
    row: string;
    tier: string;
}

export interface Container {
    containerNumber: string;
    cargoType: number;
    description: string;
}

export interface Person {
    name: string;
    citizenshipId: number;
    emailAddress: string;
    phone: string;
}

export interface NotificationDecisionDto {
    status: NotificationDecisionStatus;
    reason?: string;
    decisionDate: Date;
    officerID?: number;
    assignedDockCode?: string;
    isFinal: boolean;
}