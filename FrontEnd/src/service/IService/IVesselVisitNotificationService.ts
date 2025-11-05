import type { Filter, Page } from "@/model/Page";
import type { VesselVisitNotification, VesselVisitNotificationStatus, NotificationDecision } from "@/model/VesselVisitNotification";

export interface IVesselVisitNotificationService {
    getVesselVisitNotifications(): Promise<VesselVisitNotification[]>;
    getVesselVisitNotificationById(id: string): Promise<VesselVisitNotification>;
    getNotificationDecisions(vesselVisitNotificationId: string): Promise<NotificationDecision[]>;
    createVesselVisitNotification(notification: VesselVisitNotification): Promise<VesselVisitNotification>;
    createNotificationDecision(vesselVisitNotificationId: string, decision: NotificationDecision): Promise<NotificationDecision>;
    updateVesselVisitNotification(id: string, notification: VesselVisitNotification): Promise<VesselVisitNotification>;
    submitVesselVisitNotification(id: string): Promise<void>;
    filterVesselVisitNotifications(filter: Filter<VesselVisitNotificationStatus>): Promise<Page<VesselVisitNotificationStatus>>;
    deleteDraft(id: string): Promise<void>;
}