import { injectable, inject } from "inversify";
import { TYPES } from "@/inversify/types";
import type { IHttpService } from "./IService/IHttpService";
import type { IVesselVisitNotificationService } from "./IService/IVesselVisitNotificationService";
import type { Filter, Page } from "@/model/Page";
import type {
    VesselVisitNotification,
    NotificationDecision,
} from "@/model/VesselVisitNotification";
import type { NotificationDecisionDto, VesselVisitDistributionDto, VesselVisitNotificationDto, VesselVisitNotificationFilter, VesselVisitNotificationFilterPa } from "@/model/dto/VesselVisitNotificationDto";

@injectable()
export class VesselVisitNotificationService implements IVesselVisitNotificationService {
    constructor(
        @inject(TYPES.api)
        private http: IHttpService
    ) { }

    async count(): Promise<VesselVisitDistributionDto> {
        
        return (await this.http.get<VesselVisitDistributionDto>("/api/VesselVisitNotification/distribution")).data;
    }

    async getVesselVisitNotifications(filter: Filter<VesselVisitNotificationFilterPa>): Promise<Page<VesselVisitNotification>> {

        const query: string[] = [];
        if (filter) {
            query.push(filter.pageNumber !== undefined ? `PageNumber=${filter.pageNumber}&` : "");
            query.push(filter.pageSize !==undefined ? `PageSize=${filter.pageSize}` : "");
        }

        const res = await this.http.get<Page<VesselVisitNotification>>(`/api/VesselVisitNotification/filterPa${query.length ? `?${query.join('')}` : ''}`);
        return res.data;
    }

    async getVesselVisitNotificationsForReview(filter: Filter<null>): Promise<Page<VesselVisitNotification>> {

        const query: string[] = [];
        if (filter) {
            query.push(filter.pageNumber !== undefined ? `PageNumber=${filter.pageNumber}&` : "");
            query.push(filter.pageSize !==undefined ? `PageSize=${filter.pageSize}` : "");
        }

        const res = await this.http.get<Page<VesselVisitNotification>>(`/api/VesselVisitNotification/filterPa?OnlyPending=true`);
        return res.data;
    }

    async getVesselVisitNotificationsByRepresentative(filter?: Filter<VesselVisitNotificationFilter>): Promise<Page<VesselVisitNotification>> {
        let query: string[] = [];

        if (filter) {
            query.push(filter.filter.Status ? `Status=${filter.filter.Status}&` : "");
            query.push(filter.filter.WithReason ? `WithReason=${filter.filter.WithReason}&` : "");
            query.push(filter.filter.WithDockAssigned ? `WithDockAssigned=${filter.filter.WithDockAssigned}&` : "");
            query.push(filter.filter.Vessel ? `Vessel.Value=${encodeURIComponent(filter.filter.Vessel)}&` : "");
            query.push(filter.filter.ExpectedArrivalFrom ? `ExpectedArrivalFrom=${encodeURIComponent(filter.filter.ExpectedArrivalFrom.toISOString())}&` : "");
            query.push(filter.filter.ExpectedArrivalTo ? `ExpectedArrivalTo=${encodeURIComponent(filter.filter.ExpectedArrivalTo.toISOString())}&` : "");

            query.push(filter.pageNumber !== undefined ? `PageNumber=${filter.pageNumber}&` : "");
            query.push(filter.pageSize !==undefined ? `PageSize=${filter.pageSize}` : "");
        }
        console.log(query)
        try{
            const res = await this.http.get<Page<VesselVisitNotification>>(`/api/VesselVisitNotification/filter${query.length ? `?${query.join('')}` : ''}`);
            return res.data;
        }
        catch{
            return { items: [], pageNumber: 1, pageSize: 0, pageCount: 0 };
        }
    }

    async getVesselVisitNotificationById(id: string): Promise<VesselVisitNotification> {
        const res = await this.http.get<VesselVisitNotification>(`/api/VesselVisitNotification/${id}`);
        return res.data;
    }

    async getNotificationDecisions(vesselVisitNotificationId: string): Promise<NotificationDecision[]> {
        const res = await this.http.get<NotificationDecision[]>(`/api/VesselVisitNotification/decisions?vesselVisitNotificationId=${vesselVisitNotificationId}`);
        return res.data;
    }

    async createVesselVisitNotification(notification: VesselVisitNotification): Promise<VesselVisitNotification> {
        const res = await this.http.post<VesselVisitNotification>("/api/VesselVisitNotification", notification.toDto());
        return res.data;
    }

    async createNotificationDecision(vesselVisitNotificationId: string, decision: NotificationDecisionDto): Promise<NotificationDecision> {
        const res = await this.http.post<NotificationDecision>(
            `/api/VesselVisitNotification/decisions?vesselVisitNotificationId=${vesselVisitNotificationId}`,
            decision
        );
        return res.data;
    }

    async updateVesselVisitNotification(notification: VesselVisitNotification): Promise<VesselVisitNotification> {
        console.log(notification.toDto());
        const res = await this.http.put<VesselVisitNotification>(`/api/VesselVisitNotification/${notification.notificationId}`, notification.toDto());
        return res.data;
    }

    async submitVesselVisitNotification(id: string): Promise<void> {
        await this.http.put<void>(`/api/VesselVisitNotification/submit/${id}`, {});
    }

    async deleteDraft(id: string): Promise<void> {
        await this.http.delete<void>(`/api/VesselVisitNotification?id=${id}`);
    }
}