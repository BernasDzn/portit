import type { VesselVisitNotification } from "./VesselVisitNotification";

export interface Representative {
    id: string;
    name: string;
    citizenshipId: string;
    emailAddress: string;
    phone: string;
    location: string;
    vesselVisitNotifications?: VesselVisitNotification[];
}
