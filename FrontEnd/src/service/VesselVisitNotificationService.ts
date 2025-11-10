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

    async getVesselVisitNotificationsForReview(): Promise<Page<VesselVisitNotification>> {
        const res = await this.getVesselVisitNotifications();
        const filteredItems = res.items.filter(item => item.status === 1);
        const page: Page<VesselVisitNotification> = {
            items: filteredItems,
            pageNumber: 1,
            pageSize: filteredItems.length,
            pageCount: filteredItems.length > 0 ? 1 : 0
        };
        return page;
    }

    async getVesselVisitNotifcationsByDay(date: Date): Promise<VesselVisitNotification[]> {    
        // Format to YYYY-MM-DD 
        const formatedDate = date.toISOString().split('T')[0];
        console.log(`/VesselVisitNotification/onDay?day=${formatedDate}`);
        const res = await this.http.get<VesselVisitNotification[]>(`/VesselVisitNotification/onDay?day=${formatedDate}`);
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
            const res = await this.http.get<Page<VesselVisitNotification>>(`/VesselVisitNotification/filter${query.length ? `?${query.join('')}` : ''}`);
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