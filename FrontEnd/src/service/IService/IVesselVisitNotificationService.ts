import type { NotificationDecisionDto, VesselVisitDistributionDto, VesselVisitNotificationDto, VesselVisitNotificationFilter, VesselVisitNotificationFilterPa } from "@/model/dto/VesselVisitNotificationDto";
import type { Filter, Page } from "@/model/Page";
import type { VesselVisitNotification, NotificationDecision } from "@/model/VesselVisitNotification";

export interface IVesselVisitNotificationService {

    getVesselVisitNotifications(filter?: Filter<null>): Promise<Page<VesselVisitNotification>>;
    getVesselVisitNotificationsForReview(filter: Filter<null>): Promise<Page<VesselVisitNotification>>;
    getVesselVisitNotificationsByRepresentative(filter?: Filter<VesselVisitNotificationFilter>): Promise<Page<VesselVisitNotification>>;
    getVesselVisitNotificationById(id: string): Promise<VesselVisitNotification>;
    getNotificationDecisions(vesselVisitNotificationId: string): Promise<NotificationDecision[]>;
    getNotificationsOnMonth(date: string): Promise<VesselVisitNotification[]>;

    createVesselVisitNotification(notification: VesselVisitNotification): Promise<VesselVisitNotification>;
    createNotificationDecision(vesselVisitNotificationId: string, decision: NotificationDecisionDto): Promise<NotificationDecision>;
    
    updateVesselVisitNotification(notification: VesselVisitNotification): Promise<VesselVisitNotification>;
    submitVesselVisitNotification(id: string): Promise<void>;
    deleteDraft(id: string): Promise<void>;

    count(): Promise<VesselVisitDistributionDto>;

    rebalanceDocks(date: Date, daysAhead: number): Promise<any>;
    applyRebalancing(assignments: any[]): Promise<void>;
}
