import { injectable, inject } from "inversify";
import { TYPES } from "@/inversify/types";
import type { IHttpService } from "./IService/IHttpService";
import type { IVesselVisitNotificationService } from "./IService/IVesselVisitNotificationService";
import type { Filter, Page } from "@/model/Page";
import type {

VesselVisitNotification,
VesselVisitNotificationStatus,
NotificationDecision
} from "@/model/VesselVisitNotification";

@injectable()
export class VesselVisitNotificationService implements IVesselVisitNotificationService {
constructor(
    @inject(TYPES.api)
    private http: IHttpService
) {}

async getVesselVisitNotifications(): Promise<VesselVisitNotification[]> {
    const res = await this.http.get<VesselVisitNotification[]>("/VesselVisitNotification");
    return res.data;
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

async filterVesselVisitNotifications(filter: Filter<VesselVisitNotificationStatus>): Promise<Page<VesselVisitNotificationStatus>> {
    // Build query string from filter object
    let query: string[] = [];
    if (filter) {
        query.push(filter.filter !== undefined ? `Status=${filter.filter}` : "");
        query.push(filter.pageNumber !== undefined ? `PageNumber=${filter.pageNumber}` : "");
        query.push(filter.pageSize !== undefined ? `PageSize=${filter.pageSize}` : "");
    }
    const queryString = query.filter(Boolean).join("&");
    const res = await this.http.get<Page<VesselVisitNotificationStatus>>(
        `/VesselVisitNotification/filter${queryString ? `?${queryString}` : ""}`
    );
    return res.data;
}

async deleteDraft(id: string): Promise<void> {
    await this.http.delete<void>(`/VesselVisitNotification?id=${id}`);
}
}