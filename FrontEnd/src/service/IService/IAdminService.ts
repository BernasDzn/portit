import type { Logs } from "@/model/values/Logs";
import type { Filter, Page } from "@/model/Page";
import type { SystemUser } from "@/model/SystemUser";

export interface IAdminService {
    changeUserRole(emailAddress: string, newRole: number): Promise<SystemUser>;
    activateUserAccount(emailAddress: string): Promise<void>;
    deactivateUserAccount(emailAddress: string): Promise<void>;
    createUser(emailAddress: string, role: number): Promise<SystemUser>;
    getAllUsers(): Promise<Array<SystemUser>>;
    getByEmail(emailAddress: string): Promise<SystemUser>;
    deleteUser(emailAddress: string): Promise<void>;
    filterSystemUsers(filter: Filter<SystemUser>): Promise<Page<SystemUser>>;
    getLogs(): Promise<Logs[]>;
    inviteUser(emailAddress: string, role: number): Promise<SystemUser>;
}