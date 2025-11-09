import { injectable, inject } from "inversify";
import { TYPES } from "@/inversify/types";
import type { IHttpService } from "./IService/IHttpService";
import type { IVesselVisitNotificationService } from "./IService/IVesselVisitNotificationService";
import type { Filter, Page } from "@/model/Page";
import type {
    VesselVisitNotification,
    NotificationDecision,
    VesselVisitNotificationFilter
} from "@/model/VesselVisitNotification";

@injectable()
export class VesselVisitNotificationService implements IVesselVisitNotificationService {
    constructor(
        @inject(TYPES.api)
        private http: IHttpService
    ) { }

    async getVesselVisitNotifications(): Promise<Page<VesselVisitNotification>> {
        const res = await this.http.get<VesselVisitNotification[]>(`/VesselVisitNotification`);
        const page: Page<VesselVisitNotification> = {
            items: res.data || [],
            pageNumber: 1,
            pageSize: res.data ? res.data.length : 0,
            pageCount: res.data && res.data.length > 0 ? 1 : 0
        };
        return page;
    }

    async getVesselVisitNotificationsByRepresentative(filter?: Filter<VesselVisitNotificationFilter>): Promise<Page<VesselVisitNotification>> {
        let queryString: string[] = [];

        if (filter) {
            queryString.push(filter.filter.Status !== undefined ? `Status=${encodeURIComponent(filter.filter.Status)}` : "");
            queryString.push(filter.filter.WithReason !== undefined ? `WithReason=${filter.filter.WithReason}` : "");
            queryString.push(filter.filter.WithDockAssigned !== undefined ? `WithDockAssigned=${filter.filter.WithDockAssigned}` : "");
            queryString.push(filter.filter.Vessel !== undefined ? `Vessel=${encodeURIComponent(filter.filter.Vessel)}` : "");
            queryString.push(filter.filter.ExpectedArrivalFrom !== undefined ? `ExpectedArrivalFrom=${encodeURIComponent(filter.filter.ExpectedArrivalFrom.toISOString())}` : "");
            queryString.push(filter.filter.ExpectedArrivalTo !== undefined ? `ExpectedArrivalTo=${encodeURIComponent(filter.filter.ExpectedArrivalTo.toISOString())}` : "");

            queryString.push(filter.pageNumber !== undefined ? `pageNumber=${filter.pageNumber}` : "");
            queryString.push(filter.pageSize !== undefined ? `pageSize=${filter.pageSize}` : "");
        }
        const query = queryString.filter(Boolean).join("&");
        try{
            const res = await this.http.get<Page<VesselVisitNotification>>(`/VesselVisitNotification/filter?${query}`);
            return res.data;
        }
        catch{
            return { items: [], pageNumber: 1, pageSize: 0, pageCount: 0 };
        }
    }

    async getVesselVisitNotificationById(id: string): Promise<VesselVisitNotification> {
        const res = await this.http.get<VesselVisitNotification>(`/VesselVisitNotification/${id}`);
        return res.data;
    }

    async getNotificationDecisions(vesselVisitNotificationId: string): Promise<NotificationDecision[]> {
        const res = await this.http.get<NotificationDecision[]>(`/VesselVisitNotification/decisions?vesselVisitNotificationId=${vesselVisitNotificationId}`);
        return res.data;
    }

    async createVesselVisitNotification(notification: VesselVisitNotification): Promise<VesselVisitNotification> {
        const res = await this.http.post<VesselVisitNotification>("/VesselVisitNotification", notification);
        return res.data;
    }

    async createNotificationDecision(vesselVisitNotificationId: string, decision: NotificationDecision): Promise<NotificationDecision> {
        const res = await this.http.post<NotificationDecision>(
            `/VesselVisitNotification/decisions?vesselVisitNotificationId=${vesselVisitNotificationId}`,
            decision
        );
        return res.data;
    }

    async updateVesselVisitNotification(id: string, notification: VesselVisitNotification): Promise<VesselVisitNotification> {
        const res = await this.http.put<VesselVisitNotification>(`/VesselVisitNotification/${id}`, notification);
        return res.data;
    }

    async submitVesselVisitNotification(id: string): Promise<void> {
        await this.http.put<void>(`/VesselVisitNotification/submit/${id}`, {});
    }

    async deleteDraft(id: string): Promise<void> {
        await this.http.delete<void>(`/VesselVisitNotification?id=${id}`);
    }
}