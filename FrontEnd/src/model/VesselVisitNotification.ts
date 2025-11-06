export interface VesselVisitNotification {
    notificationId: string;
    expectedArrival: string;
    expectedDeparture: string;
    isCargoHazardous: boolean;
    specialRequirements: string;
    crewDetails: CrewDetails;
    loadCargoManifest: LoadCargoManifestItem[];
    unloadCargoManifest: string[];
    vessel: Vessel;
    submitter: Person;
}

export interface CrewDetails {
    captain: {
        value: string;
    };
    totalCrewMembers: number;
    safetyOfficers: string | null;
}

export interface LoadCargoManifestItem {
    position: Position;
    area: Area;
    container: Container;
}

export interface Position {
    bay: string;
    row: string;
    tier: string;
}

export interface Area {
    nameCode: string;
    location: string;
    type: number;
    capacity: number;
    currentOccupancy: number;
    dockServices: DockService[];
}

export interface DockService {
    dock: Dock;
    distance: number;
    isServingDock: boolean;
}

export interface Dock {
    code: string;
    name: string;
    location: string;
    physicalCharacteristics: PhysicalCharacteristics;
    supportedVesselTypes: VesselType[];
}

export interface VesselType {
    name: string;
    description: string;
    maxNumberOfRows: number;
    maxNumberOfBays: number;
    maxNumberOfTiers: number;
    capacity: number;
    physicalCharacteristics: PhysicalCharacteristics;
}

export interface PhysicalCharacteristics {
    length: number;
    depth: number;
    draft: number;
}

export interface Container {
    containerNumber: string;
    cargoType: number;
    description: string;
}

export interface Vessel {
    name: string;
    imoNumber: string;
    type: VesselType;
    owner: Owner;
    physicalCharacteristics: PhysicalCharacteristics;
}

export interface Owner {
    name: string;
    altNames: string[];
    taxNumber: string;
    address: Address;
    representatives: Person[];
}

export interface Address {
    id: string;
    street: string;
    city: string;
    zipCode: string;
    country: string;
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
    status: number;
    reason?: string;
    decisionDate: string;
    officerId?: number;
    assignedDock?: Dock;
    isFinal: boolean;
}