import type { IAdminService } from "./IService/IAdminService";
import type { IHttpService } from "./IService/IHttpService";
import { TYPES } from "@/inversify/types";
import type { Filter, Page } from "@/model/Page";
import type { SystemUser } from "@/model/SystemUser";
import {inject, injectable} from 'inversify';


@injectable()
export class AdminService implements IAdminService {
    constructor(
		@inject(TYPES.api) 
		private http: IHttpService
	){}

    async changeUserRole(emailAddress: string, newRole: number): Promise<SystemUser> {
        const res = await this.http.put<SystemUser>(`/SystemUser/${encodeURIComponent(emailAddress)}/role?role=${encodeURIComponent(newRole)}`, { });
        return res.data;
    }

    async activateUserAccount(emailAddress: string): Promise<void> {
        await this.http.put<void>(`/SystemUser/${encodeURIComponent(emailAddress)}/activate`, { });
    }

    async deactivateUserAccount(emailAddress: string): Promise<void> {
        await this.http.put<void>(`/SystemUser/${encodeURIComponent(emailAddress)}/deactivate`, { });
    }

    async createUser(emailAddress: string, role: number): Promise<SystemUser> {
        const res = await this.http.post<SystemUser>('/SystemUser', {
                "sub":"",
                "isActive": false,
                "role": role,
                "email": emailAddress 
            });
        return res.data;
    }

    async inviteUser(emailAddress: string, role: number): Promise<SystemUser> {
        const res = await this.createUser(emailAddress, role);
        await this.changeUserRole(emailAddress, role);
        return res;
    }

    async getAllUsers(): Promise<Array<SystemUser>> {
        const res = await this.http.get<Array<SystemUser>>('/SystemUser');
        return res.data;
    }

    async getByEmail(emailAddress: string): Promise<SystemUser> {
        const res = await this.http.get<SystemUser>(`/SystemUser/${encodeURIComponent(emailAddress)}`);
        return res.data;
    }

    async deleteUser(emailAddress: string): Promise<void> {
        await this.http.delete<void>(`/SystemUser/${encodeURIComponent(emailAddress)}`);
    }

    async filterSystemUsers(filter: Filter<SystemUser>): Promise<Page<SystemUser>> {
        let query: string[] = [];

        if (filter) {
            query.push(filter.filter.emailAddress ? `Email=${encodeURIComponent(filter.filter.emailAddress)}&` : '');
            query.push(filter.filter.role !== undefined ? `Role=${encodeURIComponent(filter.filter.role)}&` : '');
            query.push(filter.filter.isActive !== undefined ? `IsActive=${encodeURIComponent(filter.filter.isActive)}&` : '');
            query.push(filter.pageNumber !== undefined ? `PageNumber=${encodeURIComponent(filter.pageNumber)}&` : '');
            query.push(filter.pageSize !== undefined ? `PageSize=${encodeURIComponent(filter.pageSize)}` : '');
        }
        const res = await this.http.get<Page<SystemUser>>(`/SystemUser/filter${query.length ? `?${query.join('')}` : ''}`);
        return res.data;
    }
}