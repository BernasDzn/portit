import type { Filter, Page } from "@/model/Page";
import type { VesselVisitNotification, NotificationDecision, VesselVisitNotificationFilter } from "@/model/VesselVisitNotification";

export interface IVesselVisitNotificationService {
    getVesselVisitNotifications(): Promise<Page<VesselVisitNotification>>;
    getVesselVisitNotificationsByRepresentative(citizenId: string, filter?: Filter<VesselVisitNotificationFilter>): Promise<Page<VesselVisitNotification>>;
    getVesselVisitNotificationById(id: string): Promise<VesselVisitNotification>;
    getNotificationDecisions(vesselVisitNotificationId: string): Promise<NotificationDecision[]>;
    createVesselVisitNotification(notification: VesselVisitNotification): Promise<VesselVisitNotification>;
    createNotificationDecision(vesselVisitNotificationId: string, decision: NotificationDecision): Promise<NotificationDecision>;
    updateVesselVisitNotification(id: string, notification: VesselVisitNotification): Promise<VesselVisitNotification>;
    submitVesselVisitNotification(id: string): Promise<void>;
    deleteDraft(id: string): Promise<void>;
}