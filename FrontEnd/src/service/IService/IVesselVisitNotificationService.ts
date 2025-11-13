import type { NotificationDecisionDto, VesselVisitNotificationDto, VesselVisitNotificationFilter } from "@/model/dto/VesselVisitNotificationDto";
import type { Filter, Page } from "@/model/Page";
import type { VesselVisitNotification, NotificationDecision } from "@/model/VesselVisitNotification";

export interface IVesselVisitNotificationService {

    getVesselVisitNotifications(): Promise<Page<VesselVisitNotification>>;
    getVesselVisitNotificationsForReview(): Promise<Page<VesselVisitNotification>>;
    getVesselVisitNotificationsByRepresentative(filter?: Filter<VesselVisitNotificationFilter>): Promise<Page<VesselVisitNotification>>;
    getVesselVisitNotificationById(id: string): Promise<VesselVisitNotification>;
    getNotificationDecisions(vesselVisitNotificationId: string): Promise<NotificationDecision[]>;

    createVesselVisitNotification(notification: VesselVisitNotificationDto): Promise<VesselVisitNotification>;
    createNotificationDecision(vesselVisitNotificationId: string, decision: NotificationDecisionDto): Promise<NotificationDecision>;
    
    updateVesselVisitNotification(notification: VesselVisitNotificationDto): Promise<VesselVisitNotification>;
    submitVesselVisitNotification(id: string): Promise<void>;
    deleteDraft(id: string): Promise<void>;
}