import type { Address } from "./values/Address";
import type { Dock } from "./Dock";
import type { StorageArea } from "./StorageArea";
import type { Vessel } from "./Vessel";

export interface VesselVisitNotification {
    notificationId: string;
    status: VesselVisitNotificationStatus;
    expectedArrival: string;
    expectedDeparture: string;
    isCargoHazardous: boolean;
    specialRequirements: string;
    crewDetails: CrewDetails;
    loadCargoManifest: CargoManifestItem[];
    unloadCargoManifest: CargoManifestItem[];
    vessel: Vessel;
    submitter: Person;
    notificationDecisions: NotificationDecision[];
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
    safetyOfficers: string | null;
}

export interface CargoManifestItem {
    position: Position;
    area: StorageArea;
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

export interface Owner {
    name: string;
    altNames: string[];
    taxNumber: string;
    address: Address;
    representatives: Person[];
}

export interface Person {
    name: string;
    citizenshipId: number;
    emailAddress: string;
    phone: string;
}

export enum VesselVisitNotificationStatus {
    InProgress = 0,
    ApprovalPending = 1,
    Decided = 2
}

export interface NotificationDecision {
    status: NotificationDecisionStatus;
    reason?: string;
    decisionDate: Date;
    officerID?: number;
    assignedDockCode?: string;
    isFinal: boolean;
}

export enum NotificationDecisionStatus {
    Accepted = 1,
    Rejected = 2,
}